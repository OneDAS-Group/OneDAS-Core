class BufferRequestViewModel
{
    public SampleRate: KnockoutObservable<SampleRateEnum>
    public GroupFilter: KnockoutObservable<string>
    public ErrorMessage: KnockoutObservable<string>
    public HasError: KnockoutComputed<boolean>

    public SampleRateSet: KnockoutObservableArray<SampleRateEnum>

    private _onPropertyChanged: EventDispatcher<BufferRequestViewModel, any>

    constructor(model: BufferRequestModel)
    {
        this.SampleRateSet = ko.observableArray<SampleRateEnum>(EnumerationHelper.GetEnumValues("SampleRateEnum"))
        this.SampleRate = ko.observable<SampleRateEnum>(model.SampleRate);
        this.GroupFilter = ko.observable<string>(model.GroupFilter);

        this.ErrorMessage = ko.observable<string>("")
        this.HasError = ko.computed<boolean>(() => this.ErrorMessage().length > 0)

        this._onPropertyChanged = new EventDispatcher<BufferRequestViewModel, any>();

        this.SampleRate.subscribe(newValue => this.OnPropertyChanged())
        this.GroupFilter.subscribe(newValue => this.OnPropertyChanged())
    }

    get PropertyChanged(): IEvent<BufferRequestViewModel, any>
    {
        return this._onPropertyChanged;
    }

    public OnPropertyChanged = () =>
    {
        this.Validate()
        this._onPropertyChanged.dispatch(this, null)
    }

    public Validate()
    {
        let result: any

        this.ErrorMessage("")

        this.GroupFilter().split(";").forEach(groupFilter =>
        {
            result = this.CheckGroupFilter(groupFilter)

            if (result.HasError)
            {
                this.ErrorMessage(result.ErrorDescription)

                return
            }
        })
    }

    public ToString()
    {
        return "sample rate: " + EnumerationHelper.GetEnumLocalization("SampleRateEnum", this.SampleRate()) + " / group filter: '" + this.GroupFilter() + "'"
    }

    public ToModel()
    {
        let model: any = {
            SampleRate: <SampleRateEnum>this.SampleRate(),
            GroupFilter: <string>this.GroupFilter()
        }

        return model
    }

    private CheckGroupFilter(value: string)
    {
        var regExp: any

        if (value.length === 0) {
            return { HasError: true, ErrorDescription: ErrorMessage["Project_GroupFilterEmpty"] }
        }

        regExp = new RegExp("[^A-Za-z0-9_!*]")

        if (regExp.test(value)) {
            return { HasError: true, ErrorDescription: ErrorMessage["Project_InvalidCharacters"] }
        }

        regExp = new RegExp("^[0-9_]")

        if (regExp.test(value)) {
            return { HasError: true, ErrorDescription: ErrorMessage["Project_InvalidLeadingCharacter"] }
        }

        regExp = new RegExp("^!")

        if (regExp.test(value) && value.length === 1) {
            return { HasError: true, ErrorDescription: ErrorMessage["Project_DetachedExclamationMarkNotAllowed"] }
        }

        return {
            HasError: false,
            ErrorDescription: ""
        }
    }
}