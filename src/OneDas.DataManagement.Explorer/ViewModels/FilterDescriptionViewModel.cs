using OneDas.DataManagement.Explorer.Core;
using Prism.Mvvm;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneDas.DataManagement.Explorer.ViewModels
{
    public class FilterDescriptionViewModel : BindableBase /* Must be a class to ensure reference equality! Otherwise there would be created a new copy with every modification. */
    {
        #region Constructors

        public FilterDescriptionViewModel(FilterDescription model)
        {
            this.Model = model;
        }

        #endregion

        #region Properties

        public FilterDescription Model { get; }

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

        public string Id 
        {
            get { return this.Model.Id; } 
            set { this.Model.Id = value; }
        }

        public bool IsPublic
        {
            get { return this.Model.IsPublic; }
            set { this.Model.IsPublic = value; }
        }

        [IsValidName]
        public string Name
        {
            get { return this.Model.Name; }
            set { this.Model.Name = value; }
        }

        public string Group
        {
            get { return this.Model.Group; }
            set { this.Model.Group = value; }
        }

        public string Unit
        {
            get { return this.Model.Unit; }
            set { this.Model.Unit = value; }
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
