using OneDas.DataManagement;
using System;

namespace IwesOneDas
{
    public class HdfVersioning
    {
        #region Constructors

        public HdfVersioning()
        {
            //
        }

        #endregion

        #region Properties

        public DateTime ScannedUntil { get; set; }

        #endregion

        #region Methods

        public static HdfVersioning Load(string filePath)
        {
            return JsonSerializerHelper.Deserialize<HdfVersioning>(filePath);
        }

        #endregion
    }
}
