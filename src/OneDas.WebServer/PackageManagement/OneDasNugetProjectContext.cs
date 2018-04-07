using NuGet.Packaging;
using NuGet.ProjectManagement;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace OneDas.WebServer.PackageManagement
{
    public class EmptyNuGetProjectContext2 : INuGetProjectContext
    {
        public PackageExtractionContext PackageExtractionContext { get; set; }

        public XDocument OriginalPackagesConfig { get; set; }

        public NuGetActionType ActionType { get; set; }

        public Guid OperationId { get; set; }

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

        public void Log(MessageLevel level, string message, params object[] args)
        {
            Debug.WriteLine(string.Format(message, args));
        }

        public FileConflictAction ResolveFileConflict(string message)
        {
            return FileConflictAction.IgnoreAll;
        }

        public void ReportError(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
