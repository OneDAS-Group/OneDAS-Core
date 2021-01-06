using OneDas.DataManagement.Explorer.Core;
using Prism.Mvvm;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneDas.DataManagement.Explorer.ViewModels
{
    public class CodeDefinitionViewModel : BindableBase /* Must be a class to ensure reference equality! Otherwise there would be created a new copy with every modification. */
    {
        #region Constructors

        public CodeDefinitionViewModel(CodeDefinition model)
        {
            this.Model = model;
        }

        #endregion

        #region Properties

        public CodeDefinition Model { get; }

        public string Owner
        {
            get { return this.Model.Owner; }
            set { this.Model.Owner = value; }
        }

        public CodeType CodeType
        {
            get
            {
                return this.Model.CodeType;
            }
            set
            {
                this.Model.CodeType = value;
                this.RaisePropertyChanged();
            }
        }

        public CodeLanguage CodeLanguage
        {
            get { return this.Model.CodeLanguage; }
            set { this.Model.CodeLanguage = value; }
        }

        public string Code
        {
            get { return this.Model.Code; }
            set { this.Model.Code = value; }
        }

        public bool IsPublic
        {
            get { return this.Model.IsPublic; }
            set { this.Model.IsPublic = value; }
        }

        public string Name
        {
            get { return this.Model.Name; }
            set { this.Model.Name = value; }
        }

        [Required]
        public string SampleRate
        {
            get 
            {
                return this.Model.SampleRate; 
            }
            set 
            {
                this.Model.SampleRate = value;
                this.RaisePropertyChanged();
            }
        }

        public List<string> RequestedProjectIds
        {
            get
            {
                return this.Model.RequestedProjectIds;
            }
            set
            {
                this.Model.RequestedProjectIds = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion
    }
}
