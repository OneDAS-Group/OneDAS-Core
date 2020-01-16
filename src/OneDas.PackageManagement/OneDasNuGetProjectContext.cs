using System;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Signing;
using NuGet.ProjectManagement;

namespace OneDas.PackageManagement
{
    public class OneDasNuGetProjectContext : INuGetProjectContext
    {
        #region "Fields"

        ILogger _logger;

        #endregion

        #region "Constructors"

        public OneDasNuGetProjectContext(ISettings settings, ILogger logger)
        {
            _logger = logger;

            this.LoggerAdapter = new LoggerAdapter(this);

            var clientPolicyContext = ClientPolicyContext.GetClientPolicy(settings, this.LoggerAdapter);
            this.PackageExtractionContext = new PackageExtractionContext(PackageSaveMode.Defaultv3, XmlDocFileSaveMode.None, clientPolicyContext, this.LoggerAdapter);
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
            switch (level)
            {
                case MessageLevel.Info:

                    _logger.LogInformation(message, args);
                    break;

                case MessageLevel.Warning:

                    _logger.LogWarning(message, args);
                    break;

                case MessageLevel.Debug:

                    _logger.LogDebug(message, args);
                    break;

                case MessageLevel.Error:

                    _logger.LogError(message, args);
                    break;

                default:

                    throw new ArgumentException();
            }
        }

        public void ReportError(string message)
        {
            _logger.LogError(message);
        }

        public FileConflictAction ResolveFileConflict(string message)
        {
            return FileConflictAction.IgnoreAll;
        }

        #endregion
    }
}
