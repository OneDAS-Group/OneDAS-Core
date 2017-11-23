class TransferFunctionViewModel implements IModelizable
{
    public DateTime: KnockoutObservable<string>
    public Type: KnockoutObservable<string>
    public Option: KnockoutObservable<string>
    public Argument: KnockoutObservable<string>

    constructor(transferFunctionModel: TransferFunctionModel)
    {
        this.DateTime = ko.observable(transferFunctionModel.DateTime)
        this.Type = ko.observable(transferFunctionModel.Type)
        this.Option = ko.observable(transferFunctionModel.Option)
        this.Argument = ko.observable(transferFunctionModel.Argument)
    }

    // methods
    public ToModel()
    {
        return new TransferFunctionModel(this.DateTime(), this.Type(), this.Option(), this.Argument())
    }
}