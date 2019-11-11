let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new CanRawViewModel(model, identification)

class CanRawViewModel extends ExtendedDataGatewayViewModelBase
{
    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification, new OneDasModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(oneDasModuleModel => new OneDasModuleViewModel(oneDasModuleModel))))

        this.OneDasModuleSelector.SetMaxBytes(128)
    }

    // methods
    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.ModuleSet = <OneDasModuleModel[]>this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel())
    }
}