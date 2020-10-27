using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;

namespace OneDas.DataManagement.Explorer.Core
{
    public class ProjectSettings
    {
        public ProjectSettings(SparseProjectInfo project,
                                DataReaderExtensionBase nativeDataReader,
                                DataReaderExtensionBase aggregationDataReader)
        {
            this.Project = project;
            this.NativeDataReader = nativeDataReader;
            this.AggregationDataReader = aggregationDataReader;
        }


        public SparseProjectInfo Project { get; }

        public DataReaderExtensionBase NativeDataReader { get; }

        public DataReaderExtensionBase AggregationDataReader { get; }
    }
}
