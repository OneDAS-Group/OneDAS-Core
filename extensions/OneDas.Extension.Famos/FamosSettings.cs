using OneDas.Extensibility;
using System.Runtime.Serialization;

namespace OneDas.Extension.Famos
{
    [DataContract]
    [ExtensionContext(typeof(FamosWriter))]
    [ExtensionIdentification("Famos", "FAMOS (imc2)", "Store data in FAMOS v2 .dat files (imc2).", @"WebClient.FamosView.html", @"WebClient.Famos.js")]
    public class FamosSettings : DataWriterExtensionSettingsBase
    {
        #region "Constructors"

        public FamosSettings()
        {
            //
        }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            base.Validate();
        }

        #endregion
    }
}
