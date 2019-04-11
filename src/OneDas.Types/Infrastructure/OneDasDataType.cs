namespace OneDas.Infrastructure
{
    /// <summary>
    /// Specifies the data type of the OneDAS data.
    /// </summary>
    public enum OneDasDataType
    {
        BOOLEAN = 0x008,
        UINT8 = 0x108,
        INT8 = 0x208,
        UINT16 = 0x110,
        INT16 = 0x210,
        UINT32 = 0x120,
        INT32 = 0x220,
        UINT64 = 0x140,
        INT64 = 0x240,
        FLOAT32 = 0x320,
        FLOAT64 = 0x340
    }
}