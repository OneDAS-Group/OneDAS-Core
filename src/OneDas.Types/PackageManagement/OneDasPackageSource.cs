namespace OneDas.PackageManagement
{
    public class OneDasPackageSource
    {
        #region "Constructors"

        public OneDasPackageSource(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }

        #endregion

        #region "Properties"

        public string Name { get; set; }

        public string Address { get; set; }

        #endregion
    }
}
