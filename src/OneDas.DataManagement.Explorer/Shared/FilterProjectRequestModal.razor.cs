using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class FilterProjectRequestModal
    {
        #region Records

        public record ProjectState()
        {
            public string Id { get; set; }
            public bool IsSelected { get; set; }
        }

        #endregion

        #region Properties

        [Inject]
        public UserState UserState { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public Action<List<string>> OnProjectIdsSelected { get; set; }

        [Parameter]
        public List<string> SelectedProjectIds { get; set; }

        private List<ProjectState> ProjectContainerStates { get; set; }

        #endregion

        #region Methods

        protected override void OnParametersSet()
        {
            var accessibleProjects = this.UserState.ProjectContainersInfo.Accessible;
            var accessibleProjectIds = this.UserState.ProjectContainersInfo.Accessible;

            this.ProjectContainerStates = accessibleProjects.Select(projectContainer =>
            {
                var isSelected = this.SelectedProjectIds.Contains(projectContainer.Id);
                return new ProjectState() 
                { 
                    Id = projectContainer.Id,
                    IsSelected = isSelected
                };
            }).ToList();

            base.OnParametersSet();
        }

        private void OK()
        {
            this.OnIsOpenChanged(false);

            var newSelectedProjectIds = this.ProjectContainerStates
                .Where(state => state.IsSelected)
                .Select(state => state.Id).ToList();

            this.OnProjectIdsSelected?.Invoke(newSelectedProjectIds);
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
