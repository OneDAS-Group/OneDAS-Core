abstract class DataWriterViewModelBase extends PluginViewModelBase
{
    public readonly FileGranularity: KnockoutObservable<FileGranularityEnum>

    constructor(model, identification: PluginIdentificationViewModel)
    {
        super(model, identification)

        this.FileGranularity = ko.observable<FileGranularityEnum>(model.FileGranularity)
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.FileGranularity = <FileGranularityEnum>this.FileGranularity()
    }
}