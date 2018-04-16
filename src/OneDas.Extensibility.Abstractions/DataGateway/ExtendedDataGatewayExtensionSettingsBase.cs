using OneDas.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Extensibility
{
    [DataContract]
    public abstract class ExtendedDataGatewayExtensionSettingsBase : DataGatewayExtensionSettingsBase
    {
        #region "Constructors"

        public ExtendedDataGatewayExtensionSettingsBase()
        {
            this.ModuleSet = new List<OneDasModule>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public List<OneDasModule> ModuleSet { get; set; }

        #endregion

        #region "Methods"

        public List<OneDasModule> GetInputModuleSet()
        {
            return this.ModuleSet.Where(module => module.DataDirection == DataDirection.Input).ToList();
        }

        public List<OneDasModule> GetOutputModuleSet()
        {
            return this.ModuleSet.Where(module => module.DataDirection == DataDirection.Output).ToList();
        }

        #endregion
    }
}
