using OneDas.Extensibility;
using System.Runtime.Serialization;

namespace OneDas.Extension.Mat73
{
    [DataContract]
    [ExtensionContext(typeof(Mat73Writer))]
    [ExtensionIdentification("MAT73", "Matlab files (v7.3)", "Store data in Matlab's hierachical data format (v7.3).", @"WebClient.Mat73View.html", @"WebClient.Mat73.js")]
    public class Mat73Settings : DataWriterExtensionSettingsBase
    {
        #region "Constructors"

        public Mat73Settings()
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
