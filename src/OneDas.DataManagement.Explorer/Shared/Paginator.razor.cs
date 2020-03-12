using MatBlazor;
using Microsoft.AspNetCore.Components;
using System;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class Paginator
    {
        #region Properties

        [Parameter]
        public int PageSize { get; set; }

        [Parameter]
        public int Length { get; set; }

        [Parameter]
        public int Page { get; set; }

        [Parameter]
        public EventCallback<int> PageChanged { get; set; }

        public int TotalPages { get; private set; }

        #endregion

        #region Methods

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            this.TotalPages = this.CalculateTotalPages(PageSize);
        }

        protected int CalculateTotalPages(int pageSize)
        {
            if (pageSize == 0)
                return int.MaxValue;

            return Math.Max(0, (int)Math.Ceiling((decimal)this.Length / this.PageSize));
        }

        public void NavigateToPage(MatPaginatorAction direction, int pageSize)
        {
            var page = this.Page;

            switch (direction)
            {
                case MatPaginatorAction.Default:
                    break;
                case MatPaginatorAction.First:
                    page = 0;
                    break;
                case MatPaginatorAction.Previous:
                    page--;
                    break;
                case MatPaginatorAction.Next:
                    page++;
                    break;
                case MatPaginatorAction.Last:
                    page = this.TotalPages - 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            if (page < 0)
                page = 0;

            if (page >= this.TotalPages)
                page = this.TotalPages - 1;

            this.Page = page;
            this.PageChanged.InvokeAsync(page);
        }

        #endregion
    }
}