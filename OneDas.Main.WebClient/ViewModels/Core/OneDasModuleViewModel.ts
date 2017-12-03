class OneDasModuleViewModel
{
    public DataType: KnockoutObservable<OneDasDataTypeEnum>
    public DataDirection: KnockoutObservable<DataDirectionEnum>
    public Size: KnockoutObservable<number>

    protected _onPropertyChanged: EventDispatcher<OneDasModuleViewModel, any>

    constructor(model: OneDasModuleModel)
    {
        this.DataType = ko.observable(model.DataType)
        this.DataDirection = ko.observable(model.DataDirection)
        this.Size = ko.observable(model.Size)

        this._onPropertyChanged = new EventDispatcher<OneDasModuleViewModel, any>();

        this.DataType.subscribe(newValue => this._onPropertyChanged.dispatch(this, null))
        this.DataDirection.subscribe(newValue => this._onPropertyChanged.dispatch(this, null))
        this.Size.subscribe(newValue => this._onPropertyChanged.dispatch(this, null))
    }

    get OnPropertyChanged(): IEvent<OneDasModuleViewModel, any>
    {
        return this._onPropertyChanged;
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
        return this.Size() + "x " + EnumerationHelper.GetEnumLocalization('OneDasDataTypeEnum', this.DataType())
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
