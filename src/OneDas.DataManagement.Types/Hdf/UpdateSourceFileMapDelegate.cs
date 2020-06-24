using OneDas.DataManagement.Database;

namespace OneDas.DataManagement.Hdf
{
    public delegate void UpdateSourceFileMapDelegate(long datasetId, HdfDatasetInfo datasetInfo, SourceFileInfo sourceFile);
}
