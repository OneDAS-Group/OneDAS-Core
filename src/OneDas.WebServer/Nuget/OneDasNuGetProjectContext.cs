using System;
using System.Xml.Linq;
using NuGet.Packaging;
using NuGet.ProjectManagement;

namespace OneDas.WebServer.Nuget
{
    public class OneDasNuGetProjectContext : INuGetProjectContext
    {
        #region "Constructors"

        public OneDasNuGetProjectContext()
        {
            this.LoggerAdapter = new LoggerAdapter(this);
            this.PackageExtractionContext = new PackageExtractionContext(PackageSaveMode.Defaultv3, XmlDocFileSaveMode.None, this.LoggerAdapter, null, null);
        }

        #endregion

        #region "Properties"

        public LoggerAdapter LoggerAdapter { get; }

        public PackageExtractionContext PackageExtractionContext { get; set; }

        public XDocument OriginalPackagesConfig { get; set; }

        public Guid OperationId { get; set; }

        public NuGetActionType ActionType { get; set; }

        public ISourceControlManagerProvider SourceControlManagerProvider
        {
            get
            {
                return null;
            }
        }

        public ExecutionContext ExecutionContext
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region "Methods"

        public void Log(MessageLevel level, string message, params object[] args)
        {
            AdvancedBootloader.ClientPushService?.SendNugetMessage(string.Format(message, args));
        }

        public void ReportError(string message)
        {
            AdvancedBootloader.ClientPushService?.SendNugetMessage(message);
        }

        public FileConflictAction ResolveFileConflict(string message)
        {
            return FileConflictAction.IgnoreAll;
        }

        #endregion
    }
}
