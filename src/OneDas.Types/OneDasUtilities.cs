using OneDas.Infrastructure;
using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace OneDas
{
    public static class OneDasUtilities
    {
        public static int SizeOf(OneDasDataType dataType)
        {
            return OneDasUtilities.SizeOf(OneDasUtilities.GetTypeFromOneDasDataType(dataType));
        }

        public static int SizeOf(Type type)
        {
            if (type == typeof(bool))
                return 1;
            else
                return Marshal.SizeOf(type);
        }

        public static bool ValidateIPv4(string ipAddressString, out IPAddress ipAddress)
        {
            return IPAddress.TryParse(ipAddressString, out ipAddress) && ipAddressString.Split('.').Length == 4;
        }

        public static T GetFirstAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(false).OfType<T>().FirstOrDefault();
        }

        public static ulong GetSamplesPerDayFromSampleRate(SampleRate sampleRate)
        {
            return sampleRate switch
            {
                SampleRate.SampleRate_100   => 86400UL * 100,
                SampleRate.SampleRate_25    => 86400UL * 25,
                SampleRate.SampleRate_5     => 86400UL * 5,
                SampleRate.SampleRate_1     => 86400UL * 1,
                _                           => throw new ArgumentException()
            };
        }

        // Improve! RegEx
        public static ulong GetSamplesPerDayFromString(string datasetName)
        {
            return true switch
            {
                true when datasetName.StartsWith("100 Hz")                  => 86400UL * 100,
                true when datasetName.StartsWith("25 Hz")                   => 86400UL * 25,
                true when datasetName.StartsWith("5 Hz")                    => 86400UL * 5,
                true when datasetName.StartsWith("1 Hz")                    => 86400UL * 1,
                true when datasetName.StartsWith("1 s")                     => 86400UL * 1,
                true when datasetName.StartsWith("60 s")                    => 86400UL / 60,
                true when datasetName.StartsWith("600 s")                   => 86400UL / 600,
                true when datasetName.StartsWith("is_chunk_completed_set")  => 86400UL / 60,
                _                                                           => throw new ArgumentException(nameof(datasetName))
            };
        }

        public static OneDasDataType GetOneDasDataTypeFromType(Type type)
        {
            return true switch
            {
                true when type == typeof(bool)      => OneDasDataType.BOOLEAN,
                true when type == typeof(Byte)      => OneDasDataType.UINT8,
                true when type == typeof(SByte)     => OneDasDataType.INT8,
                true when type == typeof(UInt16)    => OneDasDataType.UINT16,
                true when type == typeof(Int16)     => OneDasDataType.INT16,
                true when type == typeof(UInt32)    => OneDasDataType.UINT32,
                true when type == typeof(Int32)     => OneDasDataType.INT32,
                true when type == typeof(UInt64)    => OneDasDataType.UINT64,
                true when type == typeof(Int64)     => OneDasDataType.INT64,
                true when type == typeof(Single)    => OneDasDataType.FLOAT32,
                true when type == typeof(Double)    => OneDasDataType.FLOAT64,
                _                                   => throw new NotSupportedException()
            };
        }

        public static Type GetTypeFromOneDasDataType(OneDasDataType dateType)
        {
            return dateType switch
            {
                OneDasDataType.BOOLEAN              => typeof(bool),
                OneDasDataType.UINT8                => typeof(Byte),
                OneDasDataType.INT8                 => typeof(SByte),
                OneDasDataType.UINT16               => typeof(UInt16),
                OneDasDataType.INT16                => typeof(Int16),
                OneDasDataType.UINT32               => typeof(UInt32),
                OneDasDataType.INT32                => typeof(Int32),
                OneDasDataType.UINT64               => typeof(UInt64),
                OneDasDataType.INT64                => typeof(Int64),
                OneDasDataType.FLOAT32              => typeof(Single),
                OneDasDataType.FLOAT64              => typeof(Double),
                _                                   => throw new NotSupportedException()
            };
        }

        public static byte GetBitLength(OneDasDataType oneDasDataType, bool expandedBoolean)
        {
            if (!expandedBoolean && oneDasDataType == OneDasDataType.BOOLEAN)
                return 1;
            else
                return Convert.ToByte((int)oneDasDataType & 0xff);
        }

        public static bool CheckNamingConvention(string value, out string errorDescription)
        {
            errorDescription = true switch
            {
                true when string.IsNullOrWhiteSpace(value)      => $"{ErrorMessage.OneDasUtilities_NameEmpty} (value: '{value}')",
                true when Regex.IsMatch(value, "[^A-Za-z0-9_]") => $"{ErrorMessage.OneDasUtilities_InvalidCharacters} (value: '{value}')",
                true when Regex.IsMatch(value, "^[0-9_]")       => $"{ErrorMessage.OneDasUtilities_InvalidLeadingCharacter} (value: '{value}')",
                _                                               => string.Empty
            };

            return string.IsNullOrWhiteSpace(errorDescription);
        }

        public static string EnforceNamingConvention(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                value = "unnamed";

            Regex.Replace(value, "[^A-Za-z0-9_]", "_");

            if (Regex.IsMatch(value, "^[0-9_]"))
                value = "X_" + value;

            return value;
        }
    }
}