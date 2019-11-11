let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new OpcUaServerViewModel(model, identification)

class OpcUaServerViewModel extends ExtendedDataGatewayViewModelBase
{
    public LocalIpAddress: KnockoutObservable<string>
    public FrameRateDivider: KnockoutObservable<number>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification, new OneDasModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(module => new OneDasModuleViewModel(module))))

        this.LocalIpAddress = ko.observable<string>(model.LocalIpAddress)
        this.FrameRateDivider = ko.observable<number>(model.FrameRateDivider)
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)
        
        model.LocalIpAddress = <string>this.LocalIpAddress()
        model.FrameRateDivider = <number>this.FrameRateDivider()
        model.ModuleSet = <OneDasModuleModel[]>this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel())
    }
}