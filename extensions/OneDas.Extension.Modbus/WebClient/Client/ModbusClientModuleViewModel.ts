class ModbusClientModuleViewModel extends ModbusModuleViewModel
{
    public Validate()
    {
        super.Validate()

        if (this.DataDirection() === DataDirectionEnum.Output && this.ObjectType() !== ModbusObjectTypeEnum.HoldingRegister)
        {
            this.ErrorMessage("Only object type 'holding register' is allowed for output modules.")
        }
    }
}