class BufferRequestViewModel
{
    public SampleRate: KnockoutObservable<SampleRateEnum>
    public GroupFilter: KnockoutObservable<string>

    constructor(model: BufferRequestModel)
    {
        this.SampleRate = ko.observable<SampleRateEnum>(model.SampleRate);
        this.GroupFilter = ko.observable<string>(model.GroupFilter);
    }

    public ToModel() {
        return {
            SampleRate: <SampleRateEnum>this.SampleRate(),
            GroupFilter: <string>this.GroupFilter()
        }
    }
}