using OneDas.Extensibility;
using System.Runtime.Serialization;

namespace OneDas.Extension.Hdf
{
    [DataContract]
    [ExtensionContext(typeof(HdfWriter))]
    [ExtensionIdentification("HDF", "HDF files", "Store data in hierachical data format files.", "WebClient.HdfView.html", "WebClient.Hdf.js")]
    public class HdfSettings : DataWriterExtensionSettingsBase
    {
        #region "Constructors"

        public HdfSettings()
        {
            this.FileGranularity = Infrastructure.FileGranularity.Day;
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
