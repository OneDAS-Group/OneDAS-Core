class OneDasModuleModel
{
    public DataType: OneDasDataTypeEnum
    public DataDirection: DataDirectionEnum
    public Size: number

    constructor(dataType: OneDasDataTypeEnum, dataDirection: DataDirectionEnum, size: number)
    {
        this.DataType = dataType
        this.DataDirection = dataDirection
        this.Size = size
    }
}
