class CanModuleSelectorViewModel extends OneDasModuleSelectorViewModel
{
    constructor(oneDasModuleSelectorMode: OneDasModuleSelectorModeEnum, moduleSet: OneDasModuleViewModel[])
    {
        super(oneDasModuleSelectorMode, moduleSet)

        this.SettingsTemplateName = ko.observable("Can_OneDasModuleSettingsTemplate")
    }

    protected CreateNewModule()
    {
        return new CanModuleViewModel(new CanModuleModel(0, CanFrameFormatEnum.Standard, OneDasDataTypeEnum.INT32, DataDirectionEnum.Input, EndiannessEnum.BigEndian, 2))
    }
}