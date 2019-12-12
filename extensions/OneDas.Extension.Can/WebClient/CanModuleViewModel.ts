class CanModuleViewModel extends OneDasModuleViewModel
{
    public Identifier: KnockoutObservable<number>
    public FrameFormat: KnockoutObservable<CanFrameFormatEnum>

    public CanFrameFormatSet: KnockoutObservableArray<CanFrameFormatEnum>

    constructor(canModuleModel: CanModuleModel)
    {
        super(canModuleModel)

        this.Identifier = ko.observable<number>(canModuleModel.Identifier)
        this.FrameFormat = ko.observable<CanFrameFormatEnum>(canModuleModel.FrameFormat)

        this.CanFrameFormatSet = ko.observableArray<CanFrameFormatEnum>(EnumerationHelper.GetEnumValues("CanFrameFormatEnum"))

        this.Identifier.subscribe(_ => this.OnPropertyChanged())
        this.FrameFormat.subscribe(_ => this.OnPropertyChanged())

        this.MaxSize(8)

        // improve: better would be server side generation of correct module
        if (!this._model.$type) {
            this._model.$type = "OneDas.Extension.Can.CanModule, OneDas.Extension.Can"
        }
    }

    public Validate()
    {
        super.Validate()

        switch (this.FrameFormat()) {

            case CanFrameFormatEnum.Standard:

                if (!(0 <= this.Identifier() && this.Identifier() <= Math.pow(2, 11) - 1)) {
                    this.ErrorMessage("The identifier of a standard frame must be between 0..2047.")
                }

                break

            case CanFrameFormatEnum.Extended:

                if (!(0 <= this.Identifier() && this.Identifier() <= Math.pow(2, 29) - 1)) {
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