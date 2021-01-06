using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Roslyn;
using OneDas.DataManagement.Explorer.Services;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static OneDas.DataManagement.Explorer.Services.MonacoService;

namespace OneDas.DataManagement.Explorer.Pages
{
    public partial class FilterEditor : IDisposable
    {
        #region Fields

        private string _editorId;
        private CodeDefinitionViewModel _filter;
        private DotNetObjectReference<FilterEditor> _filterEditorRef;
        private DotNetObjectReference<MonacoService> _monacoServiceRef;

        #endregion

        #region Properties

        public List<Diagnostic> Diagnostics { get; set; }

        [Inject]
        private ToasterService ToasterService { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        [Inject]
        private MonacoService MonacoService { get; set; }

        [Inject]
        private UserIdService UserIdService { get; set; }

        private List<CodeDefinitionViewModel> CodeDefinitions { get; set; }

        private bool CodeDefinitionGalleryIsOpen { get; set; }

        private bool CodeDefinitionCreateDialogIsOpen { get; set; }

        private bool CodeDefinitionDeleteDialogIsOpen { get; set; }

        private CodeDefinitionViewModel CodeDefinition
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                _ = this.OnFilterChangedAsync(value);
            }
        }

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _filterEditorRef?.Dispose();
                _monacoServiceRef?.Dispose();
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            // create filter here so rendering works fine
            _filter = this.CreateCodeDefinition(CodeType.Filter);

            // update filter list
            this.UpdateFilters();

