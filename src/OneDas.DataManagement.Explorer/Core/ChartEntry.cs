namespace OneDas.DataManagement.Explorer.Core
{
    public class ChartEntry
    {
        #region Constructors

        public ChartEntry(string name, string path, string unit)
        {
            this.Name = name;
            this.Path = path;
            this.Unit = unit;
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public string Path { get; set; }

        public string Unit { get; set; }

        #endregion
    }
}
