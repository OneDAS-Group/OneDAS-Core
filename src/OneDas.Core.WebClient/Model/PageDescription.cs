using System.Collections.Generic;

namespace OneDas.Core.WebClient.Model
{
    public class PageDescription
    {
        #region Constructors

        public PageDescription(string displayName, string iconName, string route) : this(displayName, iconName, route, new List<PageDescription>())
        {
            //
        }

        public PageDescription(string displayName, string iconName, string route, List<PageDescription> pageDescriptionSet)
        {
            this.DisplayName = displayName;
            this.IconName = iconName;
            this.Route = route;

            this.PageDescriptionSet = pageDescriptionSet;
        }

        #endregion

        #region Properties

        public string DisplayName { get; }
        public string IconName { get; }
        public string Route { get; }

        public List<PageDescription> PageDescriptionSet { get; }

        #endregion
    }
}
