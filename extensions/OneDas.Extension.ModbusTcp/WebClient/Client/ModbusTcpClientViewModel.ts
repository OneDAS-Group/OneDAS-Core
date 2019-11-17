let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new ModbusTcpClientViewModel(model, identification)

class ModbusTcpClientViewModel extends ExtendedDataGatewayViewModelBase
{
    public RemoteIpAddress: KnockoutObservable<string>
    public Port: KnockoutObservable<number>
    public UnitIdentifier: KnockoutObservable<number>
    public FrameRateDivider: KnockoutObservable<number>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification, new ModbusTcpClientModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(modbusTcpModuleModel => new ModbusTcpClientModuleViewModel(modbusTcpModuleModel))))

        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_DiscreteInput"] = "Discrete Input (R)"
        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_Coil"] = "Coil (RW)"
        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_InputRegister"] = "Input Register (R)"
        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_HoldingRegister"] = "Holding Register (RW)"

        this.RemoteIpAddress = ko.observable<string>(model.RemoteIpAddress)
        this.Port = ko.observable<number>(model.Port)
        this.UnitIdentifier = ko.observable<number>(model.UnitIdentifier)
        this.FrameRateDivider = ko.observable<number>(model.FrameRateDivider)
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.RemoteIpAddress = <string>this.RemoteIpAddress()
        model.Port = <number>this.Port()
        model.UnitIdentifier = <number>this.UnitIdentifier()
        model.FrameRateDivider = <number>this.FrameRateDivider()
        model.ModuleSet = <OneDasModuleModel[]>this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel())
    }
}