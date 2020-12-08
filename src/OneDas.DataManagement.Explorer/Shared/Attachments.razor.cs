using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.ViewModels;
using System.IO;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class Attachments
    {
        #region Properties

        [Inject]
        public UserStateViewModel UserState { get; set; }

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
            return $"/attachments/{this.UserState.ProjectContainer.PhysicalName}/{this.GetFileName(filePath)}";
        }

        private void OnIsOpenChanged(bool value)
        {
            this.IsOpen = value;
            this.IsOpenChanged.InvokeAsync(value);
        }

        #endregion
    }
}
