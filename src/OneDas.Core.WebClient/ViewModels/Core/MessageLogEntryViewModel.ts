class MessageLogEntryViewModel
{
    public Prefix: string
    public Message: string

    constructor(prefix: string, message: string)
    {
        this.Prefix = prefix
        this.Message = message
    }
}