abstract class ExtendedDataGatewayViewModel extends DataGatewayViewModelBase
{
    public OneDasModuleSelector: KnockoutObservable<OneDasModuleSelectorViewModel>
    public GroupedDataPortSet: KnockoutObservableArray<ObservableGroup<DataPortViewModel>>

    private _mapModuleNameAction: (oneDasModule: OneDasModuleViewModel) => string

    constructor(model, identification: PluginIdentificationViewModel, oneDasModuleSelector: OneDasModuleSelectorViewModel, mapModuleNameAction: (oneDasModule: OneDasModuleViewModel) => string = oneDasModule => oneDasModule.Size + "x " + EnumerationHelper.GetEnumLocalization("OneDasDataTypeEnum", oneDasModule.DataType))
    {
        super(model, identification)

        this.OneDasModuleSelector = ko.observable<OneDasModuleSelectorViewModel>(oneDasModuleSelector)
        this.GroupedDataPortSet = ko.observableArray()
        this._mapModuleNameAction = mapModuleNameAction

        this.OneDasModuleSelector().OnInputModuleSetChanged.subscribe((sender, args) =>
        {
            this.UpdateDataPortSet()
        })

        this.OneDasModuleSelector().OnOutputModuleSetChanged.subscribe((sender, args) =>
        {
            this.UpdateDataPortSet()
        })
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
        index = 0

        groupedDataPortSet = groupedDataPortSet.concat(this.OneDasModuleSelector().InputModuleSet().map(oneDasModule =>
        {
            let group: ObservableGroup<DataPortViewModel>

            group = new ObservableGroup<DataPortViewModel>(this._mapModuleNameAction(oneDasModule), this.CreateDataPortSet(oneDasModule, index))
            index += oneDasModule.Size;

            return group
        }))

        // outputs
        index = 0

        groupedDataPortSet = groupedDataPortSet.concat(this.OneDasModuleSelector().OutputModuleSet().map(oneDasModule =>
        {
            let group: ObservableGroup<DataPortViewModel>

            group = new ObservableGroup<DataPortViewModel>(this._mapModuleNameAction(oneDasModule), this.CreateDataPortSet(oneDasModule, index))
            index += oneDasModule.Size;

            return group
        }))

        this.GroupedDataPortSet(groupedDataPortSet)
        this.DataPortSet(MapMany(this.GroupedDataPortSet(), group => group.Members()))
    }

    public CreateDataPortSet(oneDasModule: OneDasModuleViewModel, index: number)
    {
        let prefix: string

        switch (oneDasModule.DataDirection)
        {
            case DataDirectionEnum.Input:
                prefix = "Input"
                break

            case DataDirectionEnum.Output:
                prefix = "Output"
                break
        }

        return Array.from(new Array(oneDasModule.Size), (x, i) => 
        {
            return {
                Name: <string>prefix + " " + (index + i),
                OneDasDataType: <OneDasDataTypeEnum>oneDasModule.DataType,
                DataDirection: <DataDirectionEnum>oneDasModule.DataDirection
            }
        }).map(dataPortModel => new DataPortViewModel(dataPortModel, this))
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.InputModuleSet = <OneDasModuleModel[]>this.OneDasModuleSelector().InputModuleSet().map(moduleModel => moduleModel.ToModel())
        model.OutputModuleSet = <OneDasModuleModel[]>this.OneDasModuleSelector().OutputModuleSet().map(moduleModel => moduleModel.ToModel())
    }
}