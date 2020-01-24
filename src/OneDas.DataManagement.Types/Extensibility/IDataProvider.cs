using OneDas.Database;
using System;
using System.IO.Pipelines;

namespace OneDas.DataManagement.Extensibility
{
    public interface IDataProvider : IDisposable
    {
        #region Properties

        PipeReader DataReader { get; }
        PipeReader StatusReader { get; }

        #endregion

        #region Methods

        CampaignInfo GetCampaignInfo();

        #endregion
    }
}
