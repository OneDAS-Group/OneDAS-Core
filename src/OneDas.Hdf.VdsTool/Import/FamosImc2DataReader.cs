using ImcFamosFile;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.Hdf.VdsTool.Import
{
    public class FamosImc2DataReader : IDataReader
    {
        #region Constructors

        public FamosImc2DataReader()
        {
            //
        }

        #endregion

        #region Methods

        public List<VariableDescription> GetVariableDescriptions(string filePath)
        {
            using var famosFile = FamosFile.Open(filePath);
            var fields = famosFile.Fields.Where(field => field.Type == FamosFileFieldType.MultipleYToSingleEquidistantTime).ToList();

            return fields.SelectMany(field =>
            {
                var variableDescriptions = new List<VariableDescription>();

                foreach (var component in field.Components)
                {
                    var analogComponent = component as FamosFileAnalogComponent;

                    if (analogComponent == null)
                        continue;

                    if (analogComponent.CalibrationInfo == null)
                        continue;

                    var channel = component.Channels.First();

                    // variable name
                    var variableName = OneDasUtilities.EnforceNamingConvention(channel.Name);

                    // samples per day
                    SampleRateContainer sampleRate;

                    var xAxisScaling = component.XAxisScaling;

                    if (xAxisScaling != null && xAxisScaling.Unit == "s")
                        sampleRate = new SampleRateContainer((ulong)((decimal)86400UL / xAxisScaling.DeltaX));
                    else
                        throw new Exception("Could not determine the sample rate.");

                    // dataset name
                    var datasetName = sampleRate.ToUnitString();

                    // group name
                    var group = famosFile.Groups.FirstOrDefault(group => group.Channels.Contains(channel));
                    var groupName = group != null ? group.Name : "General";

                    // data type
                    var dataType = this.GetOneDasDataTypeFromFamosFileDataType(component.PackInfo.DataType);

                    // unit
                    var unit = analogComponent.CalibrationInfo.Unit;

                    // transfer function set
                    var argument = $"{analogComponent.CalibrationInfo.Factor};{analogComponent.CalibrationInfo.Offset}";
                    var transferFunctionSet = new List<TransferFunction>() { new TransferFunction(DateTime.MinValue, "polynomial", string.Empty, argument) };

                    // create variable description
                    var variableDescription = new VariableDescription(Guid.Empty, variableName, datasetName, groupName, dataType, sampleRate, unit, transferFunctionSet, DataStorageType.Extended);
                    variableDescriptions.Add(variableDescription);
                }

                return variableDescriptions;
            }).ToList();
        }

        public List<IDataStorage> GetData(string filePath, List<VariableDescription> variableDescriptionSet, bool convertToDouble)
        {
            using var famosFile = FamosFile.Open(filePath);

            var channels = famosFile.Groups.SelectMany(group => group.Channels).Concat(famosFile.Channels).ToList();

            return variableDescriptionSet.Select(variableDescription =>
            {
                var channel = channels.First(channel => OneDasUtilities.EnforceNamingConvention(channel.Name) == variableDescription.VariableName);
                var channelData = famosFile.ReadSingle(channel);
                var componentsData = channelData.ComponentsData.First();
                var rawData = componentsData.RawData;
                var elementSize = OneDasUtilities.SizeOf(this.GetOneDasDataTypeFromFamosFileDataType(channelData.ComponentsData.First().PackInfo.DataType));
                var statusSet = new byte[rawData.Length / elementSize];
                
                statusSet.AsSpan().Fill(0x01);
#warning Improve DataStorage system. Using managed memory, Span<T> and work only with byte arrays is more suitable for everything

                // Do not use variableInfo.DataType because it could have been modified to double if 'convertToDouble' is true!
                var dataStorage = componentsData.PackInfo.DataType switch
                {
                    FamosFileDataType.UInt8         => this.CreateDataStorage<byte>(rawData, statusSet),
                    FamosFileDataType.Int8          => this.CreateDataStorage<sbyte>(rawData, statusSet),
                    FamosFileDataType.UInt16        => this.CreateDataStorage<ushort>(rawData, statusSet),
                    FamosFileDataType.Int16         => this.CreateDataStorage<short>(rawData, statusSet),
                    FamosFileDataType.UInt32        => this.CreateDataStorage<uint>(rawData, statusSet),
                    FamosFileDataType.Int32         => this.CreateDataStorage<int>(rawData, statusSet),
                    FamosFileDataType.Float32       => this.CreateDataStorage<float>(rawData, statusSet),
                    FamosFileDataType.Float64       => this.CreateDataStorage<double>(rawData, statusSet),
                    FamosFileDataType.Digital16Bit  => this.CreateDataStorage<ushort>(rawData, statusSet),
                    _                               => throw new NotSupportedException()
                };

                if (convertToDouble)
                    return dataStorage.ToSimpleDataStorage();
                else
                    return dataStorage;
            }).ToList();
        }

        private IDataStorage CreateDataStorage<T>(byte[] rawData, byte[] statusSet) where T : unmanaged
        {
            var data = MemoryMarshal.Cast<byte, T>(rawData.AsSpan());
            return new ExtendedDataStorage<T>(data, statusSet);
        }

        private OneDasDataType GetOneDasDataTypeFromFamosFileDataType(FamosFileDataType dataType)
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
