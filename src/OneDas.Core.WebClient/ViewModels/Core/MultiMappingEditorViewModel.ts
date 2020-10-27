class MultiMappingEditorViewModel
{
    public SelectedGroupName: KnockoutObservable<string>
    public ChannelHubGroupNameSet: KnockoutObservableArray<string>
    public FilteredChannelHubSet: KnockoutObservableArray<ChannelHubViewModel>
    public DataPortStartIndex: KnockoutObservable<number>
    public ChannelHubStartIndex: KnockoutObservable<number>
    public Length: KnockoutObservable<number>
    public KeepChannelMappings: KnockoutObservable<boolean>
    public IsValid: KnockoutObservable<boolean>
    public ErrorMessage: KnockoutObservable<string>

    private _dataPortSet: DataPortViewModel[]
    private _channelHubSet: ChannelHubViewModel[]

    constructor(dataPortSet: DataPortViewModel[], channelHubSet: ChannelHubViewModel[])
    {
        this._dataPortSet = dataPortSet
        this._channelHubSet = channelHubSet

        this.SelectedGroupName = ko.observable<string>("")
        this.ChannelHubGroupNameSet = ko.observableArray<string>(Array.from(new Set(MapMany(channelHubSet.map(channelHub => channelHub.Group), group => group().split("\n")))))
        this.FilteredChannelHubSet = ko.observableArray<ChannelHubViewModel>()
        this.DataPortStartIndex = ko.observable<number>(0)
        this.ChannelHubStartIndex = ko.observable<number>(0)
        this.Length = ko.observable<number>(0)
        this.KeepChannelMappings = ko.observable<boolean>(false)
        this.IsValid = ko.observable<boolean>(false)
        this.ErrorMessage = ko.observable<string>("")

        this.SelectedGroupName.subscribe(newValue =>
        {
            this.FilteredChannelHubSet(channelHubSet.filter(channelHub => channelHub.Group().indexOf(this.SelectedGroupName()) > -1))
            this.Validate()
        })

        this.DataPortStartIndex.subscribe(newValue =>
        {
            this.Validate()
        })

        this.ChannelHubStartIndex.subscribe(newValue =>
        {
            this.Validate()
        })

        this.Length.subscribe(newValue =>
        {
            this.Validate()
        })
    }

    // methods
    private Validate = () =>
    {
        let startDataPortIndex: number
        let startChannelHubIndex: number

        let endDataPortIndex: number
        let endChannelHubIndex: number

        let filteredChannelHubSet: ChannelHubViewModel[]

        startDataPortIndex = this.DataPortStartIndex()
        startChannelHubIndex = this.ChannelHubStartIndex()

        endDataPortIndex = this.DataPortStartIndex() + this.Length() - 1
        endChannelHubIndex = this.ChannelHubStartIndex() + this.Length() - 1

        filteredChannelHubSet = this.FilteredChannelHubSet()

        this._dataPortSet.forEach(dataPort => dataPort.IsSelected(false))
        this._channelHubSet.forEach(channelHub => channelHub.IsSelected(false))

        // check parameters
        if ((startDataPortIndex < 0) || (endDataPortIndex >= this._dataPortSet.length) ||
            (startChannelHubIndex < 0) || (endChannelHubIndex >= filteredChannelHubSet.length))
        {
            this.ErrorMessage(ErrorMessage["MultiMappingEditorViewModel_InvalidSettings"])
            this.IsValid(false)

            return
        }

        // check data type
        for (var i = 0; i < this.Length(); i++)
        {
            if (!filteredChannelHubSet[i + startChannelHubIndex].IsAssociationAllowed(this._dataPortSet[i + startDataPortIndex]))
            {
                this.ErrorMessage(ErrorMessage["MultiMappingEditorViewModel_WrongDataType"])
                this.IsValid(false)

                return
            }
        }

        // no errors found (valid)
        this._dataPortSet.forEach((dataPort, index) =>
        {
            if (startDataPortIndex <= index && index <= endDataPortIndex)
            {
                dataPort.IsSelected(true)
            }
        })

        filteredChannelHubSet.forEach((channelHub, index) =>
        {
            if (startChannelHubIndex <= index && index <= endChannelHubIndex)
            {
                channelHub.IsSelected(true)
            }
        })

        this.ErrorMessage("")
        this.IsValid(true)
    }

    // commands
    private Apply()
    {
        let filteredChannelHubSet: ChannelHubViewModel[]
        let keepChannelMappings: boolean
        let startDataPortIndex: number
        let startChannelHubIndex: number

        filteredChannelHubSet = this.FilteredChannelHubSet()
        keepChannelMappings = this.KeepChannelMappings()
        startDataPortIndex = this.DataPortStartIndex()
        startChannelHubIndex = this.ChannelHubStartIndex()

        for (var i = 0; i < this.Length(); i++)
        {
            if (!keepChannelMappings)
            {
                this._dataPortSet[i + startDataPortIndex].ResetAssociations(false)
            }

            filteredChannelHubSet[i + startChannelHubIndex].UpdateAssociation(this._dataPortSet[i + startDataPortIndex])
        }
    }
}