using System;
using System.IO;
using System.Text.Json;

namespace IwesOneDas
{
    public class HdfConfig
    {
        #region Constructors

        public HdfConfig()
        {
            //
        }

        #endregion

        #region Properties

        public DateTime LastScan { get; set; }

        #endregion

        #region Methods

        public static HdfConfig Load(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<HdfConfig>(jsonString);
        }

        #endregion
    }
}
