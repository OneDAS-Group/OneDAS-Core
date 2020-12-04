using OneDas.Infrastructure;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace OneDas
{
    public static class OneDasUtilities
    {
        public static double ToUnixTimeStamp(this DateTime value)
        {
            return value.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

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

        public static Type GetTypeFromOneDasDataType(OneDasDataType dataType)
        {
            return dataType switch
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
                _                                   => throw new NotSupportedException($"The specified data type '{dataType}' is not supported.")
            };
        }

        public static byte GetBitLength(OneDasDataType oneDasDataType, bool expandedBoolean)
        {
            if (!expandedBoolean && oneDasDataType == OneDasDataType.BOOLEAN)
                return 1;
            else
                return Convert.ToByte((int)oneDasDataType & 0xff);
        }

        public static bool CheckNamingConvention(string value, out string errorDescription, bool includeValue = false)
        {
            var valueAsString = string.Empty;

            if (includeValue)
                valueAsString = $" (value: '{value}')";

            errorDescription = true switch
            {
                true when string.IsNullOrWhiteSpace(value)      => $"{ErrorMessage.OneDasUtilities_NameEmpty}{valueAsString}",
                true when Regex.IsMatch(value, "[^A-Za-z0-9_]") => $"{ErrorMessage.OneDasUtilities_InvalidCharacters}{valueAsString}",
                true when Regex.IsMatch(value, "^[0-9_]")       => $"{ErrorMessage.OneDasUtilities_InvalidLeadingCharacter}{valueAsString}",
                _                                               => string.Empty
            };

            return string.IsNullOrWhiteSpace(errorDescription);
        }

        public static string EnforceNamingConvention(string value, string prefix = "X")
        {
            if (string.IsNullOrWhiteSpace(value))
                value = "unnamed";

            value = Regex.Replace(value, "[^A-Za-z0-9_]", "_");

            if (Regex.IsMatch(value, "^[0-9_]"))
                value = $"{prefix}_" + value;

            return value;
        }

        public static object InvokeGenericMethod<T>(T instance, string methodName, BindingFlags bindingFlags, Type genericType, object[] parameters)
        {
            return OneDasUtilities.InvokeGenericMethod(typeof(T), instance, methodName, bindingFlags, genericType, parameters);
        }

        public static object InvokeGenericMethod(Type methodParent, object instance, string methodName, BindingFlags bindingFlags, Type genericType, object[] parameters)
        {
            var methodInfo = methodParent.GetMethod(methodName, bindingFlags);
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericType);

            return genericMethodInfo.Invoke(instance, parameters);
        }
    }
}