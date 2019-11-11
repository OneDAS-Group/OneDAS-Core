class SlaveInfoViewModel
{
    // fields
    public DynamicData: KnockoutObservable<SlaveInfoDynamicDataViewModel>

    public readonly Manufacturer: number
    public readonly ProductCode: number
    public readonly Revision: number
    public readonly Csa: number
    public readonly ChildSet: SlaveInfoViewModel[]

    private _slaveExtensionSet: SlaveExtensionViewModelBase[]

    private _oldCsa: number
    private _slaveExtensionModelSet: any[]
    
    // constructors
    constructor(slaveInfoModel: any)
    {
        this.Manufacturer = slaveInfoModel.Manufacturer
        this.ProductCode = slaveInfoModel.ProductCode
        this.Revision = slaveInfoModel.Revision
        this.Csa = slaveInfoModel.Csa
        this.ChildSet = slaveInfoModel.ChildSet.map(x => new SlaveInfoViewModel(x))

        this._oldCsa = slaveInfoModel.OldCsa
        this._slaveExtensionModelSet = slaveInfoModel.SlaveExtensionSet     

        this.DynamicData = ko.observable<SlaveInfoDynamicDataViewModel>()
    }

    // properties
    public get SlaveExtensionSet(): SlaveExtensionViewModelBase[]
    {
        return this._slaveExtensionSet;
    }

    // methods
    public async InitializeAsync()
    {
        this._slaveExtensionSet = await Promise.all(this._slaveExtensionModelSet.map(async slaveExtensionModel =>
        {
            await ExtensionFactory.CreateExtensionViewModelAsync("EthercatSlaveExtension", slaveExtensionModel)

            return <SlaveExtensionViewModelBase>(await ExtensionFactory.CreateExtensionViewModelAsync("EthercatSlaveExtension", slaveExtensionModel))
        }))

        this._slaveExtensionSet.forEach(model => model.SlaveInfo = this);

        await Promise.all(this._slaveExtensionSet.map(slaveExtension => slaveExtension.InitializeAsync()))
    }

    public ToModel = () =>
    {
        return {
            Manufacturer: <number>this.Manufacturer,
            ProductCode: <number>this.ProductCode,
            Revision: <number>this.Revision,
            OldCsa: <number>this._oldCsa,
            Csa: <number>this.Csa,
            ChildSet: <any[]>this.ChildSet.map(slaveInfo => slaveInfo.ToModel()),
            SlaveExtensionSet: <any[]>this._slaveExtensionSet.map(slaveExtension => slaveExtension.ToModel())
        }
    }

    public GetOldCsa = () =>
    {
        return this._oldCsa
    }

    public ResetOldCsa = () =>
    {
        this._oldCsa = this.Csa
    }

    public GetDescendants = (includeSelf: boolean) =>
    {
        let descendantSet: SlaveInfoViewModel[]

        descendantSet = this.InternalGetDescendants()

        if (includeSelf)
        {
            descendantSet.unshift(this)
        }

        return descendantSet
    }

    private InternalGetDescendants = () =>
    {
        let descendantSet: SlaveInfoViewModel[] = []

        this.ChildSet.forEach(x => 
        {
            descendantSet.push(x)
            descendantSet.push(...x.InternalGetDescendants())
        })

        return descendantSet
    }

    public GetVariables = () =>
    {
        if (this.DynamicData())
        {
            return MapMany(this.DynamicData().PdoSet, (x: SlavePdoViewModel) => x.VariableSet)
        }
        else
        {
            return []
        }
    }

    public GetEthercatSlaveExtension = (index: number) =>
    {
        return this.SlaveExtensionSet[index]
    }

    public ToFlatModel = () =>
    {
        return {
            Manufacturer: <number>this.Manufacturer,
            ProductCode: <number>this.ProductCode,
            Revision: <number>this.Revision,
            OldCsa: <number>this._oldCsa,
            Csa: <number>this.Csa,
            ChildSet: <any[]>[],
            SlaveExtensionSet: <any[]>this._slaveExtensionSet.map(slaveExtension => slaveExtension.ToModel())
        }
    }
}