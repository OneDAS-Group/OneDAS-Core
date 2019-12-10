using Ixxat.Vci4;
using Ixxat.Vci4.Bal.Can;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Extension.CanRaw
{
    class Sample
    {
        private ICanMessageReader _reader;
        private AutoResetEvent _autoResetEvent;
        private CancellationTokenSource _cts;

        public void Run()
        {
            _autoResetEvent = new AutoResetEvent(false);
            _cts = new CancellationTokenSource();

            var deviceManager = VciServer.Instance().DeviceManager;
            var devices = deviceManager.GetDeviceList();

            var enumerator = devices.GetEnumerator();
            enumerator.MoveNext();

            var device = (IVciDevice)enumerator.Current;

            using (var bal = device.OpenBusAccessLayer())
            {
                // channel
                var channel = (ICanChannel2)bal.OpenSocket(0, typeof(ICanChannel2));

                channel.Initialize(10, 10, 3, CanFilterModes.Inclusive, false);
                //channel.SetAccFilter(CanFilter.Std, (uint)CanAccCode.None, (uint)CanAccMask.None);
                //channel.AddFilterIds(CanFilter.Std, 0x01 << 1, 0xFFF);
                //channel.AddFilterIds(CanFilter.Std, 0x02 << 1, 0xFFF);
                //channel.AddFilterIds(CanFilter.Std, 0x03 << 1, 0xFFF);

                _reader = channel.GetMessageReader();
                _reader.Threshold = 1;
                _reader.AssignEvent(_autoResetEvent);

                channel.Activate();

                // control
                using (var control = (ICanControl)bal.OpenSocket(0, typeof(ICanControl)))
                {
                    control.InitLine(
                        operatingMode: CanOperatingModes.Standard | CanOperatingModes.Extended | CanOperatingModes.ErrFrame,
                        bitrate: CanBitrate.Cia125KBit);

                    control.SetAccFilter(CanFilter.Std, (uint)CanAccCode.None, (uint)CanAccMask.None);
                    control.AddFilterIds(CanFilter.Std, 0x01 << 1, 0xFFF);
                    control.AddFilterIds(CanFilter.Std, 0x02 << 1, 0xFFF);

                    control.StartLine();
                }
            }

            Task.Run(ReadMessage);
        }

        private void ReadMessage()
        {
            while (!_cts.IsCancellationRequested)
            {
#warning currently not cancelable
                _autoResetEvent.WaitOne();

                while (_reader.ReadMessage(out ICanMessage message))
                {
                    var stringBuilder = new StringBuilder();

                    switch (message.FrameType)
                    {
                        case CanMsgFrameType.Data:

                            if (!message.RemoteTransmissionRequest)
                            {
                                for (int i = 0; i < message.DataLength; i++)
                                {
                                    // message[i];
                                }
                            }

                            break;

                        default:
                            stringBuilder.Append("Other.");
                            break;
                    }

                    Console.WriteLine(stringBuilder.ToString());
                }
            }
        }
    }
}
