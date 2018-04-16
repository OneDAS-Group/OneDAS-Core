class OneDasModuleViewModel
{
    public DataType: KnockoutObservable<OneDasDataTypeEnum>
    public DataDirection: KnockoutObservable<DataDirectionEnum>
    public Endianness: KnockoutObservable<EndiannessEnum>
    public Size: KnockoutObservable<number>
    public MaxSize: KnockoutObservable<number>
    public ErrorMessage: KnockoutObservable<string>
    public HasError: KnockoutComputed<boolean>

    public DataTypeSet: KnockoutObservableArray<OneDasDataTypeEnum>

    private _onPropertyChanged: EventDispatcher<OneDasModuleViewModel, any>
    protected _model: any

    constructor(oneDasModuleModel: OneDasModuleModel)
    {
        this._model = oneDasModuleModel

        this.DataTypeSet = ko.observableArray<OneDasDataTypeEnum>(EnumerationHelper.GetEnumValues('OneDasDataTypeEnum').filter(dataType => dataType !== OneDasDataTypeEnum.BOOLEAN))
        this.DataType = ko.observable<OneDasDataTypeEnum>(oneDasModuleModel.DataType)
        this.DataDirection = ko.observable<DataDirectionEnum>(oneDasModuleModel.DataDirection)
        this.Endianness = ko.observable<EndiannessEnum>(oneDasModuleModel.Endianness)
        this.Size = ko.observable<number>(oneDasModuleModel.Size)
        this.MaxSize = ko.observable<number>(Infinity)
        this.ErrorMessage = ko.observable<string>("")
        this.HasError = ko.computed<boolean>(() => this.ErrorMessage().length > 0)

        this._onPropertyChanged = new EventDispatcher<OneDasModuleViewModel, any>();

        this.DataType.subscribe(newValue => this.OnPropertyChanged())
        this.DataDirection.subscribe(newValue => this.OnPropertyChanged())
        this.Size.subscribe(newValue => this.OnPropertyChanged())
    }

    get PropertyChanged(): IEvent<OneDasModuleViewModel, any>
    {
        return this._onPropertyChanged;
    }

    public OnPropertyChanged = () =>
    {
        this.Validate()
        this._onPropertyChanged.dispatch(this, null)
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

    public Validate()
    {
        this.ErrorMessage("")

        if (this.Size() < 1 || (isFinite(this.MaxSize()) && this.Size() > this.MaxSize()))
        {
            this.ErrorMessage("Size must be within range 1.." + this.MaxSize() + ".")
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
            $type: <string>this._model.$type,
            DataType: <OneDasDataTypeEnum>this.DataType(),
            Size: <number>this.Size(),
            DataDirection: <DataDirectionEnum>this.DataDirection(),
            Endianness: <EndiannessEnum>this.Endianness()
        }

        this.ExtendModel(model)

        return model
    }
}
