using OneDas.DataManagement.Database;
using OneDas.Infrastructure;

namespace OneDas.DataManagement.Explorer.ViewModels
{
    public class DatasetInfoViewModel
    {
        #region Constructors

        public DatasetInfoViewModel(DatasetInfo dataset, VariableInfoViewModel parent)
        {
            this.Model = dataset;
            this.Parent = parent;
        }

        #endregion

        #region Properties

        public DatasetInfo Model { get; }

        public string Name => Model.Name;

        public OneDasDataType DataType => Model.DataType;

        public VariableInfoViewModel Parent { get; }

        #endregion
    }
}
