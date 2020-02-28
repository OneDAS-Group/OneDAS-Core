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

        public DatasetInfoViewModel(DatasetInfo dataset, VariableInfoViewModel parent)
        {
            _model = dataset;

            this.Parent = parent;
        }

        #endregion

        #region Properties

        public string Name => _model.Name;

        public OneDasDataType DataType => _model.DataType;

        public bool IsSelected { get; set; }

        public VariableInfoViewModel Parent { get; }

        #endregion
    }
}
