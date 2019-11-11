var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
let ViewModelConstructor = (model, identification) => new Mat73ViewModel(model, identification);
class Mat73ViewModel extends DataWriterViewModelBase {
    constructor(model, identification) {
        super(model, identification);
    }
    // methods
    InitializeAsync() {
        return __awaiter(this, void 0, void 0, function* () {
            //
        });
    }
    ExtendModel(model) {
        super.ExtendModel(model);
    }
}
