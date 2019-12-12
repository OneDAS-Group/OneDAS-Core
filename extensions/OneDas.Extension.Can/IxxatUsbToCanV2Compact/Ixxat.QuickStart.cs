using Ixxat.Vci4;
using Ixxat.Vci4.Bal;
using Ixxat.Vci4.Bal.Can;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Extension.Can
{
    internal class QuickStart
    {
        private ICanMessageReader _reader;
        private AutoResetEvent _autoResetEvent;
        private CancellationTokenSource _cts;

        public void Run()
        {
            // ISO 11898 - 2, also called high speed CAN (bit speeds up to 1 Mbit/s on CAN, 5 Mbit/s on CAN-FD), uses a linear bus terminated at each end with 120 Ω resistors. 
            // ISO 11898 - 3, also called low speed or fault tolerant CAN (up to 125 Kbps), uses a linear bus, star bus or multiple star buses connected by a linear bus and is terminated at each node by a fraction of the overall termination resistance. The overall termination resistance should be about 100 Ω, but not less than 100 Ω.

            // CBFF     Classical Base Frame Format          11 - Bit Identifier, maximal 8 Byte payload, (old: Standard Frame, CAN 2.0A)
            // CEFF     Classical Extended Frame Format      29 - Bit Identifier, maximal 8 Byte payload, (old: Extended Frame, CAN 2.0B)
            // FBFF     FD Base Frame Format                 11 - Bit Identifier, maximal 64 Byte payload
            // FEFF     FD Extended Frame Format             29 - Bit Identifier, maximal 64 Byte payload

            _autoResetEvent = new AutoResetEvent(false);
            _cts = new CancellationTokenSource();

            var deviceManager = VciServerImpl.Instance().DeviceManager;
            var devices = deviceManager.GetDeviceList();

            Console.WriteLine($"Found devices:");

            foreach (IVciDevice device in devices)
            {
                Console.WriteLine();
                Console.WriteLine($"  Manufacturer: {device.Manufacturer}");
                Console.WriteLine($"   Description: {device.Description}");
                Console.WriteLine($"         Class: {device.DeviceClass}");
                Console.WriteLine($"Driver Version: {device.DriverVersion}");
                Console.WriteLine($"    HW Version: {device.HardwareVersion}");
                Console.WriteLine($"         HW ID: {device.UniqueHardwareId}");
                Console.WriteLine($"   Vci Obj. ID: {device.VciObjectId}");

                // device
                using (IBalObject bal = device.OpenBusAccessLayer())
                {
                    // socket
                    using (var socket = (ICanSocket2)bal.OpenSocket(0, typeof(ICanSocket2)))
                    {
                        Console.WriteLine();
                        Console.WriteLine($"# Socket:");
                        Console.WriteLine($"                     BusCoupling: {socket.BusCoupling}");
                        Console.WriteLine($"                         BusName: {socket.BusName}");
                        Console.WriteLine($"                         BusPort: {socket.BusPort}");
                        Console.WriteLine($"                         BusType: {socket.BusType}");
                        Console.WriteLine($"               CanClockFrequency: {socket.CanClockFrequency}");
                        Console.WriteLine($"                  ControllerType: {socket.ControllerType}");
                        Console.WriteLine($"CyclicMessageTimerClockFrequency: {socket.CyclicMessageTimerClockFrequency}");
                        Console.WriteLine($"       CyclicMessageTimerDivisor: {socket.CyclicMessageTimerDivisor}");
                        Console.WriteLine($"    DelayedTXTimerClockFrequency: {socket.DelayedTXTimerClockFrequency}");
                        Console.WriteLine($"           DelayedTXTimerDivisor: {socket.DelayedTXTimerDivisor}");
                        Console.WriteLine($"                        Features: {socket.Features}");
                        Console.WriteLine($"                      LineStatus: {socket.LineStatus}");
                        Console.WriteLine($"           MaxCyclicMessageTicks: {socket.MaxCyclicMessageTicks}");
                        Console.WriteLine($"               MaxDelayedTXTicks: {socket.MaxDelayedTXTicks}");
                        Console.WriteLine($"       MaximumArbitrationBitrate: {socket.MaximumArbitrationBitrate}");
                        Console.WriteLine($"          MaximumFastDataBitrate: {socket.MaximumFastDataBitrate}");
                        Console.WriteLine($"         Supports64BitTimeStamps: {socket.Supports64BitTimeStamps}");
                        Console.WriteLine($"   SupportsAutoBaudrateDetection: {socket.SupportsAutoBaudrateDetection}");
                        Console.WriteLine($"      SupportsBusLoadComputation: {socket.SupportsBusLoadComputation}");
                        Console.WriteLine($"  SupportsCyclicMessageScheduler: {socket.SupportsCyclicMessageScheduler}");
                        Console.WriteLine($"     SupportsDelayedTransmission: {socket.SupportsDelayedTransmission}");
                        Console.WriteLine($"    SupportsErrorFrameGeneration: {socket.SupportsErrorFrameGeneration}");
                        Console.WriteLine($"             SupportsErrorFrames: {socket.SupportsErrorFrames}");
                        Console.WriteLine($"      SupportsExactMessageFilter: {socket.SupportsExactMessageFilter}");
                        Console.WriteLine($"      SupportsExtendedDataLength: {socket.SupportsExtendedDataLength}");
                        Console.WriteLine($"            SupportsFastDataRate: {socket.SupportsFastDataRate}");
                        Console.WriteLine($"    SupportsHighPriorityMessages: {socket.SupportsHighPriorityMessages}");
                        Console.WriteLine($"          SupportsIsoCanFdFrames: {socket.SupportsIsoCanFdFrames}");
                        Console.WriteLine($"          SupportsListenOnlyMode: {socket.SupportsListenOnlyMode}");
                        Console.WriteLine($"       SupportsNonIsoCanFdFrames: {socket.SupportsNonIsoCanFdFrames}");
                        Console.WriteLine($"            SupportsRemoteFrames: {socket.SupportsRemoteFrames}");
                        Console.WriteLine($"      SupportsSingleShotMessages: {socket.SupportsSingleShotMessages}");
                        Console.WriteLine($"         SupportsStdAndExtFrames: {socket.SupportsStdAndExtFrames}");
                        Console.WriteLine($"          SupportsStdOrExtFrames: {socket.SupportsStdOrExtFrames}");
                        Console.WriteLine($"  TimeStampCounterClockFrequency: {socket.TimeStampCounterClockFrequency}");
                        Console.WriteLine($"         TimeStampCounterDivisor: {socket.TimeStampCounterDivisor}");
                    }

                    // channel
                    var channel = (ICanChannel2)bal.OpenSocket(0, typeof(ICanChannel2));

                    //channel.Activate();
                    //channel.AddFilterIds();
                    //channel.Deactivate();
                    //channel.GetFilterMode();
                    //channel.GetMessageReader();
                    //channel.GetMessageWriter();
                    //channel.Initialize();
                    //channel.RemFilterIds();
                    //channel.SetAccFilter();
                    //channel.SetFilterMode();

                    channel.Initialize(10, 10, 0, CanFilterModes.Pass, false);

                    _reader = channel.GetMessageReader();
                    _reader.Threshold = 1;
                    _reader.AssignEvent(_autoResetEvent);

                    channel.Activate();

                    // control
                    using (var control = (ICanControl)bal.OpenSocket(0, typeof(ICanControl)))
                    {
                        //control.InitLine();
                        //control.ResetLine();
                        //control.StartLine();
                        //control.StopLine();
                        //control.SetAccFilter();
                        //control.AddFilterIds();
                        //control.RemFilterIds();

                        control.InitLine(
                            operatingMode: CanOperatingModes.Standard | CanOperatingModes.Extended | CanOperatingModes.ErrFrame,
                            bitrate: CanBitrate.Cia125KBit);

                        // set-up filters here

                        control.StartLine();

                        Console.WriteLine($"{control.LineStatus}");

                        Console.WriteLine();
                        Console.WriteLine($"          Bitrate: {control.LineStatus.Bitrate}");
                        Console.WriteLine($"          Busload: {control.LineStatus.Busload}");
                        Console.WriteLine($" ControllerStatus: {control.LineStatus.ControllerStatus}");
                        Console.WriteLine($"   HasDataOverrun: {control.LineStatus.HasDataOverrun}");
                        Console.WriteLine($"  HasErrorOverrun: {control.LineStatus.HasErrorOverrun}");
                        Console.WriteLine($"IsAutomaticBitrateDetectionEnabled: {control.LineStatus.IsAutomaticBitrateDetectionEnabled}");
                        Console.WriteLine($"        IsBusCErr: {control.LineStatus.IsBusCErr}");
                        Console.WriteLine($"         IsBusOff: {control.LineStatus.IsBusOff}");
                        Console.WriteLine($"IsErrorFramesEnabled: {control.LineStatus.IsErrorFramesEnabled}");
                        Console.WriteLine($"     IsInInitMode: {control.LineStatus.IsInInitMode}");
                        Console.WriteLine($"     IsListenOnly: {control.LineStatus.IsListenOnly}");
                        Console.WriteLine($"IsLowSpeedEnabled: {control.LineStatus.IsLowSpeedEnabled}");
                        Console.WriteLine($"   IsModeExtended: {control.LineStatus.IsModeExtended}");
                        Console.WriteLine($"   IsModeStandard: {control.LineStatus.IsModeStandard}");
                        Console.WriteLine($"  IsModeUndefined: {control.LineStatus.IsModeUndefined}");
                        Console.WriteLine($"IsTransmitPending: {control.LineStatus.IsTransmitPending}");
                        Console.WriteLine($"    OperatingMode: {control.LineStatus.OperatingMode}");
                    }

                    // scheduler
                    using (ICanScheduler2 scheduler = (ICanScheduler2)bal.OpenSocket(0, typeof(ICanScheduler2)))
                    {
                        //scheduler.AddMessage();
                        //scheduler.Reset();
                        //scheduler.Resume();
                        //scheduler.Suspend();
                        //scheduler.UpdateStatus();
                    }
                }
            }

            Task.Run(ReadMessage);

            Console.WriteLine("Press any key to stop.");
            Console.ReadKey(true);
        }

        private void ReadMessage()
        {
            while (!_cts.IsCancellationRequested)
            {
                _autoResetEvent.WaitOne();

                while (_reader.ReadMessage(out ICanMessage2 message))
                {
                    //Console.WriteLine();
                    //Console.WriteLine($"             AcceptReason: {message.AcceptReason}");
                    //Console.WriteLine($"               DataLength: {message.DataLength}");
                    //Console.WriteLine($"      ErrorStateIndicator: {message.ErrorStateIndicator}");
                    //Console.WriteLine($"       ExtendedDataLength: {message.ExtendedDataLength}");
                    //Console.WriteLine($"       ExtendedDataLength: {message.ExtendedFrameFormat}");
                    //Console.WriteLine($"             FastDataRate: {message.FastDataRate}");
                    //Console.WriteLine($"                FrameType: {message.FrameType}");
                    //Console.WriteLine($"          HighPriorityMsg: {message.HighPriorityMsg}");
                    //Console.WriteLine($"               Identifier: {message.Identifier}");
                    //Console.WriteLine($"          PossibleOverrun: {message.PossibleOverrun}");
                    //Console.WriteLine($"RemoteTransmissionRequest: {message.RemoteTransmissionRequest}");
                    //Console.WriteLine($"     SelfReceptionRequest: {message.SelfReceptionRequest}");
                    //Console.WriteLine($"           SingleShotMode: {message.SingleShotMode}");
                    //Console.WriteLine($"                TimeStamp: {message.TimeStamp}");

                    var stringBuilder = new StringBuilder();

                    switch (message.FrameType)
                    {
                        case CanMsgFrameType.Data:

                            stringBuilder.Append($"Time: {message.TimeStamp.ToString()} ID: {message.Identifier.ToString("X3")}h");

                            if (message.RemoteTransmissionRequest)
                            {
                                stringBuilder.Append($" Remote Request Data Length: {message.DataLength}");
                            }
                            else
                            {
                                for (int i = 0; i < message.DataLength; i++)
                                {
                                    stringBuilder.Append($" {message[i - 1].ToString("X2")}");
                                }
                            }

                            if (message.SelfReceptionRequest)
                                stringBuilder.Append(" Self Reception");

                            break;

                        case CanMsgFrameType.Info:
                            stringBuilder.Append("Info.");
                            break;

                        case CanMsgFrameType.Error:
                            stringBuilder.Append("Error.");
                            break;
                        case CanMsgFrameType.Status:
                        case CanMsgFrameType.Wakeup:
                        case CanMsgFrameType.TimeOverrun:
                        case CanMsgFrameType.TimeReset:
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
