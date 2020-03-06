using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.BlazorExplorer.ViewModels;
using System.IO;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
    public partial class Attachments
    {
        #region Properties

        [Inject]
        public AppStateViewModel AppState { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        #endregion

        #region Methods

        public string GetIcon(string filePath)
        {
            var extension = Path.GetExtension(filePath);

            return extension switch
            {
                ".docx" => "file-word",
                ".xlsx" => "file-excel",
                ".pptx" => "file-powerpoint",
                ".pdf" => "file-pdf",
                ".jpg" => "file-image",
                ".jpeg" => "file-image",
                ".png" => "file-image",
                ".tiff" => "file-image",
                _ => "file"
            };
        }

        public string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public string GetHref(string filePath)
        {
            return $"/attachments/{this.AppState.CampaignContainer.PhysicalName}/{this.GetFileName(filePath)}";
        }

        private void OnIsOpenChanged(bool value)
        {
            this.IsOpen = value;
            this.IsOpenChanged.InvokeAsync(value);
        }

        #endregion
    }
}
