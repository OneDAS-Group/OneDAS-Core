class OneDasModuleModel
{
    public DataType: OneDasDataTypeEnum
    public DataDirection: DataDirectionEnum
    public Endianness: EndiannessEnum
    public Size: number

    constructor(dataType: OneDasDataTypeEnum, dataDirection: DataDirectionEnum, endianness: EndiannessEnum, size: number)
    {
        this.DataType = dataType
        this.DataDirection = dataDirection
        this.Endianness = endianness
        this.Size = size
    }
}
