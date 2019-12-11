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

                    var devices = new List<string>();
                    var deviceManager = VciServer.Instance().DeviceManager;

                    foreach (IVciDevice device in deviceManager.GetDeviceList())
                    {
                        if (device.DeviceClass == IxxatUtils.AcceptedDeviceClass)
                        {
                            var hardwareId = IxxatUtils.TrimHardwareId((string)device.UniqueHardwareId);
                            devices.Add(hardwareId);
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
