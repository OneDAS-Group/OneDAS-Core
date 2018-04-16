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

    public OneDasModuleSelectorMode: KnockoutObservable<OneDasModuleSelectorModeEnum>

    private _onModuleSetChanged: EventDispatcher<OneDasModuleSelectorViewModel, OneDasModuleViewModel[]>

    constructor(oneDasModuleSelectorMode: OneDasModuleSelectorModeEnum, moduleSet: OneDasModuleViewModel[] = [])
    {
        this.OneDasModuleSelectorMode = ko.observable<OneDasModuleSelectorModeEnum>(oneDasModuleSelectorMode)

        this.SettingsTemplateName = ko.observable("Project_OneDasModuleTemplate")
        this.NewModule = ko.observable<OneDasModuleViewModel>();
        this.MaxBytes = ko.observable<number>(Infinity);
        this.RemainingBytes = ko.observable<number>(NaN);
        this.RemainingCount = ko.observable<number>(NaN);
        this.ModuleSet = ko.observableArray<OneDasModuleViewModel>(moduleSet);
        this.ErrorMessage = ko.observable<string>("")
        this.HasError = ko.computed<boolean>(() => this.ErrorMessage().length > 0)

        this._onModuleSetChanged = new EventDispatcher<OneDasModuleSelectorViewModel, OneDasModuleViewModel[]>();

        this.InternalCreateNewModule()
        this.InternalUpdate()
    }

    get OnModuleSetChanged(): IEvent<OneDasModuleSelectorViewModel, OneDasModuleViewModel[]>
    {
        return this._onModuleSetChanged;
    }
    
    // methods
    public SetMaxBytes = (value: number) =>
    {
        this.MaxBytes(value)
        this.InternalUpdate()
    }

    public GetInputModuleSet = () =>
    {
        return this.ModuleSet().filter(module => module.DataDirection() === DataDirectionEnum.Input)
    }

    public GetOutputModuleSet = () =>
    {
        return this.ModuleSet().filter(module => module.DataDirection() === DataDirectionEnum.Output)
    }

    private InternalUpdate()
    {
        this.Update()
        this.Validate()
    }

    protected Update()
    {
        let moduleSet: OneDasModuleViewModel[]
        let remainingBytes: number

        switch (this.NewModule().DataDirection())
        {
            case DataDirectionEnum.Input:
                moduleSet = this.GetInputModuleSet()
                break;

            case DataDirectionEnum.Output:
                moduleSet = this.GetOutputModuleSet()
                break;
        }

        remainingBytes = this.MaxBytes() - moduleSet.map(oneDasModule => oneDasModule.GetByteCount()).reduce((previousValue, currentValue) => previousValue + currentValue, 0)

        this.RemainingBytes(remainingBytes)
        this.RemainingCount(Math.floor(this.RemainingBytes() / ((this.NewModule().DataType() & 0x0FF) / 8)))
    }

    protected Validate()
    {
        this.ErrorMessage("")

        if (this.NewModule().HasError())
        {
            this.ErrorMessage("Resolve all remaining module errors before continuing.")
        }

        if (this.OneDasModuleSelectorMode() === OneDasModuleSelectorModeEnum.InputOnly && this.NewModule().DataDirection() == DataDirectionEnum.Output)
        {
            this.ErrorMessage("Only input modules are allowed.")
        }

        if (this.OneDasModuleSelectorMode() === OneDasModuleSelectorModeEnum.OutputOnly && this.NewModule().DataDirection() == DataDirectionEnum.Input)
        {
            this.ErrorMessage("Only output modules are allowed.")
        }

        if (isFinite(this.RemainingBytes()) && (this.RemainingBytes() - this.NewModule().GetByteCount() < 0))
        {
            this.ErrorMessage("Byte count of new module is too high.")
        }

        if (this.RemainingCount() <= 0)
        {
            this.ErrorMessage("The maximum number of modules is reached.")
        }
    }

    protected CreateNewModule()
    {
        if (this.NewModule())
        {
            return new OneDasModuleViewModel(new OneDasModuleModel(this.NewModule().DataType(), this.NewModule().DataDirection(), this.NewModule().Endianness(), 1))
        }
        else
        {
            return new OneDasModuleViewModel(new OneDasModuleModel(OneDasDataTypeEnum.UINT16, DataDirectionEnum.Input, EndiannessEnum.LittleEndian, 1))
        }
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
        this.InternalUpdate()
    }

    // commands
    public AddModule = () =>
    {
        let newModule: OneDasModuleViewModel

        if (!this.HasError())
        {
            this.ModuleSet.push(this.NewModule())
            this.InternalCreateNewModule()
            this.InternalUpdate()
            this._onModuleSetChanged.dispatch(this, this.ModuleSet())
        }
    }

    public DeleteModule = () =>
    {
        this.ModuleSet.pop()
        this.InternalUpdate()
        this._onModuleSetChanged.dispatch(this, this.ModuleSet())
    }
}