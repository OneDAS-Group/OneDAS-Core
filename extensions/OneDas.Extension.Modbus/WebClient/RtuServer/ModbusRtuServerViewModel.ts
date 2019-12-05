let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new ModbusRtuServerViewModel(model, identification)

class ModbusRtuServerViewModel extends ModbusServerViewModel
{
    public UnitIdentifier: KnockoutObservable<number>
    public Port: KnockoutObservable<string>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification)

        this.UnitIdentifier = ko.observable<number>(model.UnitIdentifier)
        this.Port = ko.observable<string>(model.Port)
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.LocalIpAddress = <number>this.UnitIdentifier()
        model.Port = <string>this.Port()
    }
}