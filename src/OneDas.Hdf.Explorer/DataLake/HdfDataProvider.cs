using HDF.PInvoke;
using OneDas.Database;
using OneDas.DataManagement.Extensibility;
using System;
using System.IO;
using System.IO.Pipelines;

namespace OneDas.Hdf.Explorer.DataLake
{
    public class HdfDataProvider : IDataProvider
    {
        #region Fields

        private long _sourceFileId = -1;

        #endregion

        #region Constructors

        public HdfDataProvider(string campaignName, DateTime dateTime)
        {
            var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, "DB_NATIVE", folderName);

            _sourceFileId = H5F.open(sourceFilePath, H5F.ACC_RDONLY);
        }

        #endregion

        #region Properties

        public PipeReader Reader => throw new NotImplementedException();

        #endregion

        #region Methods

        public CampaignInfo GetCampaignInfo()
        {
            GeneralHelper.GetCampaignInfo(sourceFileId, campaignName);
        }

        public void Dispose()
        {
            if (H5I.is_valid(_sourceFileId) > 0) { H5F.close(_sourceFileId); }
        }

        #endregion
    }
}
