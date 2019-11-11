class BufferRequestSelectorViewModel
{
    public NewBufferRequest: KnockoutObservable<BufferRequestViewModel>  
    public BufferRequestSet: KnockoutObservableArray<BufferRequestViewModel>
    public ErrorMessage: KnockoutObservable<string>
    public HasError: KnockoutComputed<boolean>

    private _onBufferRequestSetChanged: EventDispatcher<BufferRequestSelectorViewModel, BufferRequestViewModel[]>

    constructor(bufferRequestSet: BufferRequestViewModel[] = [])
    {
        this.NewBufferRequest = ko.observable<BufferRequestViewModel>();
        this.BufferRequestSet = ko.observableArray<BufferRequestViewModel>(bufferRequestSet);
        this.ErrorMessage = ko.observable<string>("")
        this.HasError = ko.computed<boolean>(() => this.ErrorMessage().length > 0)

        this._onBufferRequestSetChanged = new EventDispatcher<BufferRequestSelectorViewModel, BufferRequestViewModel[]>();

        this.InternalCreateNewBufferRequest()
        this.InternalUpdate()
    }

    get OnBufferRequestSetChanged(): IEvent<BufferRequestSelectorViewModel, BufferRequestViewModel[]>
    {
        return this._onBufferRequestSetChanged;
    }
    
    // methods
    private InternalUpdate()
    {
        this.Update()
        this.Validate()
    }

    protected Update()
    {
        //
    }

    protected Validate()
    {
        this.ErrorMessage("")

        if (this.NewBufferRequest().HasError())
        {
            this.ErrorMessage("Resolve all remaining errors before continuing.")
        }
    }

    protected CreateNewBufferRequest()
    {
        if (this.NewBufferRequest())
        {
            return new BufferRequestViewModel(new BufferRequestModel(this.NewBufferRequest().SampleRate(), this.NewBufferRequest().GroupFilter()))
        }
        else
        {
            return new BufferRequestViewModel(new BufferRequestModel(SampleRateEnum.SampleRate_1, "*"))
        }
    }

    private InternalCreateNewBufferRequest()
    {
        if (this.NewBufferRequest())
        {
            this.NewBufferRequest().PropertyChanged.unsubscribe(this.OnBufferRequestPropertyChanged)
        }

        this.NewBufferRequest(this.CreateNewBufferRequest())
        this.NewBufferRequest().PropertyChanged.subscribe(this.OnBufferRequestPropertyChanged)
    }

    private OnBufferRequestPropertyChanged = () =>
    {
        this.InternalUpdate()
    }

    // commands
    public AddBufferRequest = () =>
    {
        let newBufferRequest: BufferRequestViewModel

        if (!this.HasError())
        {
            this.BufferRequestSet.push(this.NewBufferRequest())
            this.InternalCreateNewBufferRequest()
            this.InternalUpdate()
            this._onBufferRequestSetChanged.dispatch(this, this.BufferRequestSet())
        }
    }

    public DeleteBufferRequest = () =>
    {
        this.BufferRequestSet.pop()
        this.InternalUpdate()
        this._onBufferRequestSetChanged.dispatch(this, this.BufferRequestSet())
    }
}