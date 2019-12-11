class CanModuleViewModel extends OneDasModuleViewModel
{
    public Identifier: KnockoutObservable<number>
    public FrameFormat: KnockoutObservable<CanFrameFormatEnum>
    public FrameFormatSet: KnockoutObservableArray<CanFrameFormatEnum>

    constructor(canModuleModel: CanModuleModel)
    {
        super(canModuleModel)

        this.Identifier = ko.observable<number>(canModuleModel.Identifier)
        this.FrameFormat = ko.observable<CanFrameFormatEnum>(canModuleModel.FrameFormat)
        this.FrameFormatSet = ko.observableArray<CanFrameFormatEnum>(EnumerationHelper.GetEnumValues("CanFrameFormatEnum").filter(objectType => objectType >= 3))

        this.Identifier.subscribe(newValue => this.OnPropertyChanged())
        this.FrameFormat.subscribe(newValue => this.OnPropertyChanged())
    }

    public Validate()
    {
        super.Validate()

        switch (this.FrameFormat()) {

            case CanFrameFormatEnum.Standard:

                if (!(0 <= this.Identifier() || this.Identifier() <= Math.pow(2, 11) - 1)) {
                    this.ErrorMessage("The identifier of a standard frame must be between 0..2047.")
                }

                break

            case CanFrameFormatEnum.Extended:

                if (!(0 <= this.Identifier() || this.Identifier() <= Math.pow(2, 29) - 1)) {
                    this.ErrorMessage("The identifier of an extended frame must be between 0..536870911.")
                }

                break
        }
    }

    public ToString()
    {
        return super.ToString() + ' - ' + EnumerationHelper.GetEnumLocalization("CanFrameFormatEnum", this.FrameFormat()) + " - identifier: " + this.Identifier()
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.Identifier = <number>this.Identifier()
        model.FrameFormat = <CanFrameFormatEnum>this.FrameFormat()
    }
}