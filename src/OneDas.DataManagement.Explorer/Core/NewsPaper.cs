using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OneDas.DataManagement.Explorer.Core
{
    public class NewsPaper
    {
        #region Constructors

        public NewsPaper(List<NewsEntry> news)
        {
            this.News = news;
        }

        public NewsPaper()
        {
            this.News = new List<NewsEntry>();
        }

        #endregion

        #region Properties

        public List<NewsEntry> News { get; set; }

        #endregion

        #region Methods

        public static NewsPaper Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                var jsonString = File.ReadAllText(filePath);
                var newsPaper = JsonSerializer.Deserialize<NewsPaper>(jsonString);

                return newsPaper;
            }
            else
            {
                var newsPaper = new NewsPaper(new List<NewsEntry>() { new NewsEntry(DateTime.UtcNow, "First News", "News description.") });
                var jsonString = JsonSerializer.Serialize(newsPaper, new JsonSerializerOptions() { WriteIndented = true });
                File.WriteAllText(filePath, jsonString);

                return newsPaper;
            }
        }

        #endregion
    }
}
