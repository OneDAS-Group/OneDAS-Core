using ImcFamosFile;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OneDas.Hdf.VdsTool.Import
{
    public class FamosImc2DataReader : IDataReader
    {
        #region Fields

        string _sourceDirectoryPath;

        #endregion

        #region Constructors

        public FamosImc2DataReader(string sourceDirectoryPath)
        {
            _sourceDirectoryPath = sourceDirectoryPath;
        }

        #endregion

        #region Methods

        public List<VariableDescription> GetVariableDescriptions()
        {
            var firstFilePath = Directory.EnumerateFiles(_sourceDirectoryPath).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(firstFilePath))
            {
                return new List<VariableDescription>();
            }
            else
            {
                using var famosFile = FamosFile.Open(firstFilePath);
                var fields = famosFile.Fields.Where(field => field.Type == FamosFileFieldType.MultipleYToSingleEquidistantTime).ToList();

                return fields.SelectMany(field =>
                {
                    var variableDescriptions = new List<VariableDescription>();

                    foreach (var component in field.Components)
                    {
                        var analogComponent = component as FamosFileAnalogComponent;

                        if (analogComponent == null)
                            continue;

                        if (analogComponent.CalibrationInfo == null) // unfortunately XAxisScaling is null
                            continue;

#warning Generic data reader is difficult to implement. Better would be powershell scripts or command line arguments to define things like sample rate
                        var channel = component.Channels.First();

                        // variable name
                        var variableName = OneDasUtilities.EnforceNamingConvention(channel.Name);

                        // samples per day
#warning This is a magic number
                        var period = TimeSpan.FromMinutes(10).TotalSeconds;
                        ulong samplesPerDay = (ulong)(component.GetSize() * 86400.0 / period);

                        // dataset name
                        var datasetName = $"{(ulong)(samplesPerDay/86400)} Hz";

                        // group name
                        var group = famosFile.Groups.FirstOrDefault(group => group.Channels.Contains(channel));
                        var groupName = group != null ? group.Name : "General";

                        // data type
                        var dataType = this.FamosFileDataTypeToOneDasDataType(component.PackInfo.DataType);

                        // unit
                        var unit = analogComponent.CalibrationInfo.Unit;

                        // transfer function set
                        var argument = $"{analogComponent.CalibrationInfo.Factor};{analogComponent.CalibrationInfo.Offset}";
                        var transferFunctionSet = new List<TransferFunction>() { new TransferFunction(DateTime.MinValue, "polynomial", string.Empty, argument) };

                        // data storage type
                        var dataStorageType = typeof(ExtendedDataStorageBase);

                        // create variable description
                        var variableDescription = new VariableDescription(Guid.Empty, variableName, datasetName, groupName, dataType, samplesPerDay, unit, transferFunctionSet, dataStorageType);
                        variableDescriptions.Add(variableDescription);
                    }

                    return variableDescriptions;
                }).ToList();
            }
        }

        public (TimeSpan Period, List<IDataStorage> DataStorageSet) GetData(DateTime startDateTime)
        {
            var dataStorageSet = Enumerable.Range(0, 40).Select(_ => new ExtendedDataStorage<float>(1024)).Cast<IDataStorage>().ToList();

            return (TimeSpan.FromMinutes(10), dataStorageSet);
        }

        private OneDasDataType FamosFileDataTypeToOneDasDataType(FamosFileDataType dataType)
        {
            return dataType switch
            {
                FamosFileDataType.UInt8 => OneDasDataType.UINT8,
                FamosFileDataType.Int8 => OneDasDataType.INT8,
                FamosFileDataType.UInt16 => OneDasDataType.UINT16,
                FamosFileDataType.Int16 => OneDasDataType.INT16,
                FamosFileDataType.UInt32 => OneDasDataType.UINT32,
                FamosFileDataType.Int32 => OneDasDataType.INT32,
                FamosFileDataType.Float32 => OneDasDataType.FLOAT32,
                FamosFileDataType.Float64 => OneDasDataType.FLOAT64,
                FamosFileDataType.Digital16Bit => OneDasDataType.UINT16,

                _ => throw new NotSupportedException()
            };
        }

        #endregion
    }
}
