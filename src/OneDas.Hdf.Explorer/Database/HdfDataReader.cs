using HDF.PInvoke;
using OneDas.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.DataManagement.Hdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;

namespace OneDas.Hdf.Explorer.Database
{
    public class HdfDataReader : IDataReader
    {
        #region Fields

        private string _campaignName;
        private long _sourceFileId = -1;

        #endregion

        #region Constructors

        public HdfDataReader(string campaignName, DateTime dateTime)
        {
            _campaignName = campaignName;

            var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, "DB_DATA", folderName);
            _sourceFileId = H5F.open(sourceFilePath, H5F.ACC_RDONLY);
        }

        #endregion

        #region Properties

        public PipeReader DataReader => throw new NotImplementedException();

        public PipeReader StatusReader => throw new NotImplementedException();

        public bool IsDataAvailable
        {
            get
            {
                return H5I.is_valid(_sourceFileId) > 0;
            }
        }

        #endregion

        #region Methods

        public CampaignInfo GetCampaignInfo()
        {
            return GeneralHelper.GetCampaignInfo(_sourceFileId, _campaignName);
        }

        public void Dispose()
        {
            if (H5I.is_valid(_sourceFileId) > 0) { H5F.close(_sourceFileId); }
        }

        public List<int> GetVersions()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