            await base.OnParametersSetAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (this.UserIdService.User.Identity.IsAuthenticated)
                {
                    _filterEditorRef = DotNetObjectReference.Create(this);
                    _monacoServiceRef = DotNetObjectReference.Create(this.MonacoService);
                    _editorId = "1";

                    var options = new Dictionary<string, object>
                    {
                        ["automaticLayout"] = true,
                        ["language"] = "csharp",
                        ["scrollBeyondLastLine"] = false,
                        ["value"] = this.CodeDefinition.Code,
                        ["theme"] = "vs-dark"
                    };

                    // create monaco editor
                    await this.JS.CreateMonacoEditorAsync(_editorId, options);

                    // update monaco and roslyn
                    await this.OnFilterChangedAsync(this.CodeDefinition);

                    // register monaco providers after roslyn projects are created!
                    await this.JS.RegisterMonacoProvidersAsync(_editorId, _filterEditorRef, _monacoServiceRef);
                }
            }
        }

        private CodeDefinitionViewModel CreateCodeDefinition(CodeType codeType)
        {
            var name = codeType switch
            {
                CodeType.Filter => "NewFilterCode",
                CodeType.Shared => "NewSharedCode",
                _ => throw new Exception($"The code type '{codeType}' is not supported.")
            };

            var code = codeType switch
            {
                CodeType.Filter => RoslynProject.DefaultFilterCode,
                CodeType.Shared => RoslynProject.DefaultSharedCode,
                _ => throw new Exception($"The code type '{codeType}' is not supported.")
            };

            var owner = this.UserIdService.User.Identity.Name;

            return new CodeDefinitionViewModel(new CodeDefinition(owner: owner))
            {
                CodeType = codeType,
                Code = code,
                Name = name,
                SampleRate = "1 s"
            };
        }

        private async Task CreateProjectAsync()
        {
            if (this.CodeDefinition.CodeType != CodeType.Shared)
            {
                var shareCodeFiles = this.AppState.FilterSettings.Model.GetSharedFiles(this.UserIdService.User.Identity.Name)
                    .Select(codeDefinition => codeDefinition.Code)
                    .ToList();

                await this.MonacoService.CreateProjectForEditorAsync(this.CodeDefinition.Model, shareCodeFiles);
            }
            else
            {
                await this.MonacoService.CreateProjectForEditorAsync(this.CodeDefinition.Model, new List<string>());
            }            
        }

        private void UpdateFilters()
        {
            var owner = this.UserIdService.User.Identity.Name;

            this.CodeDefinitions = this.AppState
                .FilterSettings
                .CodeDefintions
                .Where(current => current.Owner == owner)
                .OrderBy(current => current.CodeType)
                .Select(current => new CodeDefinitionViewModel(current))
                .ToList();
        }

        #endregion

        #region EventHandlers

        private async Task OnFilterChangedAsync(CodeDefinitionViewModel filter)
        {
            // attach to events
            if (_filter is not null)
                _filter.PropertyChanged -= this.OnFilterPropertyChanged;

            _filter = filter;
            _filter.PropertyChanged += this.OnFilterPropertyChanged;

            // update monaco and roslyn
            await this.UpdateMonacoAndRoslyn(filter);
        }

        private async Task UpdateMonacoAndRoslyn(CodeDefinitionViewModel filter)
        {
            // create roslyn project
            await this.CreateProjectAsync();

            // get diagnostics
            this.Diagnostics = await this.MonacoService.GetDiagnosticsAsync();

            // set monaco value
            await this.JS.SetMonacoValueAsync(_editorId, filter.Code);

            // set monaco diagnostics
            await this.JS.SetMonacoDiagnosticsAsync(_editorId, this.Diagnostics);
        }

        private void OnFilterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CodeDefinitionViewModel.SampleRate))
            {
                this.InvokeAsync(async () =>
                {
                    await this.UpdateMonacoAndRoslyn(this.CodeDefinition);
                    this.StateHasChanged();
                });
            }

            else if (e.PropertyName == nameof(CodeDefinitionViewModel.RequestedProjectIds))
            {
                this.InvokeAsync(async () =>
                {
                    await this.UpdateMonacoAndRoslyn(this.CodeDefinition);
                    this.StateHasChanged();
                });
            }
        }

        #endregion

        #region Commands

        [JSInvokable]
        public async Task UpdateDiagnosticsAsync(string code)
        {
            this.Diagnostics = await this.MonacoService.GetDiagnosticsAsync(code);

            // set monaco diagnostics
            await this.JS.SetMonacoDiagnosticsAsync(_editorId, this.Diagnostics);

            // trigger a rerender
            await this.InvokeAsync(() => this.StateHasChanged());
        }

        private async Task CopyCodeDefinitionAsync(CodeDefinitionViewModel filter)
        {
            var owner = this.UserIdService.User.Identity.Name;

            this.CodeDefinition = new CodeDefinitionViewModel(filter.Model with
            {
                IsPublic = false,
                Owner = owner
            });
        }

        private void PrepareCodeDefinition(CodeType codeType)
        {
            this.CodeDefinition = this.CreateCodeDefinition(codeType);
            this.CodeDefinitionSettingsBox.OpenCodeDefinitionProjectRequestDialog();
        }

        private async Task SaveFilterAsync()
        {
            // add new filter
            this.CodeDefinition.Code = await this.JS.GetMonacoValueAsync(_editorId); // improvement: set filter code on every key stroke
            this.AppState.FilterSettings.AddOrUpdateCodeDefinition(this.CodeDefinition);

            // update filter list
            this.UpdateFilters();

            // notify
            this.ToasterService.ShowSuccess(message: "The code definition has been saved.", icon: MatIconNames.Thumb_up);
            await this.InvokeAsync(() => this.StateHasChanged());

            // update database
            await this.AppState.UpdateDatabaseAsync();
        }

        private async Task DeleteFilterAsync()
        {
            // remove old code definition
            this.AppState.FilterSettings.RemoveCodeDefinition(this.CodeDefinition);

            // create new code definition
            this.CodeDefinition = this.CreateCodeDefinition(CodeType.Filter);

            // update code definitions
            this.UpdateFilters();

            // notify
            this.ToasterService.ShowSuccess(message: "The code definition has been deleted.", icon: MatIconNames.Delete);
            await this.InvokeAsync(() => this.StateHasChanged());

            // update database
            await this.AppState.UpdateDatabaseAsync();
        }

        private void OpenGalleryDialog()
        {
            this.CodeDefinitionGalleryIsOpen = true;
        }

        private void OpenFilterCreateDialog()
        {
            this.CodeDefinitionCreateDialogIsOpen = true;
        }

        private void OpenFilterDeleteDialog()
        {
            this.CodeDefinitionDeleteDialogIsOpen = true;
        }

        #endregion
    }
}
