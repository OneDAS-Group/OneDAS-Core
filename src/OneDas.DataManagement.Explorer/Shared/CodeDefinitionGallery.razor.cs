using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class CodeDefinitionGallery
    {
        #region Properties

        [Inject]
        public AppState AppState { get; set; }

        [Inject]
        public UserManager<IdentityUser> UserManager { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public Action<CodeDefinitionViewModel> OnCodeDefinitionSelected { get; set; }

        private Dictionary<string, List<CodeDefinitionViewModel>> OwnerToCodeDefinitionsMap { get; set; }

        #endregion

        #region Methods



        protected override async Task OnParametersSetAsync()
        {
            var owners = this.AppState.FilterSettings.CodeDefinitions
                   .Where(current => current.IsEnabled)
                   .Select(current => current.Owner)
                   .Distinct()
                   .ToList();

            this.OwnerToCodeDefinitionsMap = new Dictionary<string, List<CodeDefinitionViewModel>>();

            foreach (var owner in owners)
            {
                this.OwnerToCodeDefinitionsMap[owner] = await this.GetCodeDefinitionsForOwnerAsync(owner);
            }

            await base.OnParametersSetAsync();
        }

        private async Task<List<CodeDefinitionViewModel>> GetCodeDefinitionsForOwnerAsync(string owner)
        {
            var user = await Utilities.GetClaimsPrincipalAsync(owner, UserManager);

            return this.AppState
                .FilterSettings
                .CodeDefinitions
                .Where(current => user.HasClaim(Claims.IS_ADMIN, "true") || (current.IsEnabled && current.Owner == owner))
                .OrderBy(current => current.CodeType)
                .Select(current => new CodeDefinitionViewModel(current))
                .ToList();
        }

        private void Accept(CodeDefinitionViewModel codeDefinition)
        {
            this.OnIsOpenChanged(false);
            this.OnCodeDefinitionSelected?.Invoke(codeDefinition);
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
