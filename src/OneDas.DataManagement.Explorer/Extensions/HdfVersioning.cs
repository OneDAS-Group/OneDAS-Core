using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Extensions
{
    public class HdfVersioning
    {
        #region Constructors

        public HdfVersioning()
        {
            this.ScannedUntilMap = new Dictionary<string, DateTime>();
        }

        #endregion

        #region Properties

        public Dictionary<string, DateTime> ScannedUntilMap { get; set; }

        #endregion

        #region Methods

        public static HdfVersioning Load(string filePath)
        {
            return JsonSerializerHelper.Deserialize<HdfVersioning>(filePath);
        }

        #endregion
    }
}
