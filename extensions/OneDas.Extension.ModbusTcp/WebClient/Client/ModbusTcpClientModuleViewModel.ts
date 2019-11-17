class ModbusTcpClientModuleViewModel extends ModbusTcpModuleViewModel
{
    public Validate()
    {
        super.Validate()

        if (this.DataDirection() === DataDirectionEnum.Output && this.ObjectType() !== ModbusTcpObjectTypeEnum.HoldingRegister)
        {
            this.ErrorMessage("Only object type 'holding register' is allowed for output modules.")
        }
    }
}