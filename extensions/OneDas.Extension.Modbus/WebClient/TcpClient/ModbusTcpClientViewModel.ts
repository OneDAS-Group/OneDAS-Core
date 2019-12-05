let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new ModbusTcpClientViewModel(model, identification)

class ModbusTcpClientViewModel extends ModbusClientViewModel
{
    public RemoteIpAddress: KnockoutObservable<string>
    public Port: KnockoutObservable<number>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification)

        this.RemoteIpAddress = ko.observable<string>(model.RemoteIpAddress)
        this.Port = ko.observable<number>(model.Port)
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.RemoteIpAddress = <string>this.RemoteIpAddress()
        model.Port = <number>this.Port()
    }
}