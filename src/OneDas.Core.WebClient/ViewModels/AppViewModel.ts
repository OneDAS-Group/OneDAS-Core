class AppViewModel
{
    public ProductVersion: KnockoutObservable<string>
    public ClientSet: KnockoutObservableArray<string>
    public LastError: KnockoutObservable<string>
    public OneDasState: KnockoutObservable<OneDasStateEnum>
    public WebServerOptionsLight: KnockoutObservable<WebServerOptionsLightViewModel>

    public WorkspaceSet: KnockoutObservableArray<WorkspaceBase>
    public ReducedWorkspaceSet: KnockoutObservableArray<WorkspaceBase>
    public ClientMessageLog: KnockoutObservableArray<MessageLogEntryViewModel>
    public PerformanceInformation: KnockoutObservable<OneDasPerformanceInformationViewModel>
    public IsConnected: KnockoutObservable<boolean>

    public DataGatewayExtensionIdentificationSet: KnockoutObservableArray<ExtensionIdentificationViewModel>
    public DataWriterExtensionIdentificationSet: KnockoutObservableArray<ExtensionIdentificationViewModel>

    public NewWebServerOptionsLightOneDasName: KnockoutObservable<string>
    public NewWebServerOptionsLightAspBaseUrl: KnockoutObservable<string>
    public NewWebServerOptionsLightBaseDirectoryPath: KnockoutObservable<string>

    public InstalledPackageSet: KnockoutObservableArray<PackageMetadataViewModel>
    public ActiveProject: KnockoutObservable<OneDasProjectViewModel>

    constructor(appModel: any)
    {
        // enumeration description
        EnumerationHelper.Description["DataDirectionEnum_Input"] = "Input"
        EnumerationHelper.Description["DataDirectionEnum_Output"] = "Output"

        EnumerationHelper.Description["FileGranularityEnum_Minute_1"] = "1 file per minute"
        EnumerationHelper.Description["FileGranularityEnum_Minute_10"] = "1 file per 10 minutes"
        EnumerationHelper.Description["FileGranularityEnum_Hour"] = "1 file per hour"
        EnumerationHelper.Description["FileGranularityEnum_Day"] = "1 file per day"

        EnumerationHelper.Description["LiveViewPeriodEnum_Period_60"] = "1 min"
        EnumerationHelper.Description["LiveViewPeriodEnum_Period_600"] = "10 min"
        EnumerationHelper.Description["LiveViewPeriodEnum_Period_3600"] = "1 hour"

        EnumerationHelper.Description["OneDasDataTypeEnum_BOOLEAN"] = "BOOLEAN"
        EnumerationHelper.Description["OneDasDataTypeEnum_UINT8"] = "UINT8"
        EnumerationHelper.Description["OneDasDataTypeEnum_INT8"] = "INT8"
        EnumerationHelper.Description["OneDasDataTypeEnum_UINT16"] = "UINT16"
        EnumerationHelper.Description["OneDasDataTypeEnum_INT16"] = "INT16"
        EnumerationHelper.Description["OneDasDataTypeEnum_UINT32"] = "UINT32"
        EnumerationHelper.Description["OneDasDataTypeEnum_INT32"] = "INT32"
        EnumerationHelper.Description["OneDasDataTypeEnum_FLOAT32"] = "FLOAT32"
        EnumerationHelper.Description["OneDasDataTypeEnum_FLOAT64"] = "FLOAT64"

        EnumerationHelper.Description["SampleRateEnum_SampleRate_100"] = "100 Hz"
        EnumerationHelper.Description["SampleRateEnum_SampleRate_25"] = "25 Hz"
        EnumerationHelper.Description["SampleRateEnum_SampleRate_5"] = "5 Hz"
        EnumerationHelper.Description["SampleRateEnum_SampleRate_1"] = "1 Hz"

        // 
        this.ProductVersion = ko.observable<string>()
        this.ClientSet = ko.observableArray<string>()
        this.LastError = ko.observable<string>()
        this.OneDasState = ko.observable<OneDasStateEnum>()
        this.WebServerOptionsLight = ko.observable();

        this.WorkspaceSet = ko.observableArray<WorkspaceBase>()
        this.ClientMessageLog = ko.observableArray<MessageLogEntryViewModel>()
        this.PerformanceInformation = ko.observable<OneDasPerformanceInformationViewModel>()
        this.IsConnected = ko.observable<boolean>(true)

        this.DataGatewayExtensionIdentificationSet = ko.observableArray<ExtensionIdentificationViewModel>()
        this.DataWriterExtensionIdentificationSet = ko.observableArray<ExtensionIdentificationViewModel>()

        this.NewWebServerOptionsLightOneDasName = ko.observable<string>()
        this.NewWebServerOptionsLightAspBaseUrl = ko.observable<string>()
        this.NewWebServerOptionsLightBaseDirectoryPath = ko.observable<string>()

        this.InstalledPackageSet = ko.observableArray<PackageMetadataViewModel>()
        this.ActiveProject = ko.observable<OneDasProjectViewModel>()

        this.ActiveProject.subscribe(newValue =>
        {
            if (newValue)
            {
                newValue.ChannelHubSet().forEach(channelHub =>
                {
                    channelHub.EvaluatedTransferFunctionSet = channelHub.TransferFunctionSet().map(tf => 
                    {
                        switch (tf.Type())
                        {
                            case "polynomial":

                                let argumentSet: string[]
                                let coefficient0: number
                                let coefficient1: number

                                argumentSet = tf.Argument().split(";")
                                coefficient0 = <number>math.eval(argumentSet[1])
                                coefficient1 = <number>math.eval(argumentSet[0])

                                return (x: number) => { return x * coefficient1 + coefficient0 }

                            case "function":

                                let evalFunction: mathjs.EvalFunction

                                evalFunction = math.compile(tf.Argument())

                                return (x: number) => { return evalFunction.eval({ x: x }) }
                        }
                    })
                })
            }
        })

        this.Update(appModel)

        this.WorkspaceSet.push(new StartViewModel(this.ActiveProject))
        this.WorkspaceSet.push(new ControlViewModel(this.ActiveProject))
        this.WorkspaceSet.push(new LiveViewViewModel(this.ActiveProject))
        this.WorkspaceSet.push(new EditorViewModel(this.ActiveProject))
        this.WorkspaceSet.push(new ExtensionViewModel(this.ActiveProject))

        this.ReducedWorkspaceSet = ko.observableArray(this.WorkspaceSet().slice(1, this.WorkspaceSet().length))

        // server callbacks
        ConnectionManager.WebClientHub.on("SendWebServerOptionsLight", oneDasSettingsModel =>
        {
            this.WebServerOptionsLight(new WebServerOptionsLightViewModel(oneDasSettingsModel))
        })

        ConnectionManager.WebClientHub.on("SendOneDasState", async (oneDasState) =>
        {
            this.OneDasState(oneDasState)

            if (oneDasState == OneDasStateEnum.Error)
            {
                this.ActiveProject(null)

                try
                {
                    let lastError: string

                    lastError = await ConnectionManager.InvokeWebClientHub("GetLastError")
                    this.LastError(lastError)
                }
                catch (e)
                {
                    alert(e.message)
                }
            }
        })

        ConnectionManager.WebClientHub.on("SendActiveProject", projectModel =>
        {
            this.InitializeProject(projectModel)
        })

        ConnectionManager.WebClientHub.on("SendPerformanceInformation", performanceInformationModel =>
        {
            this.PerformanceInformation(new OneDasPerformanceInformationViewModel(performanceInformationModel))
        })

        ConnectionManager.WebClientHub.on("SendDataSnapshot", (dateTime: string, dataSnapshot: any[]) =>
        {
            if (this.ActiveProject())
            {
                this.ActiveProject().DataSnapshot(dataSnapshot)
            }
        })

        ConnectionManager.WebClientHub.on("SendClientMessage", clientMessage =>
        {
            this.ClientMessageLog.unshift(new MessageLogEntryViewModel(new Date().toLocaleTimeString('de-DE',
                {
                    hour12: false,
                    hour: "numeric",
                    minute: "numeric",
                    second: "numeric"
                }), clientMessage))

            if (this.ClientMessageLog().length > 100)
            {
                this.ClientMessageLog.pop()
            }
        })

        ConnectionManager.WebClientHub.on("SendInstalledPackages", installedPackageSet =>
        {
            this.InstalledPackageSet(installedPackageSet.map(packageMetadata => new PackageMetadataViewModel(packageMetadata)))
        })

        ConnectionManager.WebClientHub.on("SendExtensionIdentifications", (dataGatewayExtensionIdentificationSet, dataWriterExtensionIdentificationSet) =>
        {
            this.RegisterExtensions("DataGateway", dataGatewayExtensionIdentificationSet, this.DataGatewayExtensionIdentificationSet)
            this.RegisterExtensions("DataWriter", dataWriterExtensionIdentificationSet, this.DataWriterExtensionIdentificationSet)
        })
    }

    // methods
    public Update(appModel: any)
    {
        // apply new appModel values
        this.ProductVersion(appModel.ProductVersion)
        this.ClientSet(appModel.ClientSet)
        this.LastError(appModel.LastError)
        this.OneDasState(appModel.OneDasState)
        this.WebServerOptionsLight(new WebServerOptionsLightViewModel(appModel.WebServerOptionsLight));

        // register extensions
        this.RegisterExtensions("DataGateway", appModel.DataGatewayExtensionIdentificationSet, this.DataGatewayExtensionIdentificationSet)
        this.RegisterExtensions("DataWriter", appModel.DataWriterExtensionIdentificationSet, this.DataWriterExtensionIdentificationSet)
        
        // installed packages
        this.InstalledPackageSet(appModel.InstalledPackageSet.map(packageMetadata => new PackageMetadataViewModel(packageMetadata)))

        // initialize project
        if (appModel.ActiveProjectSettings)
        {
            this.InitializeProject(appModel.ActiveProjectSettings)
        }
        else
        {
            this.ActiveProject(null)
        }
    }

    public RegisterExtensions(extensionType: string, extensionIdentificationModelSet: any[], extensionIdentificationSet: KnockoutObservableArray<ExtensionIdentificationViewModel>)
    {
        extensionIdentificationSet().forEach(extensionIdentification =>
        {
            if (ko.components.isRegistered(extensionIdentification.Id))
            {
                ko.components.unregister(extensionIdentification.Id)
            }
        })

        ExtensionHive.ExtensionIdentificationSet.set(extensionType, extensionIdentificationModelSet.map(x => new ExtensionIdentificationViewModel(x)))

        ExtensionHive.ExtensionIdentificationSet.get(extensionType).forEach(extensionIdentification =>
        {
            ko.components.register(extensionIdentification.Id, {
                template:
                    {
                        ExtensionType: extensionType, ExtensionIdentification: extensionIdentification
                    },
                viewModel:
                    {
                        createViewModel: (params, componentInfo) => 
                        {
                            return params.GetCallback(params.Index)
                        }
                    }
            })
        })

        extensionIdentificationSet(ExtensionHive.ExtensionIdentificationSet.get(extensionType))

        console.log("OneDAS: " + extensionType + " extensions registered (" + ExtensionHive.ExtensionIdentificationSet.get(extensionType).length + ")")
    }

    public InitializeProject = async (projectModel) =>
    {
        let project: OneDasProjectViewModel

        project = new OneDasProjectViewModel(projectModel)

        await project.InitializeAsync(projectModel.DataGatewaySettingsSet, projectModel.DataWriterSettingsSet)

        this.ActiveProject(project)

        console.log("OneDAS: project updated")
    }

    // commands
    public AcknowledgeError = async () =>
    {
        try
        {
            await ConnectionManager.InvokeWebClientHub('AcknowledgeError')
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public UpdateNewWebServerOptionsLight = () =>
    {
        this.NewWebServerOptionsLightOneDasName(this.WebServerOptionsLight().OneDasName)
        this.NewWebServerOptionsLightAspBaseUrl(this.WebServerOptionsLight().AspBaseUrl)
        this.NewWebServerOptionsLightBaseDirectoryPath(this.WebServerOptionsLight().BaseDirectoryPath)
    }

    public SaveWebServerOptionsLight = async () =>
    {
        // improve! find better solution in combination with validation
        this.WebServerOptionsLight().OneDasName = this.NewWebServerOptionsLightOneDasName()
        this.WebServerOptionsLight().AspBaseUrl = this.NewWebServerOptionsLightAspBaseUrl()
        this.WebServerOptionsLight().BaseDirectoryPath = this.NewWebServerOptionsLightBaseDirectoryPath()

        try
        {
            await ConnectionManager.InvokeWebClientHub('SaveWebServerOptionsLight', this.WebServerOptionsLight())
        }
        catch (e)
        {
            alert(e.message)
        }
    }
}
