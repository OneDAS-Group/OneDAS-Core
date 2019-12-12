class ModbusClientViewModel extends ExtendedDataGatewayViewModelBase
{
    public UnitIdentifier: KnockoutObservable<number>
    public FrameRateDivider: KnockoutObservable<number>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification, new ModbusClientModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(modbusModuleModel => new ModbusClientModuleViewModel(modbusModuleModel))))

        EnumerationHelper.Description["ModbusObjectTypeEnum_DiscreteInput"] = "Discrete Input (R)"
        EnumerationHelper.Description["ModbusObjectTypeEnum_Coil"] = "Coil (RW)"
        EnumerationHelper.Description["ModbusObjectTypeEnum_InputRegister"] = "Input Register (R)"
        EnumerationHelper.Description["ModbusObjectTypeEnum_HoldingRegister"] = "Holding Register (RW)"

        this.UnitIdentifier = ko.observable<number>(model.UnitIdentifier)
        this.FrameRateDivider = ko.observable<number>(model.FrameRateDivider)
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.UnitIdentifier = <number>this.UnitIdentifier()
        model.FrameRateDivider = <number>this.FrameRateDivider()
        model.ModuleSet = <ModbusModuleModel[]>this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel())
    }
}