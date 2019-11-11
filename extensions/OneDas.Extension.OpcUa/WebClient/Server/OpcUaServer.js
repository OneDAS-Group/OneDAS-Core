let ViewModelConstructor = (model, identification) => new OpcUaServerViewModel(model, identification);
class OpcUaServerViewModel extends ExtendedDataGatewayViewModelBase {
    constructor(model, identification) {
        super(model, identification, new OneDasModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(module => new OneDasModuleViewModel(module))));
        this.LocalIpAddress = ko.observable(model.LocalIpAddress);
        this.FrameRateDivider = ko.observable(model.FrameRateDivider);
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.LocalIpAddress = this.LocalIpAddress();
        model.FrameRateDivider = this.FrameRateDivider();
        model.ModuleSet = this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel());
    }
}
