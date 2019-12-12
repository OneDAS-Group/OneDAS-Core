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

                    var devices = new Dictionary<string, string>();
                    var deviceManager = VciServerImpl.Instance().DeviceManager;

                    foreach (IVciDevice device in deviceManager.GetDeviceList())
                    {
                        if (device.DeviceClass == IxxatUtils.AcceptedDeviceClass)
                        {
                            var hardwareId = IxxatUtils.TrimHardwareId((string)device.UniqueHardwareId);
                            devices.Add(device.Description, hardwareId);
                        }
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
