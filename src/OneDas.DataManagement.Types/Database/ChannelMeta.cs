using System.Diagnostics;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Id,nq}")]
    public class ChannelMeta
    {
        #region "Constructors"

        public ChannelMeta(string id)
        {
            this.Id = id;
            this.Description = string.Empty;
            this.SpecialInfo = string.Empty;
            this.Unit = string.Empty;
        }

        private ChannelMeta()
        {
            //
        }

        #endregion

        #region "Properties"

        public string Id { get; set; }

        public string Description { get; set; }

        public string SpecialInfo { get; set; }

        public string Unit { get; set; }

        #endregion
    }
}
