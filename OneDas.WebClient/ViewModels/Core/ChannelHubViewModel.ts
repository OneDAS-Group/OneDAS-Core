class ChannelHubViewModel
{
    public Name: KnockoutObservable<string>
    public Group: KnockoutObservable<string>
    public readonly OneDasDataType: KnockoutObservable<OneDasDataTypeEnum>
    public readonly SampleRate: KnockoutObservable<SampleRateEnum>
    public readonly Guid: string
    public readonly CreationDateTime: string
    public readonly Unit: KnockoutObservable<string>
    public readonly TransferFunctionSet: KnockoutObservableArray<TransferFunctionViewModel>
    public SelectedTransferFunction: KnockoutObservable<TransferFunctionViewModel>
    public EvaluatedTransferFunctionSet: ((value: number) => number)[]
    public IsSelected: KnockoutObservable<boolean>

    public readonly AssociatedDataInput: KnockoutObservable<DataPortViewModel>
    public readonly AssociatedDataOutputSet: KnockoutObservableArray<DataPortViewModel>

    private AssociatedDataInputId: string
    private AssociatedDataOutputIdSet: string[]

    constructor(channelHubModel: ChannelHubModel)
    {
        this.Name = ko.observable<string>(channelHubModel.Name)
        this.Group = ko.observable<string>(channelHubModel.Group)
        this.OneDasDataType = ko.observable<OneDasDataTypeEnum>(channelHubModel.OneDasDataType)
        this.SampleRate = ko.observable<SampleRateEnum>(channelHubModel.SampleRate)
        this.Guid = channelHubModel.Guid
        this.CreationDateTime = channelHubModel.CreationDateTime
        this.Unit = ko.observable<string>(channelHubModel.Unit)
        this.TransferFunctionSet = ko.observableArray<TransferFunctionViewModel>(channelHubModel.TransferFunctionSet.map(tf => new TransferFunctionViewModel(tf)))
        this.SelectedTransferFunction = ko.observable<TransferFunctionViewModel>(this.CreateDefaultTransferFunction())
        this.IsSelected = ko.observable<boolean>(false)

        this.AssociatedDataInput = ko.observable<DataPortViewModel>()
        this.AssociatedDataOutputSet = ko.observableArray<DataPortViewModel>()

        this.AssociatedDataInputId = channelHubModel.SerializerDataInputId
        this.AssociatedDataOutputIdSet = channelHubModel.SerializerDataOutputIdSet
        this.EvaluatedTransferFunctionSet = []
    }

    // methods
    public GetTransformedValue = (value: any): string => 
    {
        if (value === "NaN")
        {
            value = NaN
        }

        this.EvaluatedTransferFunctionSet.forEach(tf => value = tf(value))

        return value
    }

    private CreateDefaultTransferFunction = () =>
    {
        return new TransferFunctionViewModel(new TransferFunctionModel("0001-01-01T00:00:00Z", "polynomial", "permanent", "1;0"))
    }

    public IsAssociationAllowed(dataPort: DataPortViewModel)
    {
        return (dataPort.OneDasDataType & 0xff) == (this.OneDasDataType() & 0xff)
    }

    public UpdateAssociation = (dataPort: DataPortViewModel) =>
    {
        switch (dataPort.DataDirection)
        {
            case DataDirectionEnum.Input:

                this.ResetAssociation(false, this.AssociatedDataInput())
                break

            case DataDirectionEnum.Output:

                this.ResetAssociation(false, dataPort)
                break

            default:
                throw new Error("Not implemented.");
        }

        this.SetAssociation(dataPort)
    }

    public SetAssociation(dataPort: DataPortViewModel)
    {
        dataPort.AssociatedChannelHubSet.push(this)

        switch (dataPort.DataDirection)
        {
            case DataDirectionEnum.Input:

                this.AssociatedDataInput(dataPort)
                this.AssociatedDataInputId = dataPort.ToFullQualifiedIdentifier()

                break

            case DataDirectionEnum.Output:

                let dataOutputId = dataPort.ToFullQualifiedIdentifier()

                this.AssociatedDataOutputSet.push(dataPort)

                if (this.AssociatedDataOutputIdSet.indexOf(dataOutputId) < 0)
                {
                    this.AssociatedDataOutputIdSet.push(dataPort.ToFullQualifiedIdentifier())
                }

                break
        }
    }

    public ResetAssociation(maintainWeakReference: boolean, ...dataPortSet: DataPortViewModel[])
    {
        dataPortSet.forEach(dataPort =>
        {
            if (!dataPort)
            {
                return
            }

            dataPort.AssociatedChannelHubSet.remove(this)

            switch (dataPort.DataDirection)
            {
                case DataDirectionEnum.Input:

                    this.AssociatedDataInput(null)

                    if (!maintainWeakReference)
                    {
                        this.AssociatedDataInputId = null
                    }

                    break

                case DataDirectionEnum.Output:

                    this.AssociatedDataOutputSet.remove(dataPort)

                    if (!maintainWeakReference)
                    {
                        let index: number = this.AssociatedDataOutputIdSet.indexOf(dataPort.ToFullQualifiedIdentifier())

                        if (index > -1)
                        {
                            this.AssociatedDataOutputIdSet.splice(index, 1)
                        }   
                    }

                    break
            }
        })
    }

    public ResetAllAssociations(maintainWeakReference: boolean)
    {
        if (this.AssociatedDataInput())
        {
            this.ResetAssociation(maintainWeakReference, this.AssociatedDataInput())
        }

        this.ResetAssociation(maintainWeakReference, ...this.AssociatedDataOutputSet())
    }

    public GetAssociatedDataInputId = () =>
    {
        return this.AssociatedDataInputId;
    }

    public GetAssociatedDataOutputIdSet = () =>
    {
        return this.AssociatedDataOutputIdSet;
    }

    public ToModel()
    {
        return {
            Name: <string>this.Name(),
            Group: <string>this.Group(),
            OneDasDataType: <OneDasDataTypeEnum>this.OneDasDataType(),
            SampleRate: <SampleRateEnum>this.SampleRate(),
            Guid: <string>this.Guid,
            CreationDateTime: <string>this.CreationDateTime,
            Unit: <string>this.Unit(),
            TransferFunctionSet: <TransferFunctionModel[]>this.TransferFunctionSet().map(tf => tf.ToModel()),
            SerializerDataInputId: <string>this.AssociatedDataInputId,
            SerializerDataOutputIdSet: <string[]>this.AssociatedDataOutputIdSet
        }
    }

    public AddTransferFunction = () =>
    {
        this.TransferFunctionSet.push(this.SelectedTransferFunction())
    }

    public DeleteTransferFunction = () =>
    {
        this.TransferFunctionSet.remove(this.SelectedTransferFunction())
    }

    public NewTransferFunction = () =>
    {
        this.SelectedTransferFunction(this.CreateDefaultTransferFunction())
    }

    // commands
    public SelectTransferFunction = (transferFunction: TransferFunctionViewModel) =>
    {
        this.SelectedTransferFunction(transferFunction)
    }
}