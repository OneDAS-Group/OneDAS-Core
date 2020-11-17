namespace OneDas.DataManagement.Database
{
    public class ProjectLicense
    {
        #region Constructors

        public ProjectLicense()
        {
            this.LicensingScheme = ProjectLicensingScheme.None;
            this.DisplayMessage = string.Empty;
            this.FileMessage = string.Empty;
            this.Url = string.Empty;
        }

        #endregion

        #region Properties

        public ProjectLicensingScheme LicensingScheme { get; set; }

        public string DisplayMessage { get; set; }

        public string FileMessage { get; set; }

        public string Url { get; set; }

        #endregion
    }
}
