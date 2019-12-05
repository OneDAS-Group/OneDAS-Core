let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new ModbusRtuClientViewModel(model, identification)

class ModbusRtuClientViewModel extends ModbusClientViewModel
{
    public Port: KnockoutObservable<string>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification)

        this.Port = ko.observable<string>(model.Port)
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.Port = <string>this.Port()
    }
}