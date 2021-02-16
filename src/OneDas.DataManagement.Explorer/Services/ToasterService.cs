using MatBlazor;

namespace OneDas.DataManagement.Explorer.Services
{
    public class ToasterService
    {
        #region Fields

        private IMatToaster _toaster;

        #endregion

        #region Constructors

        public ToasterService(IMatToaster toaster)
        {
            _toaster = toaster;
        }

        #endregion

        #region Methods

        public void ShowSuccess(string message, string icon)
        {
            _toaster.Add(
                message,
                MatToastType.Primary,
                "Success!",
                icon: icon);
        }

        public void ShowWarning(string message, string icon)
        {
            _toaster.Add(
                message,
                MatToastType.Warning,
                "Warning!",
                icon: icon);
        }

        public void ShowError(string message, string icon)
        {
            _toaster.Add(
                message,
                MatToastType.Danger,
                "An error occured!",
                icon: icon);
        }

        #endregion
    }
}
