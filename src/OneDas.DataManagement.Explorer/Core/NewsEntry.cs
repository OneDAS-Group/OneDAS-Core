using System;

namespace OneDas.DataManagement.Explorer.Core
{
    public class NewsEntry
    {
        #region Constructors

        public NewsEntry(DateTime date, string title, string message)
        {
            this.Date = date;
            this.Title = title;
            this.Message = message;
        }

        private NewsEntry()
        {
            //
        }

        #endregion

        #region Properties

        public DateTime Date { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        #endregion
    }
}
