namespace OneDas.Extensibility
{
    public class ActionResponse
    {
        public ActionResponse(object data)
        {
            this.Data = data;
        }

        public object Data { get; private set; }
    }
}
