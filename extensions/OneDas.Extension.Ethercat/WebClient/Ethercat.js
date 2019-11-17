var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
let ViewModelConstructor = (model, identification) => new EthercatViewModel(model, identification);
class EthercatViewModel extends DataGatewayViewModelBase {
    constructor(model, identification) {
        super(model, identification);
        this.GetDynamicSlaveInfoDataAsync = (slaveInfo) => __awaiter(this, void 0, void 0, function* () {
            let actionResponse;
            let slaveInfoDynamicDataModel;
            actionResponse = yield this.SendActionRequest(0, "GetDynamicSlaveInfoData", slaveInfo.ToFlatModel());
            slaveInfoDynamicDataModel = actionResponse.Data;
            return new SlaveInfoDynamicDataViewModel(slaveInfoDynamicDataModel, slaveInfo, this);
        });
        // commands
        this.SelectSlaveInfo = (item) => {
            this.SelectedSlaveInfo(item);
        };
        this.GetAvailableNetworkInterfaces = () => __awaiter(this, void 0, void 0, function* () {
            let actionResponse;
            let dictionary;
            try {
                actionResponse = yield this.SendActionRequest(0, "GetAvailableNetworkInterfaces", null);
                dictionary = actionResponse.Data;
                this.NetworkInterfaceDescriptionSet.removeAll();
                for (var key in dictionary) {
                    if (key !== "$type") {
                        this.NetworkInterfaceDescriptionSet.push(new NetworkInterfaceDescription(key, dictionary[key]));
                    }
                }
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.OpenSlaveSettings = (slaveInfo) => {
            this.SelectedSlaveInfo(slaveInfo);
        };
        this.SelectNetworkAdapter = (networkInterfaceDescription) => {
            this.NicHardwareAddress(networkInterfaceDescription.Address);
        };
        this.UpdateSlaveInfo = (slaveInfo) => __awaiter(this, void 0, void 0, function* () {
            slaveInfo.DynamicData(yield this.GetDynamicSlaveInfoDataAsync(slaveInfo));
            this.DataPortSet(MapMany(this.RootSlaveInfo().GetDescendants(false), (slaveInfo) => slaveInfo.GetVariables()));
            this.SelectedSlaveInfo(null);
        });
        this.ReloadHardware = () => __awaiter(this, void 0, void 0, function* () {
            if (confirm("The terminals will be reloaded. In case of removed terminals, the corresponding mappings will be deleted (but can be restored once the terminal is reconnected). Proceed?") === true) {
                let data;
                let actionResponse;
                let slaveInfoModel;
                if (this.RootSlaveInfo()) {
                    data = { NicHardwareAddress: this.NicHardwareAddress(), RootSlaveInfo: this.RootSlaveInfo().ToModel() };
                }
                else {
                    data = { NicHardwareAddress: this.NicHardwareAddress(), RootSlaveInfo: null };
                }
                try {
                    actionResponse = yield this.SendActionRequest(0, "ReloadHardware", data);
                    slaveInfoModel = actionResponse.Data;
                    this.RootSlaveInfo(new SlaveInfoViewModel(slaveInfoModel));
                }
                catch (e) {
                    alert(e.message);
                }
            }
        });
        this.NetworkInterfaceDescriptionSet = ko.observableArray();
        this.NicHardwareAddress = ko.observable(model.NicHardwareAddress);
        this.RootSlaveInfo = ko.observable();
        this.SelectedSlaveInfo = ko.observable();
        if (model.RootSlaveInfo) {
            this.RootSlaveInfo(new SlaveInfoViewModel(model.RootSlaveInfo));
        }
        this.RootSlaveInfo.subscribe((newValue) => __awaiter(this, void 0, void 0, function* () {
            yield this.InitializeAsync();
        }));
    }
    // methods
    InitializeAsync() {
        return __awaiter(this, void 0, void 0, function* () {
            let modifiedSlaveInfoSet;
            let extensionIdentificationModelSet;
            let actionResponse;
            if (!ExtensionHive.ExtensionIdentificationSet.has("EthercatSlaveExtension")) {
                actionResponse = yield this.SendActionRequest(0, "GetExtensionIdentifications", null);
                extensionIdentificationModelSet = actionResponse.Data.$values;
                ExtensionHive.ExtensionIdentificationSet.set("EthercatSlaveExtension", extensionIdentificationModelSet.map(x => new ExtensionIdentificationViewModel(x)));
                ExtensionHive.ExtensionIdentificationSet.get("EthercatSlaveExtension").forEach(extensionIdentification => {
                    ko.components.register(extensionIdentification.Id, {
                        template: {
                            ExtensionType: "EthercatSlaveExtension", ExtensionIdentification: extensionIdentification
                        },
                        viewModel: {
                            createViewModel: (params, componentInfo) => {
                                return params.GetEthercatSlaveExtensionCallback(params.Index);
                            }
                        }
                    });
                });
            }
            if (this.RootSlaveInfo()) {
                yield Promise.all(this.RootSlaveInfo().GetDescendants(true).map((slaveInfo, index) => __awaiter(this, void 0, void 0, function* () {
                    yield slaveInfo.InitializeAsync();
                    slaveInfo.DynamicData(yield this.GetDynamicSlaveInfoDataAsync(slaveInfo));
                })));
                // find modified slaves
                modifiedSlaveInfoSet = this.RootSlaveInfo().GetDescendants(false).filter(slaveInfo => slaveInfo.GetOldCsa() !== slaveInfo.Csa);
                if (modifiedSlaveInfoSet.length > 0) {
                    let message = "The IDs of the following terminals have been changed because they were invalid:\n\n";
                    modifiedSlaveInfoSet.forEach(slaveInfo => {
                        message = message + slaveInfo.DynamicData().Name + " - old ID: " + slaveInfo.GetOldCsa() + " - new ID: " + slaveInfo.Csa + "\n";
                        slaveInfo.ResetOldCsa();
                    });
                    alert(message);
                }
                this.DataPortSet(MapMany(this.RootSlaveInfo().GetDescendants(false), (x) => x.GetVariables()));
            }
            else {
                this.DataPortSet([]);
            }
        });
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.NicHardwareAddress = this.NicHardwareAddress();
        if (this.RootSlaveInfo()) {
            model.RootSlaveInfo = this.RootSlaveInfo().ToModel();
        }
        else {
            model.RootSlaveInfo = null;
        }
    }
}
class NetworkInterfaceDescription {
    constructor(name, address) {
        this.Name = name;
        this.Address = address;
    }
}
class SlaveExtensionViewModelBase extends ExtensionViewModelBase {
}
window[SlaveExtensionViewModelBase.name] = SlaveExtensionViewModelBase;
class SlaveInfoDynamicDataViewModel {
    constructor(slaveInfoDynamicDataModel, parent, dataGateway) {
        this.Name = slaveInfoDynamicDataModel.Name;
        this.Description = slaveInfoDynamicDataModel.Description;
        this.Base64ImageData = slaveInfoDynamicDataModel.Base64ImageData;
        this.PdoSet = slaveInfoDynamicDataModel.PdoSet.filter(slavePdoModel => slavePdoModel.SyncManager >= 0).map(slavePdoModel => new SlavePdoViewModel(slavePdoModel, parent, dataGateway));
    }
}
class SlaveInfoViewModel {
    // constructors
    constructor(slaveInfoModel) {
        this.ToModel = () => {
            return {
                Manufacturer: this.Manufacturer,
                ProductCode: this.ProductCode,
                Revision: this.Revision,
                OldCsa: this._oldCsa,
                Csa: this.Csa,
                ChildSet: this.ChildSet.map(slaveInfo => slaveInfo.ToModel()),
                SlaveExtensionSet: this._slaveExtensionSet.map(slaveExtension => slaveExtension.ToModel())
            };
        };
        this.GetOldCsa = () => {
            return this._oldCsa;
        };
        this.ResetOldCsa = () => {
            this._oldCsa = this.Csa;
        };
        this.GetDescendants = (includeSelf) => {
            let descendantSet;
            descendantSet = this.InternalGetDescendants();
            if (includeSelf) {
                descendantSet.unshift(this);
            }
            return descendantSet;
        };
        this.InternalGetDescendants = () => {
            let descendantSet = [];
            this.ChildSet.forEach(x => {
                descendantSet.push(x);
                descendantSet.push(...x.InternalGetDescendants());
            });
            return descendantSet;
        };
        this.GetVariables = () => {
            if (this.DynamicData()) {
                return MapMany(this.DynamicData().PdoSet, (x) => x.VariableSet);
            }
            else {
                return [];
            }
        };
        this.GetEthercatSlaveExtension = (index) => {
            return this.SlaveExtensionSet[index];
        };
        this.ToFlatModel = () => {
            return {
                Manufacturer: this.Manufacturer,
                ProductCode: this.ProductCode,
                Revision: this.Revision,
                OldCsa: this._oldCsa,
                Csa: this.Csa,
                ChildSet: [],
                SlaveExtensionSet: this._slaveExtensionSet.map(slaveExtension => slaveExtension.ToModel())
            };
        };
        this.Manufacturer = slaveInfoModel.Manufacturer;
        this.ProductCode = slaveInfoModel.ProductCode;
        this.Revision = slaveInfoModel.Revision;
        this.Csa = slaveInfoModel.Csa;
        this.ChildSet = slaveInfoModel.ChildSet.map(x => new SlaveInfoViewModel(x));
        this._oldCsa = slaveInfoModel.OldCsa;
        this._slaveExtensionModelSet = slaveInfoModel.SlaveExtensionSet;
        this.DynamicData = ko.observable();
    }
    // properties
    get SlaveExtensionSet() {
        return this._slaveExtensionSet;
    }
    // methods
    InitializeAsync() {
        return __awaiter(this, void 0, void 0, function* () {
            this._slaveExtensionSet = yield Promise.all(this._slaveExtensionModelSet.map((slaveExtensionModel) => __awaiter(this, void 0, void 0, function* () {
                yield ExtensionFactory.CreateExtensionViewModelAsync("EthercatSlaveExtension", slaveExtensionModel);
                return (yield ExtensionFactory.CreateExtensionViewModelAsync("EthercatSlaveExtension", slaveExtensionModel));
            })));
            this._slaveExtensionSet.forEach(model => model.SlaveInfo = this);
            yield Promise.all(this._slaveExtensionSet.map(slaveExtension => slaveExtension.InitializeAsync()));
        });
    }
}
class SlavePdoViewModel {
    constructor(slavePdoModel, parent, dataGateway) {
        this.Parent = parent;
        this.Name = slavePdoModel.Name;
        this.Index = slavePdoModel.Index;
        this.SyncManager = slavePdoModel.SyncManager;
        this.VariableSet = slavePdoModel.VariableSet.map(x => new SlaveVariableViewModel(x, this, dataGateway));
        this.CompactView = ko.observable(this.VariableSet.length === 1);
    }
}
class SlaveVariableViewModel extends DataPortViewModel {
    constructor(slaveVariableModel, parent, dataGateway) {
        super(slaveVariableModel, dataGateway);
        this.Parent = parent;
        this.Index = slaveVariableModel.Index;
        this.SubIndex = slaveVariableModel.SubIndex;
    }
    GetId() {
        return this.Parent.Parent.Csa + " / " + this.Parent.Name + " / " + this.Name();
    }
}
