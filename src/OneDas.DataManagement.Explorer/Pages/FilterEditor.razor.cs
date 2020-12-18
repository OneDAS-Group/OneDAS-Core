using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.Core;
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
        private FilterDescriptionViewModel _filter;
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

        private List<FilterDescriptionViewModel> Filters { get; set; }

        private bool FilterGalleryIsOpen { get; set; }

        private bool FilterCreateDialogIsOpen { get; set; }

        private bool FilterDeleteDialogIsOpen { get; set; }

        private FilterDescriptionViewModel Filter
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
            _filter = this.CreateFilter(CodeType.Channel);

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
                        ["value"] = this.Filter.Code,
                        ["theme"] = "vs-dark"
                    };

                    // create monaco editor
                    await this.JS.CreateMonacoEditorAsync(_editorId, options);

                    // update monaco and roslyn
                    await this.OnFilterChangedAsync(this.Filter);

                    // register monaco providers after roslyn projects are created!
                    await this.JS.RegisterMonacoProvidersAsync(_editorId, _filterEditorRef, _monacoServiceRef);
                }
            }
        }

        private FilterDescriptionViewModel CreateFilter(CodeType codeType)
        {
            var name = codeType switch
            {
                CodeType.Channel => "NewChannelFilter",
                CodeType.Project => "NewProjectFilter",
                CodeType.Shared => "NewSharedCode",
                _ => throw new Exception($"The code type '{codeType}' is not supported.")
            };

            var code = codeType switch
            {
                CodeType.Channel => this.MonacoService.DefaultChannelCode,
                CodeType.Project => this.MonacoService.DefaultProjectCode,
                CodeType.Shared => this.MonacoService.DefaultSharedCode,
                _ => throw new Exception($"The code type '{codeType}' is not supported.")
            };

            var owner = this.UserIdService.User.Identity.Name;

            return new FilterDescriptionViewModel(new FilterDescription(owner: owner))
            {
                CodeType = codeType,
                Code = code,
                Name = name,
                Group = owner.Split('@').First(),
                Unit = "-",
                SampleRate = "1 s"
            };
        }

        private async Task CreateProjectAsync()
        {
            if (this.Filter.CodeType != CodeType.Shared)
            {
                var shareCodeFiles = this.AppState.FilterSettings.Model.GetSharedFiles(this.UserIdService.User.Identity.Name)
                    .Select(filterDescription => filterDescription.Code)
                    .ToList();

                await this.MonacoService.CreateProjectForEditorAsync(this.Filter.Model, shareCodeFiles);
            }
            else
            {
                await this.MonacoService.CreateProjectForEditorAsync(this.Filter.Model, new List<string>());
            }            
        }

        private void UpdateFilters()
        {
            var owner = this.UserIdService.User.Identity.Name;

            this.Filters = this.AppState
                .FilterSettings
                .Filters
                .Where(current => current.Owner == owner)
                .OrderBy(current => current.CodeType)
                .Select(current => new FilterDescriptionViewModel(current))
                .ToList();
        }

        #endregion

        #region EventHandlers

        private async Task OnFilterChangedAsync(FilterDescriptionViewModel filter)
        {
            // attach to events
            if (_filter is not null)
                _filter.PropertyChanged -= this.OnFilterPropertyChanged;

            _filter = filter;
            _filter.PropertyChanged += this.OnFilterPropertyChanged;

            // update monaco and roslyn
            await this.UpdateMonacoAndRoslyn(filter);
        }

        private async Task UpdateMonacoAndRoslyn(FilterDescriptionViewModel filter)
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
            if (e.PropertyName == nameof(FilterDescriptionViewModel.SampleRate))
            {
                this.InvokeAsync(async () =>
                {
                    await this.UpdateMonacoAndRoslyn(this.Filter);
                    this.StateHasChanged();
                });
            }

            else if (e.PropertyName == nameof(FilterDescriptionViewModel.RequestedProjectIds))
            {
                this.InvokeAsync(async () =>
                {
                    await this.UpdateMonacoAndRoslyn(this.Filter);
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

        private async Task CopyFilterAsync(FilterDescriptionViewModel filter)
        {
            var owner = this.UserIdService.User.Identity.Name;

            this.Filter = new FilterDescriptionViewModel(filter.Model with
            {
                IsPublic = false,
                Group = owner.Split('@').First(),
                Owner = owner
            });
        }

        private void PrepareFilter(CodeType codeType)
        {
            this.Filter = this.CreateFilter(codeType);
            this.FilterDescriptionSettingsBox.OpenFilterProjectRequestDialog();
        }

        private async Task SaveFilterAsync()
        {
            // add new filter
            this.Filter.Code = await this.JS.GetMonacoValueAsync(_editorId); // improvement: set filter code on every key stroke
            this.AppState.FilterSettings.AddOrUpdateFilterDescription(this.Filter);

            // update filter list
            this.UpdateFilters();

            // notify
            this.ToasterService.ShowSuccess(message: "The filter has been saved.", icon: MatIconNames.Thumb_up);
            await this.InvokeAsync(() => this.StateHasChanged());
        }

        private async Task DeleteFilterAsync()
        {
            // remove old filter
            this.AppState.FilterSettings.RemoveFilter(this.Filter);

            // create new filter
            this.Filter = this.CreateFilter(CodeType.Channel);

            // update filters
            this.UpdateFilters();

            // notify
            this.ToasterService.ShowSuccess(message: "The filter has been deleted.", icon: MatIconNames.Delete);
            await this.InvokeAsync(() => this.StateHasChanged());
        }

        private void OpenGalleryDialog()
        {
            this.FilterGalleryIsOpen = true;
        }

        private void OpenFilterCreateDialog()
        {
            this.FilterCreateDialogIsOpen = true;
        }

        private void OpenFilterDeleteDialog()
        {
            this.FilterDeleteDialogIsOpen = true;
        }

        #endregion
    }
}
