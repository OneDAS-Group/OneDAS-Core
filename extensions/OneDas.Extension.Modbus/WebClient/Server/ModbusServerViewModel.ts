abstract class ModbusServerViewModel extends ExtendedDataGatewayViewModelBase
{
    public FrameRateDivider: KnockoutObservable<number>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification, new ModbusServerModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(modbusModuleModel => new ModbusServerModuleViewModel(modbusModuleModel))))

        EnumerationHelper.Description["ModbusObjectTypeEnum_DiscreteInput"] = "Discrete Input (R)"
        EnumerationHelper.Description["ModbusObjectTypeEnum_Coil"] = "Coil (RW)"
        EnumerationHelper.Description["ModbusObjectTypeEnum_InputRegister"] = "Input Register (R)"
        EnumerationHelper.Description["ModbusObjectTypeEnum_HoldingRegister"] = "Holding Register (RW)"

        this.FrameRateDivider = ko.observable<number>(model.FrameRateDivider)
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.FrameRateDivider = <number>this.FrameRateDivider()
        model.ModuleSet = <ModbusModuleModel[]>this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel())
    }
}