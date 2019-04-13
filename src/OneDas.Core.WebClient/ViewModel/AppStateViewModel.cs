using OneDas.Core.WebClient.Model;
using System.Collections.Generic;

namespace OneDas.Core.WebClient.ViewModel
{
    public class AppStateViewModel : BindableBase
    {
        #region Field

        bool _isConnected;

        #endregion

        #region Constructors

        public AppStateViewModel()
        {
            this.IsConnected = true;

            this.PageDescriptionSet = new List<PageDescription>()
            {
                new PageDescription("Home", "", ""),
                new PageDescription("Control", "", "control"),
                new PageDescription("Live View", "", "liveview"),
                new PageDescription("Editor", "", "editor")
            };
        }

        #endregion

        #region Properties

        public bool IsConnected
        {
            get { return _isConnected; }
            set { base.SetProperty(ref _isConnected, value); }
        }

        public List<PageDescription> PageDescriptionSet { get; }

        #endregion
    }
}
