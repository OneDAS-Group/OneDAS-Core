abstract class ExtendedDataGatewayViewModelBase extends DataGatewayViewModelBase
{
    public GroupedDataPortSet: KnockoutObservableArray<ObservableGroup<DataPortViewModel>>
    public OneDasInputModuleSelector: KnockoutObservable<OneDasModuleSelectorViewModel>
    public OneDasOutputModuleSelector: KnockoutObservable<OneDasModuleSelectorViewModel>

    constructor(model, identification: PluginIdentificationViewModel, oneDasInputModuleSelector: OneDasModuleSelectorViewModel, oneDasOutputModuleSelector: OneDasModuleSelectorViewModel)
    {
        super(model, identification)

        this.GroupedDataPortSet = ko.observableArray()
        this.OneDasInputModuleSelector = ko.observable<OneDasModuleSelectorViewModel>(oneDasInputModuleSelector)
        this.OneDasOutputModuleSelector = ko.observable<OneDasModuleSelectorViewModel>(oneDasOutputModuleSelector)

        if (this.OneDasInputModuleSelector())
        {
            this.OneDasInputModuleSelector().OnModuleSetChanged.subscribe((sender, args) =>
            {
                this.UpdateDataPortSet()
            })
        }

        if (this.OneDasOutputModuleSelector())
        {
            this.OneDasOutputModuleSelector().OnModuleSetChanged.subscribe((sender, args) =>
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
        let groupedDataPortSet: ObservableGroup<DataPortViewModel>[]

        groupedDataPortSet = []

        // inputs
        if (this.OneDasInputModuleSelector())
        {
            index = 0

            groupedDataPortSet = groupedDataPortSet.concat(this.OneDasInputModuleSelector().ModuleSet().map(oneDasModule =>
            {
                let group: ObservableGroup<DataPortViewModel>

                group = new ObservableGroup<DataPortViewModel>(oneDasModule.ToString(), this.CreateDataPortSet(oneDasModule, index))
                index += oneDasModule.Size();

                return group
            }))
        }

        // outputs
        if (this.OneDasOutputModuleSelector())
        {
            index = 0

            groupedDataPortSet = groupedDataPortSet.concat(this.OneDasOutputModuleSelector().ModuleSet().map(oneDasModule =>
            {
                let group: ObservableGroup<DataPortViewModel>

                group = new ObservableGroup<DataPortViewModel>(oneDasModule.ToString(), this.CreateDataPortSet(oneDasModule, index))
                index += oneDasModule.Size();

                return group
            }))
        }

        this.GroupedDataPortSet(groupedDataPortSet)
        this.DataPortSet(MapMany(this.GroupedDataPortSet(), group => group.Members()))
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
                OneDasDataType: <OneDasDataTypeEnum>oneDasModule.DataType(),
                DataDirection: <DataDirectionEnum>oneDasModule.DataDirection()
            }
        }).map(dataPortModel => new DataPortViewModel(dataPortModel, this))
    }
}