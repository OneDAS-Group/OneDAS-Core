abstract class ExtendedDataGatewayViewModelBase extends DataGatewayViewModelBase
{
    public ModuleToDataPortMap: KnockoutObservableArray<ObservableGroup<DataPortViewModel>>
    public OneDasModuleSelector: KnockoutObservable<OneDasModuleSelectorViewModel>

    constructor(model, identification: PluginIdentificationViewModel, oneDasModuleSelector: OneDasModuleSelectorViewModel)
    {
        super(model, identification)

        this.ModuleToDataPortMap = ko.observableArray()
        this.OneDasModuleSelector = ko.observable<OneDasModuleSelectorViewModel>(oneDasModuleSelector)

        if (this.OneDasModuleSelector())
        {
            this.OneDasModuleSelector().OnModuleSetChanged.subscribe((sender, args) =>
            {
                this.UpdateDataPortSet()
            })
        }
    }

    public async InitializeAsync()
    {
        this.UpdateDataPortSet()
    }

    public UpdateDataPortSet()
    {
        let index: number
        let moduleToDataPortMap: ObservableGroup<DataPortViewModel>[]

        moduleToDataPortMap = []

        // inputs
        index = 0

        moduleToDataPortMap = moduleToDataPortMap.concat(this.OneDasModuleSelector().ModuleSet().filter(oneDasModule => oneDasModule.DataDirection() === DataDirectionEnum.Input).map(oneDasModule =>
        {
            let group: ObservableGroup<DataPortViewModel>

            group = new ObservableGroup<DataPortViewModel>(oneDasModule.ToString(), this.CreateDataPortSet(oneDasModule, index))
            index += oneDasModule.Size();

            return group
        }))

        // outputs
        index = 0

        moduleToDataPortMap = moduleToDataPortMap.concat(this.OneDasModuleSelector().ModuleSet().filter(oneDasModule => oneDasModule.DataDirection() === DataDirectionEnum.Output).map(oneDasModule =>
        {
            let group: ObservableGroup<DataPortViewModel>

            group = new ObservableGroup<DataPortViewModel>(oneDasModule.ToString(), this.CreateDataPortSet(oneDasModule, index))
            index += oneDasModule.Size();

            return group
        }))

        this.ModuleToDataPortMap(moduleToDataPortMap)
        this.DataPortSet(MapMany(this.ModuleToDataPortMap(), group => group.Members()))
    }

    public CreateDataPortSet(oneDasModule: OneDasModuleViewModel, index: number)
    {
        let prefix: string

        switch (oneDasModule.DataDirection())
        {
            case DataDirectionEnum.Input:
                prefix = "Input"
                break

            case DataDirectionEnum.Output:
                prefix = "Output"
                break
        }

        return Array.from(new Array(oneDasModule.Size()), (x, i) => 
        {
            return {
                Name: <string>prefix + " " + (index + i),
                DataType: <OneDasDataTypeEnum>oneDasModule.DataType(),
                DataDirection: <DataDirectionEnum>oneDasModule.DataDirection()
            }
        }).map(dataPortModel => new DataPortViewModel(dataPortModel, this))
    }
}