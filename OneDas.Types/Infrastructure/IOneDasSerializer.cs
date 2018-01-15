namespace OneDas.Infrastructure
{
    public interface IOneDasSerializer
    {
        string Serialize(object value);
        T Deserialize<T>(string rawJson);
        T Deserialize<T>(object jObject);
    }
}
