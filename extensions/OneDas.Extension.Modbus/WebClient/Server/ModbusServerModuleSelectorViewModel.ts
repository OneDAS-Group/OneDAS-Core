class ModbusServerModuleSelectorViewModel extends OneDasModuleSelectorViewModel
{
    constructor(oneDasModuleSelectorMode: OneDasModuleSelectorModeEnum, moduleSet: OneDasModuleViewModel[])
    {
        super(oneDasModuleSelectorMode, moduleSet)
        
        this.SettingsTemplateName = ko.observable("ModbusTcp_OneDasModuleSettingsTemplate")
    }

    protected CreateNewModule()
    {
        return new ModbusServerModuleViewModel(new ModbusModuleModel(0, ModbusObjectTypeEnum.HoldingRegister, OneDasDataTypeEnum.UINT16, DataDirectionEnum.Input, EndiannessEnum.BigEndian, 1))
    }
}