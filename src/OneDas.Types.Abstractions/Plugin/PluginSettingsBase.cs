using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace OneDas.Plugin
{
    [DataContract]
    public abstract class PluginSettingsBase
    {
        #region "Constructors"

        public PluginSettingsBase()
        {
            PluginIdentificationAttribute pluginIdentificationAttribute;

            if (!this.GetType().IsDefined(typeof(PluginContextAttribute), false))
            {
                throw new Exception(ErrorMessage.PluginSettingsBase_PluginContextAttributeNotDefined);
            }

            if (!this.GetType().IsDefined(typeof(PluginIdentificationAttribute), false))
            {
                throw new Exception(ErrorMessage.PluginSettingsBase_PluginDescriptionAttributeNotDefined);
            }

            pluginIdentificationAttribute = this.GetType().GetFirstAttribute<PluginIdentificationAttribute>();

            this.Description = new PluginDescription(FileVersionInfo.GetVersionInfo(this.GetType().Assembly.Location).ProductVersion, pluginIdentificationAttribute.Id, true);
        }

        #endregion

        #region "Properties"

        [DataMember]
        public PluginDescription Description { get; set; }

        #endregion

        #region "Methods"

        public virtual void Validate()
        {
            this.Description.Validate();
        }

        #endregion
    }
}
