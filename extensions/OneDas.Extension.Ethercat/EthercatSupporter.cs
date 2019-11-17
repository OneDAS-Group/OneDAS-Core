using EtherCAT.NET;
using EtherCAT.NET.Extensibility;
using EtherCAT.NET.Infrastructure;
using Microsoft.Extensions.Options;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OneDas.Extension.Ethercat
{
    public class EthercatSupporter : IExtensionSupporter
    {
        #region "Fields"

        private OneDasOptions _options;

        private IExtensionFactory _extensionFactory;
        private IOneDasSerializer _oneDasSerializer;

        #endregion

        #region "Constructors"

        public EthercatSupporter(IExtensionFactory extensionFactory, IOneDasSerializer oneDasSerializer, IOptions<OneDasOptions> options)
        {
            _extensionFactory = extensionFactory;
            _oneDasSerializer = oneDasSerializer;

            _options = options.Value;
        }

        #endregion

        #region Properties

        public string EsiSourceDirectoryPath => Path.Combine(_options.ConfigurationDirectoryPath, "EtherCAT", "ESI");

        #endregion

        #region "Methods"

        public void Initialize()
        {
            Directory.CreateDirectory(this.EsiSourceDirectoryPath);
        }

        public ActionResponse HandleActionRequest(ActionRequest actionRequest)
        {
            object returnData;
            string nicHardwareAddress;
            Dictionary<string, object> dictionary;
            SlaveInfo slaveInfo;

            switch (actionRequest.MethodName)
            {
                case "ReloadHardware":

                    // Improve: validation required
                    dictionary = _oneDasSerializer.Deserialize<Dictionary<string, object>>(actionRequest.Data);
                    nicHardwareAddress = nicHardwareAddress = (string)dictionary["NicHardwareAddress"];

                    if (dictionary["RootSlaveInfo"] != null)
                    {
                        slaveInfo = _oneDasSerializer.Deserialize<SlaveInfo>(dictionary["RootSlaveInfo"]);
                    }
                    else
                    {
                        slaveInfo = null;
                    }

                    returnData = ExtensibilityHelper.ReloadHardware(this.EsiSourceDirectoryPath, _extensionFactory, nicHardwareAddress, slaveInfo);

                    break;

                case "GetAvailableNetworkInterfaces":

                    returnData = EcUtilities.GetAvailableNetworkInterfaces();

                    break;

                case "GetDynamicSlaveInfoData":

                    slaveInfo = _oneDasSerializer.Deserialize<SlaveInfo>(actionRequest.Data);

                    if (slaveInfo.ProductCode != 0x00000000)
                    {
                        slaveInfo.Validate();
                    }

                    returnData = ExtensibilityHelper.GetDynamicSlaveInfoData(this.EsiSourceDirectoryPath, _extensionFactory, slaveInfo);

                    break;

                case "GetExtensionIdentifications":

                    returnData = this.GetExtensionIdentifications();

                    break;

                case "GetOpModes":

                    slaveInfo = _oneDasSerializer.Deserialize<SlaveInfo>(actionRequest.Data);
                    slaveInfo.Validate();

                    ExtensibilityHelper.CreateDynamicData(this.EsiSourceDirectoryPath, _extensionFactory, slaveInfo);

                    returnData = slaveInfo.GetOpModes();

                    break;

                default:

                    throw new ArgumentException("unknown method name");
            }

            return new ActionResponse(returnData);
        }

        public IEnumerable<ExtensionIdentificationAttribute> GetExtensionIdentifications()
        {
            return _extensionFactory.Get<SlaveExtensionSettingsBase>().Select(x => x.GetFirstAttribute<ExtensionIdentificationAttribute>()).ToList();
        }

        #endregion
    }
}
