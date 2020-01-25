using OneDas.Database;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;

namespace OneDas.DataManagement.Extensibility
{
    public interface IDataReader : IDisposable
    {
        #region Properties

        PipeReader DataReader { get; }
        PipeReader StatusReader { get; }
        bool IsDataAvailable { get; }

        #endregion

        #region Methods

        CampaignInfo GetCampaignInfo();
        List<int> GetVersions();
        (int first, int last) GetCompletedChunkBounds();

        #endregion
    }
}
