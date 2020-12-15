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
        private FilterDescriptionViewModel _filterDescription;
		private DotNetObjectReference<MonacoService> _objRef;

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

        private List<FilterDescriptionViewModel> FilterDescriptions { get; set; }

        private bool FilterGalleryIsOpen { get; set; }

        private bool FilterCreateDialogIsOpen { get; set; }

        private bool FilterDeleteDialogIsOpen { get; set; }

        private FilterDescriptionViewModel FilterDescription
        {
            get
            {
                return _filterDescription;
            }
            set
            {
                if (_filterDescription is not null)
                    _filterDescription.PropertyChanged -= this.OnFilterDescriptionPropertyChanged;

                _filterDescription = value;
                _filterDescription.PropertyChanged += this.OnFilterDescriptionPropertyChanged;

                this.MonacoService.SetValues(_filterDescription.Code, _filterDescription.SampleRate, _filterDescription.RequestedProjectIds);
                _ = this.JS.SetMonacoValueAsync(_editorId, this.FilterDescription.Code);
            }
        }

        #endregion

        #region Methods

        private void UpdateFilterDescriptions()
        {
            var owner = this.UserIdService.User.Identity.Name;

            this.FilterDescriptions = this.AppState
                .FilterSettings
                .FilterDescriptions
                .Where(current => current.Owner == owner)
                .OrderBy(current => current.CodeType)
                .Select(current => new FilterDescriptionViewModel(current))
                .ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _objRef?.Dispose();
                this.MonacoService.DiagnosticsUpdated -= this.OnDiagnosticsUpdated;
            }
        }

        protected override void OnParametersSet()
        {
            this.CreateFilterDescription(CodeType.Channel);
            this.UpdateFilterDescriptions();
            this.MonacoService.DiagnosticsUpdated += this.OnDiagnosticsUpdated;

            base.OnParametersSet();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (this.UserIdService.User.Identity.IsAuthenticated)
                {
                    _objRef = DotNetObjectReference.Create(this.MonacoService);
                    _editorId = "1";

                    var options = new Dictionary<string, object>
                    {
                        ["automaticLayout"] = true,
                        ["language"] = "csharp",
                        ["scrollBeyondLastLine"] = false,
                        ["value"] = this.FilterDescription.Code,
                        ["theme"] = "vs-dark"
                    };

                    await this.JS.CreateMonacoEditorAsync(_editorId, options);
                    await this.JS.RegisterMonacoProvidersAsync(_editorId, _objRef);
                }
            }
        }

        #endregion

        #region EventHandlers

        private void OnFilterDescriptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FilterDescriptionViewModel.SampleRate))
                this.MonacoService.SetValues(null, this.FilterDescription.SampleRate, this.FilterDescription.RequestedProjectIds);

            else if (e.PropertyName == nameof(FilterDescriptionViewModel.RequestedProjectIds))
                this.MonacoService.SetValues(null, this.FilterDescription.SampleRate, this.FilterDescription.RequestedProjectIds);
        }

        private void OnDiagnosticsUpdated(object sender, List<Diagnostic> diagnostics)
        {
            this.Diagnostics = diagnostics;
            this.InvokeAsync(() => { this.StateHasChanged(); });

            _ = this.JS.SetMonacoDiagnosticsAsync(_editorId, diagnostics);
        }

        #endregion

        #region Commands

        private void CopyFilterDescription(FilterDescriptionViewModel filterDescription)
        {
            var owner = this.UserIdService.User.Identity.Name;

            this.FilterDescription = new FilterDescriptionViewModel(filterDescription.Model with
            {
                IsPublic = false,
                Group = owner.Split('@').First(),
                Owner = owner
            });
        }

        private void CreateFilterDescription(CodeType codeType, bool selectProjects = false)
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

            this.FilterDescription = new FilterDescriptionViewModel(new FilterDescription(owner: owner))
            {
                CodeType = codeType,
                Code = code,
                Name = name,
                Group = owner.Split('@').First(),
                Unit = "-",
                SampleRate = "1 s"
            };

            if (selectProjects && codeType == CodeType.Channel)
                this.FilterDescriptionSettingsBox.OpenFilterProjectRequestDialog();
        }

        private void DeleteFilterDescription()
        {
            this.AppState.FilterSettings.RemoveFilterDescription(this.FilterDescription);
            this.CreateFilterDescription(CodeType.Channel);
            this.UpdateFilterDescriptions();
            this.ToasterService.ShowSuccess(message: "The filter has been deleted.", icon: MatIconNames.Delete);

            this.InvokeAsync(() => this.StateHasChanged());
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

        private async Task AddFilterDescription()
        {
            this.FilterDescription.Code = await this.JS.GetMonacoValueAsync(_editorId);
            this.AppState.FilterSettings.AddOrUpdateFilterDescription(this.FilterDescription);
            this.UpdateFilterDescriptions();
            this.ToasterService.ShowSuccess(message: "The filter has been saved.", icon: MatIconNames.Thumb_up);

            _ = this.InvokeAsync(() => this.StateHasChanged());
        }

        #endregion
    }
}
