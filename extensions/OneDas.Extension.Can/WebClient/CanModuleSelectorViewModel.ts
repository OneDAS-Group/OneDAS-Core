class CanModuleSelectorViewModel extends OneDasModuleSelectorViewModel
{
    constructor(oneDasModuleSelectorMode: OneDasModuleSelectorModeEnum, moduleSet: CanModuleViewModel[])
    {
        super(oneDasModuleSelectorMode, moduleSet)

        this.SettingsTemplateName = ko.observable("Can_CanModuleSettingsTemplate")
    }
    
    protected CreateNewModule()
    {
        return new CanModuleViewModel(new CanModuleModel(0, CanFrameFormatEnum.Standard, OneDasDataTypeEnum.INT32, DataDirectionEnum.Input, EndiannessEnum.LittleEndian, 2))
    }
}