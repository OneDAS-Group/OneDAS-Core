using Ixxat.Vci4;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;

namespace OneDas.Extension.Can
{
    public class CanSupporter : IExtensionSupporter
    {
        #region "Constructors"

        public CanSupporter()
        {
            //
        }

        #endregion

        #region "Methods"

        public void Initialize()
        {
            //
        }

        public ActionResponse HandleActionRequest(ActionRequest actionRequest)
        {
            object returnData;

            switch (actionRequest.MethodName)
            {
                case "GetDevices":

                    var deviceType = (CanDeviceType)(long)actionRequest.Data;
                    var devices = new Dictionary<string, string>();

                    switch (deviceType)
                    {
                        case CanDeviceType.IxxatUsbToCanV2Compact:

                            var deviceManager = VciServerImpl.Instance().DeviceManager;

                            foreach (IVciDevice device in deviceManager.GetDeviceList())
                            {
                                if (device.DeviceClass == IxxatUtils.AcceptedDeviceClass)
                                {
                                    var hardwareId = IxxatUtils.TrimHardwareId((string)device.UniqueHardwareId);
                                    devices.Add(device.Description, hardwareId);
                                }
                            }

                            break;

                        case CanDeviceType.CanLoopbackDevice:

                            devices = new Dictionary<string, string>() 
                            {
                                ["CAN Loopback Interface"] = "0"
                            };

                            break;

                        default:
                            throw new NotSupportedException($"The device type {deviceType} is not supported.");
                    }

                    returnData = devices;

                    break;

                default:
                    throw new ArgumentException("unknown method name");
            }

            return new ActionResponse(returnData);
        }

        #endregion
    }
}
