using OneDas.DataManagement.Explorer.Core;
using Prism.Mvvm;
using System.Collections.Generic;
using System.IO;

namespace OneDas.DataManagement.Explorer.ViewModels
{
    public class FilterSettingsViewModel : BindableBase
    {
        #region Fields

        private static object _editLock;

        private string _filePath;

        #endregion

        #region Constructors

        public FilterSettingsViewModel(string filePath)
        {
            if (File.Exists(filePath))
                this.Model = FilterSettings.Load(filePath);
            else
                this.Model = new FilterSettings();

            _filePath = filePath;
            _editLock = new object();
        }

        #endregion

        #region Properties

        private FilterSettings Model { get; }

        public IReadOnlyList<FilterDescription> FilterDescriptions => this.Model.FilterDescriptions;

        #endregion

        #region Methods

        public void AddOrUpdateFilterDescription(FilterDescriptionViewModel description)
        {
            lock (_editLock)
            {
                if (!this.Model.FilterDescriptions.Contains(description.Model))
                    this.Model.FilterDescriptions.Add(description.Model);

                this.Model.Save(_filePath);
            }

            this.RaisePropertyChanged(nameof(this.FilterDescriptions));
        }

        public void RemoveFilterDescription(FilterDescriptionViewModel description)
        {
            lock (_editLock)
            {
                this.Model.FilterDescriptions.Remove(description.Model);
                this.Model.Save(_filePath);
            }

            this.RaisePropertyChanged(nameof(this.FilterDescriptions));
        }

        #endregion
    }
}
