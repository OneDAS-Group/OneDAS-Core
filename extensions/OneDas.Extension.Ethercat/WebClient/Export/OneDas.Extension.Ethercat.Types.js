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
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
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
        this.GetChannels = () => {
            if (this.DynamicData()) {
                return MapMany(this.DynamicData().PdoSet, (x) => x.ChannelSet);
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
        this.ChannelSet = slavePdoModel.ChannelSet.map(x => new SlaveChannelViewModel(x, this, dataGateway));
        this.CompactView = ko.observable(this.ChannelSet.length === 1);
    }
}
class SlaveChannelViewModel extends DataPortViewModel {
    constructor(slaveChannelModel, parent, dataGateway) {
        super(slaveChannelModel, dataGateway);
        this.Parent = parent;
        this.Index = slaveChannelModel.Index;
        this.SubIndex = slaveChannelModel.SubIndex;
    }
    GetId() {
        return this.Parent.Parent.Csa + " / " + this.Parent.Name + " / " + this.Name();
    }
}
