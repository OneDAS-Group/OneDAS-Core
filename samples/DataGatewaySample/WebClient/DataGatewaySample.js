let ViewModelConstructor = (model, identification) => new DataGatewaySampleViewModel(model, identification);
class DataGatewaySampleViewModel extends ExtendedDataGatewayViewModelBase {
    constructor(model, identification) {
        super(model, identification, new OneDasModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(oneDasModuleModel => new OneDasModuleViewModel(oneDasModuleModel))));
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.ModuleSet = this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel());
    }
}
