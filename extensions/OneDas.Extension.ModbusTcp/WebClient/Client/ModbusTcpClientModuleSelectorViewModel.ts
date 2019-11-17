class ModbusTcpClientModuleSelectorViewModel extends OneDasModuleSelectorViewModel
{
    constructor(oneDasModuleSelectorMode: OneDasModuleSelectorModeEnum, moduleSet: OneDasModuleViewModel[])
    {
        super(oneDasModuleSelectorMode, moduleSet)

        this.SettingsTemplateName = ko.observable("ModbusTcp_OneDasModuleSettingsTemplate")
    }

    protected CreateNewModule()
    {
        return new ModbusTcpClientModuleViewModel(new ModbusTcpModuleModel(0, ModbusTcpObjectTypeEnum.HoldingRegister, OneDasDataTypeEnum.UINT16, DataDirectionEnum.Input, EndiannessEnum.BigEndian, 1))
    }
}