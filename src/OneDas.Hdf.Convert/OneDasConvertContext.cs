using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OneDas.Hdf.Convert
{
    public class OneDasConvertContext
    {
        #region Fields

        private string _filePath;

        #endregion

        #region Constructors

        private OneDasConvertContext()
        {
            //
        }

        private OneDasConvertContext(string filePath)
        {
            _filePath = filePath;

            this.ProcessedPeriods = new List<DateTime>();
        }

        #endregion

        #region Properties

        public List<DateTime> ProcessedPeriods { get; set; }

        #endregion

        #region Methods

        public static OneDasConvertContext OpenOrCreate(string importDirectoryPath)
        {
            var filePath = Path.Combine(importDirectoryPath, "metadata.json");

            OneDasConvertContext convertContext;

            if (File.Exists(filePath))
            {
                convertContext = JsonSerializer.Deserialize<OneDasConvertContext>(File.ReadAllText(filePath));
                convertContext._filePath = filePath;
            }
            else
            {
                convertContext = new OneDasConvertContext(filePath);
            }

            return convertContext;
        }

        public void Save()
        {
            this.ProcessedPeriods.Sort();

            // save file
            var options = new JsonSerializerOptions { WriteIndented = true };
            var writeJsonString = JsonSerializer.Serialize(this, options);

            File.WriteAllText(_filePath, writeJsonString);
        }

        #endregion
    }
}
