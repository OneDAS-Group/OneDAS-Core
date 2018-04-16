let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new DataGatewaySampleViewModel(model, identification)

class DataGatewaySampleViewModel extends ExtendedDataGatewayViewModelBase
{
    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification, new OneDasModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(oneDasModuleModel => new OneDasModuleViewModel(oneDasModuleModel))))
    }

    public ExtendModel(model)
    {
        super.ExtendModel(model);

        model.ModuleSet = <OneDasModuleModel[]>this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel())
    }
}