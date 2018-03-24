class OneDasProjectViewModel
{
    // settings
    public FormatVersion: number
    public Description: OneDasCampaignDescriptionViewModel
    public ChannelHubSet: KnockoutObservableArray<ChannelHubViewModel>
    public GroupedChannelHubSet: KnockoutObservableArray<ObservableGroup<ChannelHubViewModel>>
    public DataGatewaySet: KnockoutObservableArray<DataGatewayViewModelBase>
    public DataWriterSet: KnockoutObservableArray<DataWriterViewModelBase>

    // live data
    public DataSnapshot: KnockoutObservableArray<any>

    // new channel
    public NewChannelName: KnockoutObservable<string>
    public NewChannelGroup: KnockoutObservable<string>
    public NewChannelSelectedDataType: KnockoutObservable<OneDasDataTypeEnum>
    public NewChannelSelectedSampleRate: KnockoutObservable<SampleRateEnum>

    // new channel validation
    public NewChannelCheckError: KnockoutObservable<boolean>
    public NewChannelNameHasError: KnockoutObservable<boolean>
    public NewChannelNameErrorDescription: KnockoutObservable<string>
    public NewChannelGroupHasError: KnockoutObservable<boolean>
    public NewChannelGroupErrorDescription: KnockoutObservable<string>
    public NewChannelIsNotificationVisible: KnockoutObservable<boolean>

    // edit channel validation
    public EditChannelNameHasError: KnockoutObservable<boolean>
    public EditChannelNameErrorDescription: KnockoutObservable<string>
    public EditChannelGroupHasError: KnockoutObservable<boolean>
    public EditChannelGroupErrorDescription: KnockoutObservable<string>
    public EditChannelDummyChannel: KnockoutObservable<ChannelHubViewModel>

    // multi mapping
    public MultiMappingEditor: KnockoutObservable<MultiMappingEditorViewModel>

    // search string
    public SearchString: KnockoutObservable<string>

    // helper
    public SelectedDataPort: KnockoutObservable<DataPortViewModel>
    public SelectedChannelHub: KnockoutObservable<ChannelHubViewModel>
    public SelectedChannelHub_ContextMenu: KnockoutObservable<ChannelHubViewModel>
    public SelectedDataGateway: KnockoutObservable<DataGatewayViewModelBase>
    public SelectedDataWriter: KnockoutObservable<DataWriterViewModelBase>
    public SelectedPlugin: KnockoutObservable<PluginViewModelBase>
    public CurrentTitle: KnockoutObservable<string>
    public IsFullEditingEnabled: KnockoutObservable<boolean>
    public IsUpdating: boolean

    constructor(projectModel)
    {
        let promiseSet: Promise<any>[]

        this.FormatVersion = projectModel.FormatVersion
        this.Description = new OneDasCampaignDescriptionViewModel(projectModel.Description)
        this.ChannelHubSet = ko.observableArray<ChannelHubViewModel>(projectModel.ChannelHubSettingsSet.map(x => new ChannelHubViewModel(x)))
        this.GroupedChannelHubSet = ko.observableArray(ObservableGroupBy(this.ChannelHubSet(), x => x.Name(), x => x.Group(), ""))
        this.DataGatewaySet = ko.observableArray<DataGatewayViewModelBase>()
        this.DataWriterSet = ko.observableArray<DataWriterViewModelBase>()

        this.DataSnapshot = ko.observableArray<any>()

        this.NewChannelName = ko.observable<string>("")
        this.NewChannelGroup = ko.observable<string>("Default")
        this.NewChannelSelectedDataType = ko.observable<OneDasDataTypeEnum>(OneDasDataTypeEnum.INT16)
        this.NewChannelSelectedSampleRate = ko.observable<SampleRateEnum>(SampleRateEnum.SampleRate_1)

        this.NewChannelCheckError = ko.observable(true)
        this.NewChannelNameHasError = ko.observable<boolean>(false)
        this.NewChannelNameErrorDescription = ko.observable<string>("")
        this.NewChannelGroupHasError = ko.observable<boolean>(false)
        this.NewChannelGroupErrorDescription = ko.observable<string>("")
        this.NewChannelIsNotificationVisible = ko.observable<boolean>(false)

        this.EditChannelNameHasError = ko.observable<boolean>(false)
        this.EditChannelNameErrorDescription = ko.observable<string>("")
        this.EditChannelGroupHasError = ko.observable<boolean>(false)
        this.EditChannelGroupErrorDescription = ko.observable<string>("")
        this.EditChannelDummyChannel = ko.observable(new ChannelHubViewModel(new ChannelHubModel("", "", 0)))

        this.MultiMappingEditor = ko.observable<MultiMappingEditorViewModel>()

        this.SearchString = ko.observable("")
        this.SearchString.extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } })

        this.SelectedDataPort = ko.observable<DataPortViewModel>()
        this.SelectedChannelHub = ko.observable<ChannelHubViewModel>()
        this.SelectedChannelHub_ContextMenu = ko.observable<ChannelHubViewModel>()
        this.SelectedDataGateway = ko.observable<DataGatewayViewModelBase>()
        this.SelectedDataWriter = ko.observable<DataWriterViewModelBase>()
        this.SelectedPlugin = ko.observable<PluginViewModelBase>()
        this.CurrentTitle = ko.observable<string>()
        this.IsFullEditingEnabled = ko.observable<boolean>(this.Description.Version() === 0);

        // Multi-Mapping
        this.SelectedDataGateway.subscribe(newValue =>
        {
            if (newValue)
            {
                if (this.MultiMappingEditor())
                {
                    this.InitializeMultiMappingMode()
                }
            }
            else
            {
                this.MultiMappingEditor(null)
            }
        })

        // validation
        this.NewChannelName.subscribe(value =>
        {
            let result: any
            let valueSet: string[]

            if (!this.NewChannelCheckError())
            {
                result = { HasError: false, ErrorDescription: "" }
            }
            else
            {
                valueSet = value.split("\n")

                valueSet.some(currentValue =>
                {
                    if ($.inArray(currentValue, this.ChannelHubSet().map((element) => element.Name())) > -1 || valueSet.length !== new Set(valueSet).size)
                    {
                        result = { HasError: true, ErrorDescription: ErrorMessage["Project_ChannelAlreadyExists"] }
                    }
                    else
                    {
                        result = CheckNamingConvention(currentValue)
                    }

                    if (result.HasError)
                    {
                        return true
                    }
                })
            }

            this.NewChannelNameHasError(result.HasError)
            this.NewChannelNameErrorDescription(result.ErrorDescription)
            this.NewChannelCheckError(true)
            this.NewChannelIsNotificationVisible(false)
        })

        this.NewChannelGroup.subscribe(value =>
        {
            let result: any
            let valueSet: string[]

            if (!this.NewChannelCheckError())
            {
                result = { HasError: false, ErrorDescription: "" }
            }
            else
            {
                valueSet = value.split("\n")

                valueSet.forEach(currentValue =>
                {
                    if (valueSet.length !== new Set(valueSet).size)
                    {
                        result = { HasError: true, ErrorDescription: ErrorMessage["Project_IsAlreadyInGroup"] }
                    }
                    else
                    {
                        result = CheckNamingConvention(currentValue)
                    }

                    if (result.HasError)
                    {
                        return
                    }
                })
            }

            this.NewChannelGroupHasError(result.HasError)
            this.NewChannelGroupErrorDescription(result.ErrorDescription)
        })

        this.EditChannelDummyChannel().Name.subscribe(value =>
        {
            let result: any

            if (this.EditChannelDummyChannel().Name() !== this.SelectedChannelHub().Name() && // channel name is not the same as before
                $.inArray(value, this.ChannelHubSet().map((element) => element.Name())) > -1) // modified channel name already exists
            {
                result = { HasError: true, ErrorDescription: ErrorMessage["Project_ChannelAlreadyExists"] }
            }
            else
            {
                result = CheckNamingConvention(value)
            }

            this.EditChannelNameHasError(result.HasError)
            this.EditChannelNameErrorDescription(result.ErrorDescription)
        })

        this.EditChannelDummyChannel().Group.subscribe(value =>
        {
            let result: any
            let valueSet: string[]

            if (!this.NewChannelCheckError())
            {
                result = { HasError: false, ErrorDescription: "" }
            }
            else
            {
                valueSet = value.split("\n")

                valueSet.forEach(currentValue =>
                {
                    if (valueSet.length !== new Set(valueSet).size)
                    {
                        result = { HasError: true, ErrorDescription: ErrorMessage["Project_IsAlreadyInGroup"] }
                    }
                    else
                    {
                        result = CheckNamingConvention(currentValue)
                    }

                    if (result.HasError)
                    {
                        return
                    }
                })
            }

            this.EditChannelGroupHasError(result.HasError)
            this.EditChannelGroupErrorDescription(result.ErrorDescription)
        })

        // channel grouping
        this.ChannelHubSet.subscribe(changes =>
        {
            this.UpdateGroupedChannelHubSet(true)
        }, null, "arrayChange")

        // search
        this.SearchString.subscribe(value =>
        {
            this.UpdateGroupedChannelHubSet(false)
        })
    }

    // methods
    public InitializeMultiMappingMode()
    {
        this.MultiMappingEditor(new MultiMappingEditorViewModel(this.SelectedDataGateway().DataPortSet(), this.ChannelHubSet()))
    }

    public async InitializeAsync(dataGatewaySettingsModelSet: any[], dataWriterSettingsModelSet: any[])
    {
        try
        {
            let dataGatewaySet: DataGatewayViewModelBase[]
            let dataWriterSet: DataWriterViewModelBase[]

            // register callbacks
            this.SubscribeToChanges()

            // data gateway
            dataGatewaySet = <DataGatewayViewModelBase[]>await Promise.all(dataGatewaySettingsModelSet.map(async pluginModel =>
            {
                return await PluginFactory.CreatePluginViewModelAsync("DataGateway", pluginModel)
            }))

            dataGatewaySet.forEach(dataGateway =>
            {
                if (dataGateway.Description.InstanceId === 0)
                {
                    dataGateway.Description.InstanceId = Math.max(...dataGatewaySet.map(x => x.Description.InstanceId)) + 1
                }
            })

            await Promise.all(dataGatewaySet.map(dataGateway => dataGateway.InitializeAsync()))
            this.DataGatewaySet(dataGatewaySet)

            // data writer
            dataWriterSet = <DataWriterViewModelBase[]>await Promise.all(dataWriterSettingsModelSet.map(async pluginModel =>
            {
                return await PluginFactory.CreatePluginViewModelAsync("DataWriter", pluginModel)
            }))

            dataWriterSet.forEach(dataWriter =>
            {
                if (dataWriter.Description.InstanceId === 0)
                {
                    dataWriter.Description.InstanceId = Math.max(...dataWriterSet.map(x => x.Description.InstanceId)) + 1
                }
            })

            await Promise.all(dataWriterSet.map(dataWriter => dataWriter.InitializeAsync()))
            this.DataWriterSet(dataWriterSet)

            // finish
            this.UpdateMapping()
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public ToModel = () =>
    {
        let model: any

        model = {
            FormatVersion: <number>this.FormatVersion,
            Description: <any>this.Description.ToModel(),
            ChannelHubSettingsSet: <any[]>this.ChannelHubSet().map(x => x.ToModel()),
            DataGatewaySettingsSet: <any[]>this.DataGatewaySet().map(x => x.ToModel()),
            DataWriterSettingsSet: <any[]>this.DataWriterSet().map(x => x.ToModel())
        }

        model.ChannelHubSettingsSet.sort((channelHub1, channelHub2) => this.CompareStrings(channelHub1.Name, channelHub2.Name))

        return model
    }

    public GetDataGateway = (index: number) =>
    {
        return this.DataGatewaySet()[index]
    }

    public GetDataWriter = (index: number) =>
    {
        return this.DataWriterSet()[index]
    }

    private UpdateGroupedChannelHubSet(resetSearchString: boolean)
    {
        let groupedChannelHubSet: ObservableGroup<ChannelHubViewModel>[]

        if (this.IsUpdating)
        {
            return
        }

        this.IsUpdating = true

        if (resetSearchString)
        {
            this.SearchString("")
        }

        groupedChannelHubSet = ObservableGroupBy(this.ChannelHubSet(), x => x.Name(), x => x.Group(), this.SearchString())
        groupedChannelHubSet.forEach(channelHubSet => channelHubSet.Members.sort((channelHub1, channelHub2) => this.CompareStrings(channelHub1.Name(), channelHub2.Name())))
        groupedChannelHubSet.sort((channelHubSet1, channelHubSet2) => this.CompareStrings(channelHubSet1.Key, channelHubSet2.Key))

        this.GroupedChannelHubSet(groupedChannelHubSet)

        this.IsUpdating = false
    }

    private CompareStrings(string1, string2)
    {
        let comparison = 0;

        if (string1 > string2)
        {
            comparison = 1;
        }
        else if (string1 < string2)
        {
            comparison = -1;
        }

        return comparison
    }

    private SubscribeToChanges = () =>
    {
        // ChannelHub <-> DataPort mapping
        this.DataGatewaySet.subscribe(changes =>
        {
            changes.forEach(change =>
            {
                switch (change.status)
                {
                    // data-gateway added (subscribe to data-port array changes)
                    case "added":

                        change.value.DataPortSet.subscribe(changes =>
                        {
                            if (changes.length === 1) // single add / remove happened -> normal handling
                            {
                                switch (changes[0].status)
                                {
                                    // data port added (do nothing)
                                    case "added":
                                        break

                                    // data port removed (remove data-port <-> channel hub association)
                                    case "deleted":
                                        changes[0].value.ResetAssociations(false)
                                        break
                                }
                            }
                            else // replace, sort, reverse, etc happened -> update of mapping required
                            {
                                this.UpdateMapping()
                            }
                        }, null, "arrayChange")

                        break

                    // data-gateway removed (remove all data-port <-> channel hub associations)
                    case "deleted":

                        change.value.DataPortSet().forEach(dataPort =>
                        {
                            dataPort.ResetAssociations(true)
                        })

                        break
                }
            })
        }, null, "arrayChange")
    }

    private RegisterDataGatewayViewModelBase = (DataGatewayViewModelBase: DataGatewayViewModelBase) =>
    {
        this.DataGatewaySet.push(DataGatewayViewModelBase)
    }

    private RegisterDataWriterViewModelBase = (DataWriterViewModelBase: DataWriterViewModelBase) =>
    {
        this.DataWriterSet.push(DataWriterViewModelBase)
    }

    private OnNewChannelModalClosing = () =>
    {
        this.NewChannelIsNotificationVisible(false)
    }

    private Draggable = (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext: KnockoutBindingContext) =>
    {
        element.addEventListener("dragstart", (event) => 
        {
            this.SelectedDataPort(viewModel)
            event.dataTransfer.setData("Dummy", "")
        })
    }

    private Dropable = (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext: KnockoutBindingContext) =>
    {
        element.addEventListener("dragover", (event) => 
        {
            if (this.IsDropAllowed(<ChannelHubViewModel>viewModel))
            {
                event.preventDefault()
            }
        })

        element.addEventListener("drop", (event) => 
        {
            event.preventDefault()
            this.SelectedChannelHub(<ChannelHubViewModel>viewModel)
            this.SelectedChannelHub().UpdateAssociation(this.SelectedDataPort())
        })
    }

    private IsDropAllowed = (channelHub: ChannelHubViewModel) =>
    {
        return channelHub.IsAssociationAllowed(this.SelectedDataPort())
    }

    // commands
    public ToggleMultiMappingMode()
    {
        if (this.MultiMappingEditor())
        {
            this.MultiMappingEditor(null)
        }
        else
        {
            this.InitializeMultiMappingMode()
        }
    }

    public ResetSearch = () =>
    {
        this.SearchString("")
    }

    public ShowContextMenu = (item: ChannelHubViewModel, e) =>
    {
        let offset: JQueryCoordinates

        this.SelectedChannelHub_ContextMenu(item)
        this.SelectedChannelHub(item)

        if (e.ctrlKey)
        {
            return
        }

        offset = $("body").offset()

        $("#Project_ChannelContextMenu").show().css(
            {
                position: "absolute",
                left: e.pageX - offset.left,
                top: e.pageY - offset.top
            })
    }

    public BeginEditChannel = () =>
    {
        this.EditChannelDummyChannel().Name(this.SelectedChannelHub().Name())
        this.EditChannelDummyChannel().Group(this.SelectedChannelHub().Group())
        this.EditChannelDummyChannel().DataType(this.SelectedChannelHub().DataType())
    }

    public EndEditChannel = () =>
    {
        this.SelectedChannelHub().Name(this.EditChannelDummyChannel().Name())
        this.SelectedChannelHub().Group(this.EditChannelDummyChannel().Group())
        this.UpdateGroupedChannelHubSet(true)
    }

    public AddNewChannel = () =>
    {
        this.ChannelHubSet.push(...this.NewChannelName().split("\n").map(channelName =>
        {
            return new ChannelHubViewModel(new ChannelHubModel(channelName, this.NewChannelGroup(), this.NewChannelSelectedDataType()))
        }))

        this.NewChannelCheckError(false)
        this.NewChannelName("")
        this.NewChannelIsNotificationVisible(true)
    }

    public DeleteChannel = () =>
    {
        this.SelectedChannelHub().ResetAllAssociations(false)
        this.ChannelHubSet.remove(this.SelectedChannelHub())
    }

    public AddNewDataGateway = async (item: PluginIdentificationViewModel) =>
    {
        let pluginModel: any
        let pluginViewModel: DataGatewayViewModelBase
        let lastInstanceId: number

        try
        {
            lastInstanceId = Math.max(...this.DataGatewaySet().map(dataGateway => dataGateway.Description.InstanceId))

            pluginModel = await ConnectionManager.InvokeWebClientHub('CreateDataGatewaySettings', item.Id)
            pluginModel.Description.InstanceId = this.DataGatewaySet().length > 0 ? lastInstanceId + 1 : 1
            pluginViewModel = <DataGatewayViewModelBase>await PluginFactory.CreatePluginViewModelAsync("DataGateway", pluginModel)

            await pluginViewModel.InitializeAsync()

            this.DataGatewaySet.push(pluginViewModel)
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public AddNewDataWriter = async (item: PluginIdentificationViewModel) =>
    {
        let pluginModel: any
        let pluginViewModel: DataWriterViewModelBase
        let lastInstanceId: number

        try
        {
            lastInstanceId = Math.max(...this.DataWriterSet().map(dataWriter => dataWriter.Description.InstanceId))

            pluginModel = await ConnectionManager.InvokeWebClientHub('CreateDataWriterSettings', item.Id)
            pluginModel.Description.InstanceId = this.DataWriterSet().length > 0 ? lastInstanceId + 1 : 1
            pluginViewModel = <DataWriterViewModelBase>await PluginFactory.CreatePluginViewModelAsync("DataWriter", pluginModel)

            await pluginViewModel.InitializeAsync()

            this.DataWriterSet.push(pluginViewModel)
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public SelectPlugin = (item: PluginViewModelBase) =>
    {
        this.SelectedPlugin(item)
    }

    public DeleteDataGateway = () =>
    {
        this.DataGatewaySet.remove(<DataGatewayViewModelBase>this.SelectedPlugin())
    }

    public DeleteDataWriter = () =>
    {
        this.DataWriterSet.remove(<DataWriterViewModelBase>this.SelectedPlugin())
    }

    public ClearSelectedViewModels = (e: any) =>
    {
        this.CurrentTitle(document.title)
        this.SelectedDataGateway(null)
        this.SelectedDataWriter(null)
    }

    public SelectDataGateway = (e: any) =>
    {
        if (e.page.ctx.__proto__ instanceof DataGatewayViewModelBase)
        {
            this.SelectedDataGateway(<DataGatewayViewModelBase>e.page.ctx)
            this.SelectedDataWriter(null)
        }

        this.CurrentTitle(document.title)
    }

    public SelectDataWriter = (e: any) =>
    {
        if (e.page.ctx.__proto__ instanceof DataWriterViewModelBase)
        {
            this.SelectedDataGateway(null)
            this.SelectedDataWriter(<DataWriterViewModelBase>e.page.ctx)
        }

        this.CurrentTitle(document.title)
    }

    // mapping
    public HubResetDataInputAssociation = () =>
    {
        let channelHub: ChannelHubViewModel = this.SelectedChannelHub()

        channelHub.ResetAssociation(false, channelHub.AssociatedDataInput())
    }

    public HubResetDataOutputAssociations = () =>
    {
        let channelHub: ChannelHubViewModel = this.SelectedChannelHub()

        channelHub.ResetAssociation(false, ...channelHub.AssociatedDataOutputSet())
    }

    private CreateDataPortIdMap = () =>
    {
        let dataPortIdMap: Map<DataPortViewModel, string>

        dataPortIdMap = new Map<DataPortViewModel, string>()

        this.DataGatewaySet().forEach(dataGateway =>
        {
            dataGateway.DataPortSet().forEach(dataPort =>
            {
                dataPortIdMap.set(dataPort, dataPort.ToFullQualifiedIdentifier())
            })
        })

        return dataPortIdMap
    }

    private UpdateMapping = () =>
    {
        let inputSet: DataPortViewModel[]
        let outputSet: DataPortViewModel[]
        let dataPortIdMap: Map<DataPortViewModel, string>

        inputSet = []
        outputSet = []
        dataPortIdMap = this.CreateDataPortIdMap()

        //
        this.DataGatewaySet().forEach((x) =>
        {
            x.DataPortSet().forEach(y =>
            {
                switch (y.DataDirection)
                {
                    case DataDirectionEnum.Input:
                        inputSet.push(y)
                        break

                    case DataDirectionEnum.Output:
                        outputSet.push(y)
                        break
                }
            })
        })

        this.ChannelHubSet().forEach(channelHub =>
        {
            let dataPort: DataPortViewModel
            let inputId: string

            this.SelectedChannelHub(channelHub)
            channelHub.ResetAllAssociations(true)

            // input
            inputId = channelHub.GetAssociatedDataInputId()

            if (inputId)
            {
                dataPort = inputSet.find(input => dataPortIdMap.get(input) == inputId)

                if (dataPort && channelHub.IsAssociationAllowed(dataPort))
                {
                    channelHub.SetAssociation(dataPort)
                }
            }

            // output
            channelHub.GetAssociatedDataOutputIdSet().forEach(outputId =>
            {
                dataPort = outputSet.find(output => dataPortIdMap.get(output) == outputId)

                if (dataPort && channelHub.IsAssociationAllowed(dataPort))
                {
                    channelHub.SetAssociation(dataPort)
                }
            })
        })

        console.log("OneDAS: mapping updated")
    }
}