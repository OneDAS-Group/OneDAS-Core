abstract class OneDasModuleSelectorViewModelBase
{
    public AllowInputs: KnockoutObservable<boolean>
    public AllowOutputs: KnockoutObservable<boolean>
    public AllowBoolean: KnockoutObservable<boolean>

    public InputSettingsTemplateName: KnockoutObservable<string>
    public OutputSettingsTemplateName: KnockoutObservable<string>

    public NewInputModule: KnockoutObservable<OneDasModuleViewModel>
    public NewOutputModule: KnockoutObservable<OneDasModuleViewModel>

    public InputRemainingBytes: KnockoutObservable<number>
    public OutputRemainingBytes: KnockoutObservable<number>

    public InputRemainingCount: KnockoutObservable<number>
    public OutputRemainingCount: KnockoutObservable<number>

    public InputModuleSet: KnockoutObservableArray<OneDasModuleViewModel>
    public OutputModuleSet: KnockoutObservableArray<OneDasModuleViewModel>

    private _onInputModuleSetChanged: EventDispatcher<OneDasModuleSelectorViewModelBase, OneDasModuleViewModel[]>
    private _onOutputModuleSetChanged: EventDispatcher<OneDasModuleSelectorViewModelBase, OneDasModuleViewModel[]>

    constructor(allowInputs: boolean, allowOutputs: boolean, allowBoolean: boolean)
    {
        this.AllowInputs = ko.observable(allowInputs)
        this.AllowOutputs = ko.observable(allowOutputs)
        this.AllowBoolean = ko.observable(allowBoolean)

        this.InputSettingsTemplateName = ko.observable("Project_OneDasModuleSelectorInputSettingsTemplate")
        this.OutputSettingsTemplateName = ko.observable("Project_OneDasModuleSelectorOutputSettingsTemplate")

        this.NewInputModule = ko.observable<OneDasModuleViewModel>();
        this.NewOutputModule = ko.observable<OneDasModuleViewModel>();

        this.InputRemainingBytes = ko.observable<number>(NaN);
        this.OutputRemainingBytes = ko.observable<number>(NaN);

        this.InputRemainingCount = ko.observable<number>(NaN);
        this.OutputRemainingCount = ko.observable<number>(NaN);

        this.InputModuleSet = ko.observableArray<OneDasModuleViewModel>();
        this.OutputModuleSet = ko.observableArray<OneDasModuleViewModel>();

        this.InputModuleSet.subscribe(newValue => this.Update())
        this.OutputModuleSet.subscribe(newValue => this.Update())

        this.InternalCreateNewInputModule()
        this.InternalCreateNewOutputModule()

        this._onInputModuleSetChanged = new EventDispatcher<OneDasModuleSelectorViewModelBase, OneDasModuleViewModel[]>();
        this._onOutputModuleSetChanged = new EventDispatcher<OneDasModuleSelectorViewModelBase, OneDasModuleViewModel[]>();

        this.Update()
    }

    get OnInputModuleSetChanged(): IEvent<OneDasModuleSelectorViewModelBase, OneDasModuleViewModel[]>
    {
        return this._onInputModuleSetChanged;
    }

    get OnOutputModuleSetChanged(): IEvent<OneDasModuleSelectorViewModelBase, OneDasModuleViewModel[]>
    {
        return this._onOutputModuleSetChanged;
    }

    // methods
    public abstract Update(): void

    public InternalCreateNewInputModule()
    {
        if (this.NewInputModule())
        {
            this.NewInputModule().OnPropertyChanged.unsubscribe(this.OnModulePropertyChanged)
        }

        this.NewInputModule(this.CreateNewInputModule())
        this.NewInputModule().OnPropertyChanged.subscribe(this.OnModulePropertyChanged)
    }

    public CreateNewInputModule()
    {
        return new OneDasModuleViewModel(new OneDasModuleModel(OneDasDataTypeEnum.UINT16, DataDirectionEnum.Input, 1))
    }

    public InternalCreateNewOutputModule()
    {
        if (this.NewOutputModule())
        {
            this.NewOutputModule().OnPropertyChanged.unsubscribe(this.OnModulePropertyChanged)
        }

        this.NewOutputModule(this.CreateNewOutputModule())
        this.NewOutputModule().OnPropertyChanged.subscribe(this.OnModulePropertyChanged)
    }

    public CreateNewOutputModule()
    {
        return new OneDasModuleViewModel(new OneDasModuleModel(OneDasDataTypeEnum.UINT16, DataDirectionEnum.Output, 1))
    }

    private OnModulePropertyChanged = () =>
    {
        this.Update()
    }

    // commands
    public AddInputModule = () =>
    {
        if (this.AllowInputs())
        {
            this.CheckDataType(this.NewInputModule().DataType())

            if (Number.isNaN(this.NewInputModule().Size()) || this.NewInputModule().Size() <= this.InputRemainingCount())
            {
                this.InputModuleSet.push(this.NewInputModule())
                this.InternalCreateNewInputModule()
            }

            this._onInputModuleSetChanged.dispatch(this, this.InputModuleSet())
        }
        else
        {
            throw new Error("Input modules are disabled.")
        }
    }

    public DeleteInputModule = () =>
    {
        this.InputModuleSet.pop()
        this.Update()
        this._onInputModuleSetChanged.dispatch(this, this.InputModuleSet())
    }

    public AddOutputModule = () =>
    {
        if (this.AllowOutputs())
        {
            this.CheckDataType(this.NewOutputModule().DataType())

            if (Number.isNaN(this.NewOutputModule().Size()) || this.NewOutputModule().Size() <= this.OutputRemainingCount())
            {
                this.OutputModuleSet.push(this.NewOutputModule())
                this.InternalCreateNewOutputModule()
            }

            this._onOutputModuleSetChanged.dispatch(this, this.OutputModuleSet())
        }
        else
        {
            throw new Error("Outputs modules are disabled.")
        }
    }

    public DeleteOutputModule = () =>
    {
        this.OutputModuleSet.pop()
        this.Update()
        this._onOutputModuleSetChanged.dispatch(this, this.OutputModuleSet())
    }

    public CheckDataType(oneDasDataType: OneDasDataTypeEnum)
    {
        if (!this.AllowBoolean() && oneDasDataType === OneDasDataTypeEnum.BOOLEAN)
        {
            throw new Error("Wrong data direction of module.")
        }
    }
}