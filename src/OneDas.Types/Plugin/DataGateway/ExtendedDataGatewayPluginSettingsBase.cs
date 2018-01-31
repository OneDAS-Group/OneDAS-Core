using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using OneDas.Infrastructure;

namespace OneDas.Plugin
{
    [DataContract]
    public abstract class ExtendedDataGatewayPluginSettingsBase : DataGatewayPluginSettingsBase
    {
        #region "Constructors"

        public ExtendedDataGatewayPluginSettingsBase()
        {
            this.ModuleSet = new List<OneDasModule>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public List<OneDasModule> ModuleSet;

        #endregion

        #region "Methods"

        public List<OneDasModule> GetInputModuleSet()
        {
            return this.ModuleSet.Where(module => module.DataDirection == DataDirection.Input).ToList();
        }

        public List<OneDasModule> GetOutputModuleSet()
        {
            return this.ModuleSet.Where(module => module.DataDirection == DataDirection.Input).ToList();
        }

        #endregion
    }
}
