namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class ChartEntry
    {
        #region Constructors

        public ChartEntry(string name, string unit)
        {
            this.Name = name;
            this.Unit = unit;
        }

        #endregion

        #region Properties

        public string Name { get; set; }
        public string Unit { get; set; }

        #endregion
    }
}
