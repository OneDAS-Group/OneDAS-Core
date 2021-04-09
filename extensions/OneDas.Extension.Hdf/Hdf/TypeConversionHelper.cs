using HDF.PInvoke;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OneDas.Extension.Hdf
{
    public static class TypeConversionHelper
    {
        public static long GetHdfTypeIdFromType(Type type)
        {
            return TypeConversionHelper.GetHdfTypeIdFromType(-1, type);
        }

        public static long GetHdfTypeIdFromType(long fileId, Type type)
        {
            Type elementType;
            
            elementType = type.IsArray ? type.GetElementType() : type;

            if (elementType == typeof(bool))
            {
                return H5T.NATIVE_UINT8;
            }
            else if (elementType == typeof(Byte))
            {
                return H5T.NATIVE_UINT8;
            }
            else if (elementType == typeof(SByte))
            {
                return H5T.NATIVE_INT8;
            }
            else if (elementType == typeof(UInt16))
            {
                return H5T.NATIVE_UINT16;
            }
            else if (elementType == typeof(Int16))
            {
                return H5T.NATIVE_INT16;
            }
            else if (elementType == typeof(UInt32))
            {
                return H5T.NATIVE_UINT32;
            }
            else if (elementType == typeof(Int32))
            {
                return H5T.NATIVE_INT32;
            }
            else if (elementType == typeof(UInt64))
            {
                return H5T.NATIVE_UINT64;
            }
            else if (elementType == typeof(Int64))
            {
                return H5T.NATIVE_INT64;
            }
            else if (elementType == typeof(Single))
            {
                return H5T.NATIVE_FLOAT;
            }
            else if (elementType == typeof(Double))
            {
                return H5T.NATIVE_DOUBLE;
            }
            else if (elementType == typeof(string) || elementType == typeof(IntPtr))
            {
                long typeId = 0;

                if (H5I.is_valid(fileId) > 0 && H5L.exists(fileId, "string_t") > 0)
                {
                    typeId = H5T.open(fileId, "string_t");
                }
                else
                {
                    typeId = H5T.copy(H5T.C_S1);

                    H5T.set_size(typeId, H5T.VARIABLE);
                    H5T.set_cset(typeId, H5T.cset_t.UTF8);

                    if (fileId > -1 && H5T.commit(fileId, "string_t", typeId) < 0)
                    {
                        throw new Exception(ErrorMessage.TypeConversionHelper_CouldNotCommitDataType);
                    }
                }

                return typeId;
            }
            else if (elementType.IsValueType && !elementType.IsPrimitive && !elementType.IsEnum)
            {
                long typeId = 0;

                if (H5I.is_valid(fileId) > 0 && H5L.exists(fileId, elementType.Name) > 0)
                {
                    typeId = H5T.open(fileId, elementType.Name);
                }
                else
                {
                    typeId = H5T.create(H5T.class_t.COMPOUND, new IntPtr(Marshal.SizeOf(elementType)));

                    foreach (FieldInfo fieldInfo in elementType.GetFields())
                    {
                        long fieldType = -1;

                        fieldType = TypeConversionHelper.GetHdfTypeIdFromType(fileId, fieldInfo.FieldType);

                        H5T.insert(typeId, fieldInfo.Name, Marshal.OffsetOf(elementType, fieldInfo.Name), fieldType);

                        if (H5I.is_valid(fieldType) > 0)
                        {
                            H5T.close(fieldType);
                        }
                    }

                    if (fileId > -1 && H5T.commit(fileId, elementType.Name, typeId) < 0)
                    {
                        throw new Exception(ErrorMessage.TypeConversionHelper_CouldNotCommitDataType);
                    }
                }

                return typeId;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
