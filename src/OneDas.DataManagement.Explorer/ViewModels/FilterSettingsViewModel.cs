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

        public FilterSettings Model { get; }

        public IReadOnlyList<CodeDefinition> CodeDefintions => this.Model.Codes;

        #endregion

        #region Methods

        public void AddOrUpdateCodeDefinition(CodeDefinitionViewModel description)
        {
            lock (_editLock)
            {
                if (!this.Model.Codes.Contains(description.Model))
                    this.Model.Codes.Add(description.Model);

                this.Model.Save(_filePath);
            }

            this.RaisePropertyChanged(nameof(this.CodeDefintions));
        }

        public void RemoveCodeDefinition(CodeDefinitionViewModel description)
        {
            lock (_editLock)
            {
                this.Model.Codes.Remove(description.Model);
                this.Model.Save(_filePath);
            }

            this.RaisePropertyChanged(nameof(this.CodeDefintions));
        }

        #endregion
    }
}
