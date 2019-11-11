using OneDas.Infrastructure;
using Opc.Ua;
using System;

namespace OneDas.Extension.OpcUa
{
    public class OpcUaUtilities
    {
        public static BuiltInType GetOpcUaDataTypeFromOneDasDataType(OneDasDataType oneDasDataType)
        {
            switch (oneDasDataType)
            {
                case OneDasDataType.BOOLEAN:
                    return BuiltInType.Boolean;
                case OneDasDataType.UINT8:
                    return BuiltInType.Byte;
                case OneDasDataType.INT8:
                    return BuiltInType.SByte;
                case OneDasDataType.UINT16:
                    return BuiltInType.UInt16;
                case OneDasDataType.INT16:
                    return BuiltInType.Int16;
                case OneDasDataType.UINT32:
                    return BuiltInType.UInt32;
                case OneDasDataType.INT32:
                    return BuiltInType.Int32;
                case OneDasDataType.UINT64:
                    return BuiltInType.UInt64;
                case OneDasDataType.INT64:
                    return BuiltInType.Int32;
                case OneDasDataType.FLOAT32:
                    return BuiltInType.Float;
                case OneDasDataType.FLOAT64:
                    return BuiltInType.Double;
                default:
                    throw new ArgumentException(nameof(oneDasDataType));
            }
        }
    }
}
