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
            {
                return 1;
            }
            else
            {
                return Marshal.SizeOf(type);
            }
        }

        public static bool ValidateIPv4(string ipAddressString, out IPAddress ipAddress)
        {
            ipAddress = null;

            return IPAddress.TryParse(ipAddressString, out ipAddress) && ipAddressString.Split('.').Length == 4;
        }

        public static T GetFirstAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(false).OfType<T>().FirstOrDefault();
        }

        public static ulong GetSamplesPerDayFromSampleRate(SampleRate sampleRate)
        {
            switch (sampleRate)
            {
                case SampleRate.SampleRate_100:
                    return 86400 * 100;
                case SampleRate.SampleRate_25:
                    return 86400 * 25;
                case SampleRate.SampleRate_5:
                    return 86400 * 5;
                case SampleRate.SampleRate_1:
                    return 86400 * 1;
                default:
                    throw new ArgumentException();
            }
        }

        // Improve! RegEx
        public static ulong GetSamplesPerDayFromString(string datasetName)
        {
            if (datasetName.StartsWith("100 Hz"))
            {
                return 86400 * 100;
            }
            else if (datasetName.StartsWith("25 Hz"))
            {
                return 86400 * 25;
            }
            else if (datasetName.StartsWith("5 Hz"))
            {
                return 86400 * 5;
            }
            else if (datasetName.StartsWith("1 Hz"))
            {
                return 86400 * 1;
            }
            else if (datasetName.StartsWith("1 s"))
            {
                return 86400 * 1;
            }
            else if (datasetName.StartsWith("60 s"))
            {
                return 86400 / 60;
            }
            else if (datasetName.StartsWith("600 s"))
            {
                return 86400 / 600;
            }
            else if (datasetName == "is_chunk_completed_set")
            {
                return 86400 / 60;
            }
            else
            {
                throw new ArgumentException(nameof(datasetName));
            }
        }

        public static OneDasDataType GetOneDasDataTypeFromType(Type type)
        {
            if (type == typeof(bool))
            {
                return OneDasDataType.BOOLEAN;
            }
            else if (type == typeof(Byte))
            {
                return OneDasDataType.UINT8;
            }
            else if (type == typeof(SByte))
            {
                return OneDasDataType.INT8;
            }
            else if (type == typeof(UInt16))
            {
                return OneDasDataType.UINT16;
            }
            else if (type == typeof(Int16))
            {
                return OneDasDataType.INT16;
            }
            else if (type == typeof(UInt32))
            {
                return OneDasDataType.UINT32;
            }
            else if (type == typeof(Int32))
            {
                return OneDasDataType.INT32;
            }
            else if (type == typeof(UInt64))
            {
                return OneDasDataType.UINT64;
            }
            else if (type == typeof(Int64))
            {
                return OneDasDataType.INT64;
            }
            else if (type == typeof(Single))
            {
                return OneDasDataType.FLOAT32;
            }
            else if (type == typeof(Double))
            {
                return OneDasDataType.FLOAT64;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static Type GetTypeFromOneDasDataType(OneDasDataType oneDasDataType)
        {
            switch (oneDasDataType)
            {
                case OneDasDataType.BOOLEAN:
                    return typeof(bool);
                case OneDasDataType.UINT8:
                    return typeof(Byte);
                case OneDasDataType.INT8:
                    return typeof(SByte);
                case OneDasDataType.UINT16:
                    return typeof(UInt16);
                case OneDasDataType.INT16:
                    return typeof(Int16);
                case OneDasDataType.UINT32:
                    return typeof(UInt32);
                case OneDasDataType.INT32:
                    return typeof(Int32);
                case OneDasDataType.UINT64:
                    return typeof(UInt64);
                case OneDasDataType.INT64:
                    return typeof(Int64);
                case OneDasDataType.FLOAT32:
                    return typeof(Single);
                case OneDasDataType.FLOAT64:
                    return typeof(Double);
                default:
                    throw new NotSupportedException();
            }
        }

        public static byte GetBitLength(OneDasDataType oneDasDataType, bool expandedBoolean)
        {
            if (!expandedBoolean && oneDasDataType == OneDasDataType.BOOLEAN)
            {
                return 1;
            }
            else
            {
                return Convert.ToByte((int)oneDasDataType & 0xff);
            }
        }

        public static bool CheckNamingConvention(string value, out string errorDescription)
        {
            errorDescription = string.Empty;

            if (string.IsNullOrWhiteSpace(value))
            {
                errorDescription = ErrorMessage.OneDasUtilities_NameEmpty;
            }
            else if (Regex.IsMatch(value, "[^A-Za-z0-9_]"))
            {
                errorDescription = ErrorMessage.OneDasUtilities_InvalidCharacters;
            }
            else if (Regex.IsMatch(value, "^[0-9_]"))
            {
                errorDescription = ErrorMessage.OneDasUtilities_InvalidLeadingCharacter;
            }
            else
            {
                return true;
            }

            return false;
        }
    }
}