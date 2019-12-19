using Ixxat.Vci4;
using Ixxat.Vci4.Bal.Can;
using System;

namespace OneDas.Extension.Can
{
    public class IxxatUsbToCanV2Compact : ICanDevice
    {
        #region Fields

        private IVciDevice _device;
        private ICanChannel2 _channel;
        private ICanMessageReader _reader;
        private ICanMessageWriter _writer;
        private IMessageFactory _messageFactory;
        private ICanControl _control;

        #endregion

        #region Constructors

        public IxxatUsbToCanV2Compact(CanSettings settings)
        {
            _messageFactory = VciServerImpl.Instance().MsgFactory;

            var deviceManager = VciServerImpl.Instance().DeviceManager;
            var devices = deviceManager.GetDeviceList();

            foreach (IVciDevice device in devices)
            {
                var hardwareId = IxxatUtils.TrimHardwareId((string)device.UniqueHardwareId);

                if (_device == null && hardwareId == settings.HardwareId && device.DeviceClass == IxxatUtils.AcceptedDeviceClass)
                    _device = device;
            }

            if (_device == null)
                throw new InvalidOperationException($"The CAN hardware with ID {settings.HardwareId} could not be found.");

            using (var bal = _device.OpenBusAccessLayer())
            {
                // channel
                _channel = (ICanChannel2)bal.OpenSocket(0, typeof(ICanChannel2));
                _channel.Initialize(10, 10, 0, CanFilterModes.Inclusive, exclusive: false);

                _writer = _channel.GetMessageWriter();
                _reader = _channel.GetMessageReader();
                _reader.Threshold = 1;

                _channel.Activate();

                // control
                _control = (ICanControl)bal.OpenSocket(0, typeof(ICanControl));

                _control.InitLine(
                         operatingMode: CanOperatingModes.Standard | CanOperatingModes.Extended,
                         bitrate: IxxatUtils.CiaBitRateToCanBitrate(settings.BitRate));

                _control.SetAccFilter(CanFilter.Std, (uint)CanAccCode.None, (uint)CanAccMask.None);
                _control.SetAccFilter(CanFilter.Ext, (uint)CanAccCode.None, (uint)CanAccMask.None);

                foreach (CanModule module in settings.GetInputModuleSet())
                {
                    switch (module.FrameFormat)
                    {
                        case CanFrameFormat.Standard:
                            _control.AddFilterIds(CanFilter.Std, module.Identifier << 1, 0xFFF);
                            break;

                        case CanFrameFormat.Extended:
                            _control.AddFilterIds(CanFilter.Ext, module.Identifier << 1, 0x3FFFFFFF);
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                }

                _control.StartLine();
            }
        }

        #endregion

        #region Properties

        public int AvailableMessagesCount => _reader.FillCount;

        #endregion

        #region Methods

        public void Send(uint identifier, CanFrameFormat frameFormat, Span<byte> data)
        {
            if (data.Length > 8)
                throw new FormatException(ErrorMessage.MessageSizeExceeded);

            var message = (ICanMessage)_messageFactory.CreateMsg(typeof(ICanMessage));

            message.TimeStamp = 0;
            message.Identifier = message.Identifier;
            message.FrameType = CanMsgFrameType.Data;
            message.DataLength = (byte)data.Length;
            message.ExtendedFrameFormat = frameFormat == CanFrameFormat.Extended;
            message.SelfReceptionRequest = false;

            for (int i = 0; i < data.Length; i++)
            {
                message[i] = data[i];
            }

            _writer.SendMessage(message);
        }

        public bool Receive(out uint identifier, out byte[] data)
        {
            identifier = default;
            data = default;

            _reader.ReadMessage(out ICanMessage ixxatMessage);

            if (ixxatMessage.FrameType == CanMsgFrameType.Data && !ixxatMessage.RemoteTransmissionRequest)
            {
                identifier = ixxatMessage.Identifier;
                data = new byte[ixxatMessage.DataLength];

                for (int i = 0; i < ixxatMessage.DataLength; i++)
                {
                    data[i] = ixxatMessage[i];
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            try
            {
                using (var bal = _device.OpenBusAccessLayer())
                {
                    using (var control = (ICanControl)bal.OpenSocket(0, typeof(ICanControl)))
                    {
                        control.ResetLine();
                    }
                }
            }
            catch (Exception)
            {
                //
            }

            _reader?.Dispose();
            _writer?.Dispose();
            _channel?.Dispose();
            _control?.Dispose();
        }

        #endregion
    }
}
