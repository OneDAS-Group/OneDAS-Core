using OneDas.DataManagement.Explorer.Core;
using Prism.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public FilterSettings Model { get; }

        public IReadOnlyList<FilterDescription> Filters => this.Model.Filters;

        #endregion

        #region Methods

        public void AddOrUpdateFilterDescription(FilterDescriptionViewModel description)
        {
            lock (_editLock)
            {
                if (!this.Model.Filters.Contains(description.Model))
                    this.Model.Filters.Add(description.Model);

                this.Model.Save(_filePath);
            }

            this.RaisePropertyChanged(nameof(this.Filters));
        }

        public void RemoveFilter(FilterDescriptionViewModel description)
        {
            lock (_editLock)
            {
                this.Model.Filters.Remove(description.Model);
                this.Model.Save(_filePath);
            }

            this.RaisePropertyChanged(nameof(this.Filters));
        }

        #endregion
    }
}
