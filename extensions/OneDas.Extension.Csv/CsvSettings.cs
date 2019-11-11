using OneDas.Extensibility;
using System.Runtime.Serialization;

namespace OneDas.Extension.Csv
{
    [DataContract]
    [ExtensionContext(typeof(CsvWriter))]
    [ExtensionIdentification("CSV", "Comma-separated values", "Store data in comma-separated values files.", @"WebClient.CsvView.html", @"WebClient.Csv.js")]
    public class CsvSettings : DataWriterExtensionSettingsBase
    {
        #region "Constructors"

        public CsvSettings()
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
