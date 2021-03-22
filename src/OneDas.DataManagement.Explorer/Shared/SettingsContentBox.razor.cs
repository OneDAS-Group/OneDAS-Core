using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
using OneDas.DataManagement.Explorer.ViewModels;
using System;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class SettingsContentBox : IDisposable
    {
        #region Fields

        private EventHandler _handler;

        #endregion

        #region Properties

        [Inject]
        public SettingsViewModel Settings { get; set; }

        [Inject]
        public OneDasExplorerOptions Options { get; set; }

        [Inject]
        public JobEditor JobEditor { get; set; }

        #endregion

        #region Methods

        protected override void OnParametersSet()
        {
            _handler = (sender, e) => this.InvokeAsync(() => this.StateHasChanged());
            this.JobEditor.Changed += _handler;

            base.OnParametersSet();
        }

        public void SaveOptions()
        {
            this.Options.Save(Program.OptionsFilePath);
        }

        public void Dipose()
        {
            if (_handler != null)
                this.JobEditor.Changed -= _handler;
        }

        #endregion

        #region Commands

        public void UpdateJobEditor()
        {
            this.JobEditor.Update();
            this.InvokeAsync(() => this.StateHasChanged());
        }

        #endregion
    }
}
