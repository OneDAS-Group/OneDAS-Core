using System.Collections.Generic;

namespace OneDas.DataManagement.Database
{
    public class AggregationConfig
    {
        #region Constructors

        public AggregationConfig()
        {
            this.ProjectName = string.Empty;
            this.Method = AggregationMethod.Mean;
            this.Argument = string.Empty;
            this.Filters = new Dictionary<string, string>();
        }

        #endregion

        #region Properties

        public string ProjectName { get; set; }

        public AggregationMethod Method { get; set; }

        public string Argument { get; set; }

        public Dictionary<string, string> Filters { get; set; }

        #endregion
    }
}
