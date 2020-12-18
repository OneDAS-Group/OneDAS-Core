using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class FilterGallery
    {
        #region Properties

        [Inject]
        public AppState AppState { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public Action<FilterDescriptionViewModel> OnFilterDescriptionSelected { get; set; }

        private List<string> Owners { get; set; }

        #endregion

        #region Methods

        protected override void OnAfterRender(bool firstRender)
        {
            this.Owners = this.AppState.FilterSettings.Filters
                .Where(current => current.IsPublic)
                .Select(current => current.Owner)
                .Distinct()
                .ToList();

            base.OnAfterRender(firstRender);
        }

        private List<FilterDescriptionViewModel> GetFilterDescriptionsForOwner(string owner)
        {
            return this.AppState
                .FilterSettings
                .Filters
                .Where(current => current.IsPublic && current.Owner == owner)
                .OrderBy(current => current.CodeType)
                .Select(current => new FilterDescriptionViewModel(current))
                .ToList();
        }

        private void Accept(FilterDescriptionViewModel filterDescription)
        {
            this.OnIsOpenChanged(false);
            this.OnFilterDescriptionSelected?.Invoke(filterDescription);
        }

        private void Cancel()
        {
            this.OnIsOpenChanged(false);
        }

        private void OnIsOpenChanged(bool value)
        {
            this.IsOpen = value;
            this.IsOpenChanged.InvokeAsync(value);
        }

        #endregion
    }
}
