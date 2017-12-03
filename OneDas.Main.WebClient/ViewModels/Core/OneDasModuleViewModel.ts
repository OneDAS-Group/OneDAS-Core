class OneDasModuleViewModel
{
    public DataType: KnockoutObservable<OneDasDataTypeEnum>
    public DataDirection: KnockoutObservable<DataDirectionEnum>
    public Size: KnockoutObservable<number>

    constructor(model: OneDasModuleModel)
    {
        this.DataType = ko.observable(model.DataType)
        this.DataDirection = ko.observable(model.DataDirection)
        this.Size = ko.observable(model.Size)
    }

    public GetByteCount = (booleanBitSize?: number) =>
    {
        if (booleanBitSize && this.DataType() === OneDasDataTypeEnum.BOOLEAN)
        {
            booleanBitSize = parseInt(<any>booleanBitSize)

            return Math.ceil(booleanBitSize * this.Size() / 8);
        }
        else
        {
            return (this.DataType() & 0x0FF) / 8 * this.Size()
        }
    }

    public ToString()
    {
        return EnumerationHelper.GetEnumLocalization('OneDasDataTypeEnum', this.DataType()) + ' [ ' + this.Size() + 'x ]'
    }

    public ExtendModel(model: any)
    {
        //
    }

    public ToModel()
    {
        let model: any = {
            DataType: <OneDasDataTypeEnum>this.DataType(),
            Size: <number>this.Size(),
            DataDirection: <DataDirectionEnum>this.DataDirection()
        }

        this.ExtendModel(model)

        return model
    }
}
