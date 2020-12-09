using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using OmniSharp.Models;
using OmniSharp.Models.SignatureHelp;
using OmniSharp.Models.v1.Completion;
using OmniSharp.Options;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Omnisharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Services
{
    public class MonacoService
    {
        #region "Events"

        public event EventHandler<List<Diagnostic>> DiagnosticsUpdated;

        #endregion

        #region Fields

        private RoslynProject _completionProject;
        private RoslynProject _diagnosticProject;
        private OmniSharpCompletionService _completionService;
        private OmniSharpSignatureHelpService _signatureService;
        private OmniSharpQuickInfoProvider _quickInfoProvider;

        #endregion

        #region Records

        public record Diagnostic()
        {
            public LinePosition Start { get; init; }
            public LinePosition End { get; init; }
            public string Message { get; init; }
            public int Severity { get; init; }
        }

        #endregion

        #region Constructors

        public MonacoService(OneDasDatabaseManager databaseManager)
        {
            this.DefaultChannelCode =
$@"using System; 
                 
namespace {nameof(OneDas)}.{nameof(DataManagement)}.{nameof(Explorer)}
{{
    class FilterChannel
    {{
        public void Filter(DateTime begin, DateTime end, CodeGenerationDatabase database, double[] result)
        {{
            
        }}
    }}
}};
";

            this.DefaultProjectCode =
$@"using System; 
                 
namespace {nameof(OneDas)}.{nameof(DataManagement)}.{nameof(Explorer)}
{{
    class FilterProject
    {{
        public void AppliesTo()
        {{
            
        }}

        public void Filter(DateTime begin, DateTime end, string channel, double[] result)
        {{
            
        }}
    }}
}};
";

            this.DefaultSharedCode =
$@"using System; 
                 
namespace {nameof(OneDas)}.{nameof(DataManagement)}.{nameof(Explorer)}
{{
    static class Shared
    {{
        public static void MySharedFunction(string myParameter1)
        {{
            
        }}
    }}
}};
";

            _completionProject = new RoslynProject(databaseManager, this.DefaultChannelCode);
            _diagnosticProject = new RoslynProject(databaseManager, this.DefaultChannelCode);

            var loggerFactory = LoggerFactory.Create(configure => { });
            var formattingOptions = new FormattingOptions();

            _completionService = new OmniSharpCompletionService(_completionProject.Workspace, formattingOptions, loggerFactory);
            _signatureService = new OmniSharpSignatureHelpService(_completionProject.Workspace);
            _quickInfoProvider = new OmniSharpQuickInfoProvider(_completionProject.Workspace, formattingOptions, loggerFactory);
        }

        #endregion

        #region Properties

        public string DefaultChannelCode { get; init; }

        public string DefaultProjectCode { get; init; }

        public string DefaultSharedCode { get; init; }

        #endregion

        #region Methods

        [JSInvokable]
        public async Task<CompletionResponse> GetCompletionAsync(string code, CompletionRequest completionRequest)
        {
            Solution updatedSolution;

            do
            {
                updatedSolution = _completionProject.Workspace.CurrentSolution.WithDocumentText(_completionProject.DocumentId, SourceText.From(code));
            } while (!_completionProject.Workspace.TryApplyChanges(updatedSolution));

            var document = updatedSolution.GetDocument(_completionProject.DocumentId);
            var completionResponse = await _completionService.Handle(completionRequest, document);

            return completionResponse;
        }

        [JSInvokable]
        public async Task<CompletionResolveResponse> GetCompletionResolveAsync(CompletionResolveRequest completionResolveRequest)
        {
            var document = _completionProject.Workspace.CurrentSolution.GetDocument(_completionProject.DocumentId);
            var completionResponse = await _completionService.Handle(completionResolveRequest, document);

            return completionResponse;
        }

        [JSInvokable]
        public async Task<SignatureHelpResponse> GetSignatureHelpAsync(string code, SignatureHelpRequest signatureHelpRequest)
        {
            Solution updatedSolution;

            do
            {
                updatedSolution = _completionProject.Workspace.CurrentSolution.WithDocumentText(_completionProject.DocumentId, SourceText.From(code));
            } while (!_completionProject.Workspace.TryApplyChanges(updatedSolution));

            var document = updatedSolution.GetDocument(_completionProject.DocumentId);
            var signatureHelpResponse = await _signatureService.Handle(signatureHelpRequest, document);

            return signatureHelpResponse;
        }

        [JSInvokable]
        public async Task<QuickInfoResponse> GetQuickInfoAsync(QuickInfoRequest quickInfoRequest)
        {
            var document = _diagnosticProject.Workspace.CurrentSolution.GetDocument(_diagnosticProject.DocumentId);
            var quickInfoResponse = await _quickInfoProvider.Handle(quickInfoRequest, document);

            return quickInfoResponse;
        }

        [JSInvokable]
        public async Task UpdateDiagnosticsAsync(string code = null)
        {
            Solution updatedSolution = _diagnosticProject.Workspace.CurrentSolution;

            if (code is not null)
            {
                do
                {
                    updatedSolution = _diagnosticProject.Workspace.CurrentSolution.WithDocumentText(_diagnosticProject.DocumentId, SourceText.From(code));
                } while (!_diagnosticProject.Workspace.TryApplyChanges(updatedSolution));
            }          

            var compilation = await updatedSolution.Projects.First().GetCompilationAsync();
            var dotnetDiagnostics = compilation.GetDiagnostics();

            var diagnostics = dotnetDiagnostics.Select(current =>
            {
                var lineSpan = current.Location.GetLineSpan();

                return new Diagnostic()
                {
                    Start = lineSpan.StartLinePosition,
                    End = lineSpan.EndLinePosition,
                    Message = current.GetMessage(),
                    Severity = this.GetSeverity(current.Severity)
                };
            }).ToList();

            this.OnDiagnosticsUpdated(diagnostics);
        }

        public void SetValues(string code, string sampleRate)
        {
            _completionProject.SetValues(code, sampleRate);
            _diagnosticProject.SetValues(code, sampleRate);

            _ = this.UpdateDiagnosticsAsync();
        }

        private void OnDiagnosticsUpdated(List<Diagnostic> diagnostics)
        {
            this.DiagnosticsUpdated?.Invoke(this, diagnostics);
        }

        private int GetSeverity(DiagnosticSeverity severity)
        {
            return severity switch
            {
                DiagnosticSeverity.Hidden => 1,
                DiagnosticSeverity.Info => 2,
                DiagnosticSeverity.Warning => 4,
                DiagnosticSeverity.Error => 8,
                _ => throw new Exception("Unknown diagnostic severity.")
            };
        }

        #endregion
    }
}