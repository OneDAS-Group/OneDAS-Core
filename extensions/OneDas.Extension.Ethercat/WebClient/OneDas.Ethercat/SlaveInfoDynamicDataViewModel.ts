class SlaveInfoDynamicDataViewModel
{
    public readonly Name: string
    public readonly Description: string
    public readonly Base64ImageData: ByteString
    public readonly PdoSet: SlavePdoViewModel[]
        
    constructor(slaveInfoDynamicDataModel: any, parent: SlaveInfoViewModel, dataGateway: DataGatewayViewModelBase)
    {
        this.Name = slaveInfoDynamicDataModel.Name
        this.Description = slaveInfoDynamicDataModel.Description
        this.Base64ImageData = slaveInfoDynamicDataModel.Base64ImageData
        this.PdoSet = slaveInfoDynamicDataModel.PdoSet.filter(slavePdoModel => slavePdoModel.SyncManager >= 0).map(slavePdoModel => new SlavePdoViewModel(slavePdoModel, parent, dataGateway))
    }
}