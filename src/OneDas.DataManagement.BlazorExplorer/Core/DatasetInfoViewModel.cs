using OneDas.DataManagement.Database;
using OneDas.Infrastructure;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class DatasetInfoViewModel
    {
        #region Fields

        private DatasetInfo _model;

        #endregion

        #region Constructors

        public DatasetInfoViewModel(DatasetInfo dataset)
        {
            _model = dataset;
        }

        #endregion

        #region Properties

        public string Name => _model.Name;

        public OneDasDataType DataType => _model.DataType;

        public bool IsSelected { get; set; }

        #endregion
    }
}
