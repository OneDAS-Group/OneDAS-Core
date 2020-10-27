let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new EthercatViewModel(model, identification)

class EthercatViewModel extends DataGatewayViewModelBase
{
    public NetworkInterfaceDescriptionSet: KnockoutObservableArray<NetworkInterfaceDescription>
    public NicHardwareAddress: KnockoutObservable<string>
    public RootSlaveInfo: KnockoutObservable<SlaveInfoViewModel>
    public SelectedSlaveInfo: KnockoutObservable<SlaveInfoViewModel>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification)

        this.NetworkInterfaceDescriptionSet = ko.observableArray()
        this.NicHardwareAddress = ko.observable<string>(model.NicHardwareAddress)
        this.RootSlaveInfo = ko.observable<SlaveInfoViewModel>()
        this.SelectedSlaveInfo = ko.observable<SlaveInfoViewModel>()

        if (model.RootSlaveInfo)
        {
            this.RootSlaveInfo(new SlaveInfoViewModel(model.RootSlaveInfo))
        }

        this.RootSlaveInfo.subscribe(async newValue => 
        {
            await this.InitializeAsync()
        })
    }

    // methods
    public async InitializeAsync()
    {
        let modifiedSlaveInfoSet: SlaveInfoViewModel[]
        let extensionIdentificationModelSet: any[]
        let actionResponse: ActionResponse

        if (!ExtensionHive.ExtensionIdentificationSet.has("EthercatSlaveExtension"))
        {
            actionResponse = await this.SendActionRequest(0, "GetExtensionIdentifications", null)
            extensionIdentificationModelSet = actionResponse.Data.$values

            ExtensionHive.ExtensionIdentificationSet.set("EthercatSlaveExtension", extensionIdentificationModelSet.map(x => new ExtensionIdentificationViewModel(x)))

            ExtensionHive.ExtensionIdentificationSet.get("EthercatSlaveExtension").forEach(extensionIdentification =>
            {
                ko.components.register(extensionIdentification.Id, {
                    template:
                    <any>{
                        ExtensionType: "EthercatSlaveExtension", ExtensionIdentification: extensionIdentification
                    },
                    viewModel:
                    {
                        createViewModel: (params, componentInfo) => 
                        {
                            return params.GetEthercatSlaveExtensionCallback(params.Index)
                        }
                    }
                })
            })
        }

        if (this.RootSlaveInfo())
        {
            await Promise.all(this.RootSlaveInfo().GetDescendants(true).map(async (slaveInfo, index) =>
            {
                await slaveInfo.InitializeAsync()
                slaveInfo.DynamicData(await this.GetDynamicSlaveInfoDataAsync(slaveInfo))
            }))

            // find modified slaves
            modifiedSlaveInfoSet = this.RootSlaveInfo().GetDescendants(false).filter(slaveInfo => slaveInfo.GetOldCsa() !== slaveInfo.Csa)

            if (modifiedSlaveInfoSet.length > 0)
            {
                let message: string = "The IDs of the following terminals have been changed because they were invalid:\n\n"

                modifiedSlaveInfoSet.forEach(slaveInfo =>
                {
                    message = message + slaveInfo.DynamicData().Name + " - old ID: " + slaveInfo.GetOldCsa() + " - new ID: " + slaveInfo.Csa + "\n"
                    slaveInfo.ResetOldCsa()
                })

                alert(message)
            }

            this.DataPortSet(MapMany(this.RootSlaveInfo().GetDescendants(false), (x: SlaveInfoViewModel) => x.GetChannels()))
        }     
        else
        {
            this.DataPortSet([])
        }
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.NicHardwareAddress = <string>this.NicHardwareAddress()

        if (this.RootSlaveInfo())
        {
            model.RootSlaveInfo = <SlaveInfoViewModel>this.RootSlaveInfo().ToModel()
        }
        else
        {
            model.RootSlaveInfo = null
        }
    }

    private GetDynamicSlaveInfoDataAsync = async (slaveInfo: SlaveInfoViewModel) =>
    {
        let actionResponse: ActionResponse
        let slaveInfoDynamicDataModel: any

        actionResponse = await this.SendActionRequest(0, "GetDynamicSlaveInfoData", slaveInfo.ToFlatModel())
        slaveInfoDynamicDataModel = actionResponse.Data

        return new SlaveInfoDynamicDataViewModel(slaveInfoDynamicDataModel, slaveInfo, this)
    }

    // commands
    public SelectSlaveInfo = (item: SlaveInfoViewModel) =>
    {
        this.SelectedSlaveInfo(item)
    }

    public GetAvailableNetworkInterfaces = async () =>
    {
        let actionResponse: ActionResponse
        let dictionary: any

        try
        {
            actionResponse = await this.SendActionRequest(0, "GetAvailableNetworkInterfaces", null)
            dictionary = actionResponse.Data

            this.NetworkInterfaceDescriptionSet.removeAll()

            for (var key in dictionary)
            {
                if (key !== "$type")
                {
                    this.NetworkInterfaceDescriptionSet.push(new NetworkInterfaceDescription(key, dictionary[key]))
                }
            }
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public OpenSlaveSettings = (slaveInfo: SlaveInfoViewModel) =>
    {
        this.SelectedSlaveInfo(slaveInfo)
    }

    public SelectNetworkAdapter = (networkInterfaceDescription: NetworkInterfaceDescription) =>
    {
        this.NicHardwareAddress(networkInterfaceDescription.Address)
    }

    public UpdateSlaveInfo = async (slaveInfo: SlaveInfoViewModel) =>
    {
        slaveInfo.DynamicData(await this.GetDynamicSlaveInfoDataAsync(slaveInfo))

		this.DataPortSet(MapMany(this.RootSlaveInfo().GetDescendants(false), (slaveInfo: SlaveInfoViewModel) => slaveInfo.GetChannels()))
        this.SelectedSlaveInfo(null)
    }

    public ReloadHardware = async () =>
    {
        if (confirm("The terminals will be reloaded. In case of removed terminals, the corresponding mappings will be deleted (but can be restored once the terminal is reconnected). Proceed?") === true)
        {
            let data: any
            let actionResponse: ActionResponse
            let slaveInfoModel: any

            if (this.RootSlaveInfo())
            {
                data = { NicHardwareAddress: this.NicHardwareAddress(), RootSlaveInfo: this.RootSlaveInfo().ToModel() }
            }
            else
            {
                data = { NicHardwareAddress: this.NicHardwareAddress(), RootSlaveInfo: null }
            }

            try 
            {               
                actionResponse = await this.SendActionRequest(0, "ReloadHardware", data)
                slaveInfoModel = actionResponse.Data
                this.RootSlaveInfo(new SlaveInfoViewModel(slaveInfoModel))
            }
            catch (e)
            {
                alert(e.message)
            }
        }
    }
}
