class TransferFunctionModel
{
    public DateTime: string
    public Type: string
    public Option: string
    public Argument: string

    constructor(dateTime: string, type: string, option: string, argument: string)
    {
        this.DateTime = dateTime
        this.Type = type
        this.Option = option
        this.Argument = argument
    }
}