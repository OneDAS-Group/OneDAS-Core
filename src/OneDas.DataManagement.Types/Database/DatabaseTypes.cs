namespace OneDas.DataManagement.Database
{
    public enum ProjectLicensingScheme
    {
        None = 0,
        ManualRequest = 1,
        AcceptLicense = 2
    }

    public record ProjectLicense
    {
        public ProjectLicensingScheme LicensingScheme { get; init; } = ProjectLicensingScheme.None;
        public string DisplayMessage { get; init; } = string.Empty;
        public string FileMessage { get; init; } = string.Empty;
        public string Url { get; init; } = string.Empty;
    }

    public record DataReaderRegistration
    {
        public string RootPath { get; set; }
        public string DataReaderId { get; set; }
    }
}
