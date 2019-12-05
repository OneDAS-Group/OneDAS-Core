class ModbusServerModuleViewModel extends ModbusModuleViewModel
{
    public Validate()
    {
        super.Validate()

        // because INPUT register values do not change (they are readonly for clients) it is useless to allow server side read
        if (this.DataDirection() === DataDirectionEnum.Input && this.ObjectType() !== ModbusObjectTypeEnum.HoldingRegister) {
            this.ErrorMessage("Only object type 'holding register' is allowed for input modules.")
        }
    }
}