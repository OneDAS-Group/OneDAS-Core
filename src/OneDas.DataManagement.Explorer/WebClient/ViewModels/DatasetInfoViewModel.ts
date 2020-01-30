/// <reference path="InfoViewModelBase.ts"/>

class DatasetViewModel extends InfoViewModelBase
{
    public IsSelected: KnockoutObservable<boolean>
    public IsVisible: KnockoutObservable<boolean>

    private _onIsSelectedChanged: EventDispatcher<DatasetViewModel, boolean>

    constructor(datasetInfoModel: any, parent: InfoViewModelBase)
    {
        super(datasetInfoModel.Name, parent)

        this.IsSelected = ko.observable<boolean>(false)
        this.IsVisible = ko.observable<boolean>()

        this._onIsSelectedChanged = new EventDispatcher<DatasetViewModel, boolean>();
    }  

    //
    get OnIsSelectedChanged(): IEvent<DatasetViewModel, boolean>
    {
        return this._onIsSelectedChanged;
    }

    // methods
    public GetDisplayName(): string
    {
        return this.Name;
    }

    // commands
    public ToggleIsSelected = () =>
    {
        this.IsSelected(!this.IsSelected())
        this._onIsSelectedChanged.dispatch(this, this.IsSelected())
    }
}