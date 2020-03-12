using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using System;
using System.Collections.Generic;

public class DataReaderProgressRecord
{
    #region Constructors

    public DataReaderProgressRecord(Dictionary<DatasetInfo, DataRecord> datasetToRecordMap, DateTime begin, DateTime end)
    {
        this.DatasetToRecordMap = datasetToRecordMap;
        this.Begin = begin;
        this.End = end;
    }

    #endregion

    #region Properties

    public Dictionary<DatasetInfo, DataRecord> DatasetToRecordMap { get; }

    public DateTime Begin { get; }

    public DateTime End { get; }

    #endregion
}