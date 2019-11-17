using EtherCAT.NET.Infrastructure;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Extension.Ethercat
{
    [DataContract]
    [ExtensionContext(typeof(EthercatGateway))]
    [ExtensionSupporter(typeof(EthercatSupporter))]
    [ExtensionIdentification("EtherCAT", "EtherCAT", "Access EtherCAT networks to read and write data periodically.", @"WebClient.EthercatView.html", @"WebClient.Ethercat.js")]
    public class EthercatSettings : DataGatewayExtensionSettingsBase
    {
        #region "Constructors"

        public EthercatSettings()
        {
            //
        }

        #endregion

        #region "Properties"

        [DataMember(IsRequired = true)]
        public string NicHardwareAddress { get; set; }

        [DataMember(IsRequired = true)]
        public SlaveInfo RootSlaveInfo { get; set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            List<SlaveInfo> slaveInfoSet;
            IEnumerable<ushort> csaSet;

            base.Validate();

            slaveInfoSet = this.RootSlaveInfo != null ? this.RootSlaveInfo.Descendants().ToList() : new List<SlaveInfo>();
            csaSet = slaveInfoSet.Select(x => x.Csa);

            if (csaSet.Count() > csaSet.Distinct().Count())
            {
                throw new Exception(ErrorMessage.EthercatSettings_SlaveInfoIdNotUnique);
            }

            slaveInfoSet.ForEach(slaveInfo => slaveInfo.Validate());
        }

        #endregion
    }
}
