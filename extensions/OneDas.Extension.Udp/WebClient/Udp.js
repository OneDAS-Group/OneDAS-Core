let ViewModelConstructor = (model, identification) => new UdpViewModel(model, identification);
class UdpViewModel extends ExtendedDataGatewayViewModelBase {
    constructor(model, identification) {
        super(model, identification, new OneDasModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(oneDasModuleModel => new OneDasModuleViewModel(oneDasModuleModel))));
        this.OneDasModuleSelector().SetMaxBytes(1500);
        this.RemoteIpAddress = ko.observable(model.RemoteIpAddress);
        this.LocalDataPort = ko.observable(model.LocalDataPort);
        this.RemoteDataPort = ko.observable(model.RemoteDataPort);
        this.FrameRateDivider = ko.observable(model.FrameRateDivider);
    }
    // methods
    ExtendModel(model) {
        super.ExtendModel(model);
        model.RemoteIpAddress = this.RemoteIpAddress();
        model.LocalDataPort = this.LocalDataPort();
        model.RemoteDataPort = this.RemoteDataPort();
        model.FrameRateDivider = this.FrameRateDivider();
        model.ModuleSet = this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel());
    }
}
