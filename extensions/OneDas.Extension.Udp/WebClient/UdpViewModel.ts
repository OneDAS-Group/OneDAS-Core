let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new UdpViewModel(model, identification)

class UdpViewModel extends ExtendedDataGatewayViewModelBase
{
    public RemoteIpAddress: KnockoutObservable<string>
    public LocalDataPort: KnockoutObservable<number>
    public RemoteDataPort: KnockoutObservable<number>
    public FrameRateDivider: KnockoutObservable<number>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification, new OneDasModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(oneDasModuleModel => new OneDasModuleViewModel(oneDasModuleModel))))

        this.OneDasModuleSelector().SetMaxBytes(1500)

        this.RemoteIpAddress = ko.observable<string>(model.RemoteIpAddress)
        this.LocalDataPort = ko.observable<number>(model.LocalDataPort)
        this.RemoteDataPort = ko.observable<number>(model.RemoteDataPort)
        this.FrameRateDivider = ko.observable<number>(model.FrameRateDivider)
    }

    // methods
    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.RemoteIpAddress = <string>this.RemoteIpAddress()
        model.LocalDataPort = <number>this.LocalDataPort()
        model.RemoteDataPort = <number>this.RemoteDataPort()
        model.FrameRateDivider = <number>this.FrameRateDivider()

        model.ModuleSet = <OneDasModuleModel[]>this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel())
    }
}