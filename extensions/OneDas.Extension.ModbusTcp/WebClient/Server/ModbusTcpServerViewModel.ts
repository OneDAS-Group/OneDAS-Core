let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new ModbusTcpServerViewModel(model, identification)

class ModbusTcpServerViewModel extends ExtendedDataGatewayViewModelBase
{
    public LocalIpAddress: KnockoutObservable<string>
    public Port: KnockoutObservable<number>
    public FrameRateDivider: KnockoutObservable<number>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification, new ModbusTcpServerModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(modbusTcpModuleModel => new ModbusTcpServerModuleViewModel(modbusTcpModuleModel))))

        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_DiscreteInput"] = "Discrete Input (R)"
        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_Coil"] = "Coil (RW)"
        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_InputRegister"] = "Input Register (R)"
        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_HoldingRegister"] = "Holding Register (RW)"

        this.LocalIpAddress = ko.observable<string>(model.LocalIpAddress)
        this.Port = ko.observable<number>(model.Port)
        this.FrameRateDivider = ko.observable<number>(model.FrameRateDivider)
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.LocalIpAddress = <string>this.LocalIpAddress()
        model.Port = <number>this.Port()
        model.FrameRateDivider = <number>this.FrameRateDivider()
        model.ModuleSet = <OneDasModuleModel[]>this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel())
    }
}