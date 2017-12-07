class OneDasModuleSelectorViewModel
{
    public SettingsTemplateName: KnockoutObservable<string>
    public NewModule: KnockoutObservable<OneDasModuleViewModel>  
    public MaxBytes: KnockoutObservable<number>
    public RemainingBytes: KnockoutObservable<number>
    public RemainingCount: KnockoutObservable<number>    
    public ModuleSet: KnockoutObservableArray<OneDasModuleViewModel>
    public ErrorMessage: KnockoutObservable<string>
    public HasError: KnockoutComputed<boolean>
    public DataDirection: KnockoutObservable<DataDirectionEnum>

    private _onModuleSetChanged: EventDispatcher<OneDasModuleSelectorViewModel, OneDasModuleViewModel[]>

    constructor(dataDirection: DataDirectionEnum, moduleSet: OneDasModuleViewModel[] = [])
    {
        this.SettingsTemplateName = ko.observable("Project_OneDasModuleSettingsTemplate")
        this.NewModule = ko.observable<OneDasModuleViewModel>();
        this.MaxBytes = ko.observable<number>(Infinity);
        this.RemainingBytes = ko.observable<number>(NaN);
        this.RemainingCount = ko.observable<number>(NaN);
        this.ModuleSet = ko.observableArray<OneDasModuleViewModel>(moduleSet);
        this.ErrorMessage = ko.observable<string>("")
        this.HasError = ko.computed<boolean>(() => this.ErrorMessage().length > 0)
        this.DataDirection = ko.observable<DataDirectionEnum>(dataDirection)

        this._onModuleSetChanged = new EventDispatcher<OneDasModuleSelectorViewModel, OneDasModuleViewModel[]>();

        this.InternalCreateNewModule()
        this.Update()
    }

    get OnModuleSetChanged(): IEvent<OneDasModuleSelectorViewModel, OneDasModuleViewModel[]>
    {
        return this._onModuleSetChanged;
    }
    
    // methods
    public SetMaxBytes = (value: number) =>
    {
        this.MaxBytes(value)
        this.Update()
    }

    protected Update()
    {
        let usedBytes: number
        let remainingBytes: number

        usedBytes = this.ModuleSet().map(oneDasModule => oneDasModule.GetByteCount()).reduce((previousValue, currentValue) => previousValue + currentValue, 0)
        remainingBytes = this.MaxBytes() - usedBytes

        this.RemainingBytes(remainingBytes)
        this.RemainingCount(Math.floor(this.RemainingBytes() / ((this.NewModule().DataType() & 0x0FF) / 8)))
        this.NewModule().Size(Math.min(1, this.RemainingCount()))
    }

    protected Validate()
    {
        this.ErrorMessage("")

        if (this.NewModule().HasError())
        {
            this.ErrorMessage("Resolve all remaining module errors before continuing.")
        }

        if (isFinite(this.RemainingBytes()) && (this.RemainingBytes() - this.NewModule().GetByteCount() < 0))
        {
            this.ErrorMessage("Size of new module is too big.")
        }
    }

    protected CreateNewModule()
    {
        return new OneDasModuleViewModel(new OneDasModuleModel(OneDasDataTypeEnum.UINT16, this.DataDirection(), EndiannessEnum.LittleEndian, 1))
    }

    private InternalCreateNewModule()
    {
        if (this.NewModule())
        {
            this.NewModule().PropertyChanged.unsubscribe(this.OnModulePropertyChanged)
        }

        this.NewModule(this.CreateNewModule())
        this.NewModule().PropertyChanged.subscribe(this.OnModulePropertyChanged)
    }

    private OnModulePropertyChanged = () =>
    {
        this.Validate()
    }

    // commands
    public AddModule = () =>
    {
        let newModule: OneDasModuleViewModel

        if (!this.HasError())
        {
            this.ModuleSet.push(this.NewModule())
            this.InternalCreateNewModule()
            this.Update()
            this._onModuleSetChanged.dispatch(this, this.ModuleSet())
        }
    }

    public DeleteModule = () =>
    {
        this.ModuleSet.pop()
        this.Update()
        this._onModuleSetChanged.dispatch(this, this.ModuleSet())
    }
}