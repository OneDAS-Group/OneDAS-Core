/// <reference path="PluginViewModelBase.ts"/>

abstract class DataWriterViewModelBase extends PluginViewModelBase
{
    public readonly FileGranularity: KnockoutObservable<FileGranularityEnum>
    public readonly BufferRequestSet: KnockoutObservableArray<BufferRequestViewModel>
    public readonly BufferRequestSelector: KnockoutObservable<BufferRequestSelectorViewModel>

    constructor(model, identification: PluginIdentificationViewModel)
    {
        super(model, identification)

        this.FileGranularity = ko.observable<FileGranularityEnum>(model.FileGranularity)
        this.BufferRequestSet = ko.observableArray<BufferRequestViewModel>(model.BufferRequestSet.map(bufferRequest => new BufferRequestViewModel(bufferRequest)))

        this.BufferRequestSelector = ko.observable<BufferRequestSelectorViewModel>(new BufferRequestSelectorViewModel(this.BufferRequestSet()))
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.FileGranularity = <FileGranularityEnum>this.FileGranularity()
        model.BufferRequestSet = <BufferRequestModel[]>this.BufferRequestSet().map(bufferRequest => bufferRequest.ToModel())
    }
}