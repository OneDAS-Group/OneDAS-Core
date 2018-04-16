/// <reference path="PluginViewModelBase.ts"/>

abstract class DataGatewayViewModelBase extends PluginViewModelBase
{
    public readonly MaximumDatasetAge: KnockoutObservable<number>
    public readonly DataPortSet: KnockoutObservableArray<DataPortViewModel>

    constructor(model, identification: PluginIdentificationViewModel)
    {
        super(model, identification)

        this.MaximumDatasetAge = ko.observable<number>(model.MaximumDatasetAge)
        this.DataPortSet = ko.observableArray<DataPortViewModel>()
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.MaximumDatasetAge = <number>Number.parseInt(<any>this.MaximumDatasetAge())
    }
}