class OneDasModuleViewModel implements IModelizable
{
    public DataType: OneDasDataTypeEnum
    public DataDirection: DataDirectionEnum
    public Size: number

    constructor(model: OneDasModuleModel)
    {
        if (!Number.isInteger(model.Size))
        {
            throw new Error("The value of size must be integer.")
        }

        if (model.Size <= 0)
        {
            throw new Error("The minimum value for size is 1.")
        }

        this.DataType = model.DataType
        this.DataDirection = model.DataDirection
        this.Size = model.Size
    }

    public GetByteCount = (booleanBitSize?: number) =>
    {
        if (booleanBitSize && this.DataType === OneDasDataTypeEnum.BOOLEAN)
        {
            booleanBitSize = parseInt(<any>booleanBitSize)

            return Math.ceil(booleanBitSize * this.Size / 8);
        }
        else
        {
            return (this.DataType & 0x0FF) / 8 * this.Size
        }
    }

    public ToModel()
    {
        return {
            DataType: <OneDasDataTypeEnum>this.DataType,
            Size: <number>this.Size,
            DataDirection: <DataDirectionEnum>this.DataDirection
        }
    }
}
