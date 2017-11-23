abstract class DataPortViewModelBase
{
    // fields
    public Name: KnockoutObservable<string>
    public readonly OneDasDataType: OneDasDataTypeEnum
    public readonly DataDirection: DataDirectionEnum

    public IsSelected: KnockoutObservable<boolean>
    public AssociatedChannelHubSet: KnockoutObservableArray<ChannelHubViewModel>
    public readonly AssociatedDataGateway: DataGatewayViewModelBase
    public readonly LiveDescription: KnockoutComputed<string>

    // constructors
    constructor(name: string, oneDasDataType: OneDasDataTypeEnum, dataDirection: DataDirectionEnum, associatedDataGateway: DataGatewayViewModelBase)
    {
        this.Name = ko.observable(name)
        this.OneDasDataType = oneDasDataType
        this.DataDirection = dataDirection

        this.IsSelected = ko.observable<boolean>(false)
        this.AssociatedChannelHubSet = ko.observableArray<ChannelHubViewModel>()
        this.AssociatedDataGateway = associatedDataGateway

        this.LiveDescription = ko.computed(() =>
        {
            let result: string

            result = "<div class='text-left'>" + this.Name() + "</div><div class='text-left'>" + EnumerationHelper.GetEnumLocalization('OneDasDataTypeEnum', this.OneDasDataType) + "</div>"

            if (this.AssociatedChannelHubSet().length > 0)
            {
                this.AssociatedChannelHubSet().forEach(channelHub =>
                {
                    result += "</br ><div class='text-left'>" + channelHub.Name() + "</div><div class='text-left'>" + EnumerationHelper.GetEnumLocalization('OneDasDataTypeEnum', channelHub.OneDasDataType()) + "</div>"
                })
            }

            return result
        })
    }

    // methods
    abstract GetId(): string

    public ToFullQualifiedIdentifier(): string
    {
        return this.AssociatedDataGateway.Description.Id + " (" + this.AssociatedDataGateway.Description.InstanceId + ") / " + this.GetId();
    }

    public ResetAssociations(maintainWeakReference: boolean)
    {
        if (this.AssociatedChannelHubSet().length > 0)
        {
            this.AssociatedChannelHubSet().forEach(channelHub => channelHub.ResetAssociation(maintainWeakReference, this))
        }
    }
}