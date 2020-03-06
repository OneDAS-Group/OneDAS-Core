using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OneDas.DataManagement.BlazorExplorer.Core
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

        public static NewsPaper Load()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "news.json");

            if (File.Exists(filePath))
            {
                var jsonString = File.ReadAllText(filePath);
                var newsPaper = JsonSerializer.Deserialize<NewsPaper>(jsonString);

                return newsPaper;
            }
            else
            {
                var newsPaper = new NewsPaper(new List<NewsEntry>() { new NewsEntry(DateTime.UtcNow, "Our Changing Planet", "First news.") });
                var jsonString = JsonSerializer.Serialize(newsPaper, new JsonSerializerOptions() { WriteIndented = true });
                File.WriteAllText(filePath, jsonString);

                return newsPaper;
            }
        }

        #endregion
    }
}
