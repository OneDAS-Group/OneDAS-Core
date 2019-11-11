let ViewModelConstructor = (model, identification) => new CanRawViewModel(model, identification);
class CanRawViewModel extends ExtendedDataGatewayViewModelBase {
    constructor(model, identification) {
        super(model, identification, new OneDasModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(oneDasModuleModel => new OneDasModuleViewModel(oneDasModuleModel))));
        this.OneDasModuleSelector.SetMaxBytes(128);
    }
    // methods
    ExtendModel(model) {
        super.ExtendModel(model);
        model.ModuleSet = this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel());
    }
}
