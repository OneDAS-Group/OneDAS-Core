let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new ModbusTcpServerViewModel(model, identification)

class ModbusTcpServerViewModel extends ModbusServerViewModel
{
    public LocalIpAddress: KnockoutObservable<string>
    public Port: KnockoutObservable<number>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification)

        this.LocalIpAddress = ko.observable<string>(model.LocalIpAddress)
        this.Port = ko.observable<number>(model.Port)
    }
    
    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.LocalIpAddress = <string>this.LocalIpAddress()
        model.Port = <number>this.Port()
    }
}