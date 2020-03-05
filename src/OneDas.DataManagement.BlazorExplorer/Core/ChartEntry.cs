namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class ChartEntry
    {
        #region Constructors

        public ChartEntry(string name, string unit, double[] data)
        {
            this.Name = name;
            this.Unit = unit;
            this.Data = data;
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public string Unit { get; set; }

        public double[] Data { get; set; }

        #endregion
    }
}
