namespace OneDas.Extensibility
{
    public class CustomMetadataEntry
    {
        public CustomMetadataEntry(string key, string value, CustomMetadataEntryLevel customMetadataEntryLevel)
        {
            this.Key = key;
            this.Value = value;
            this.CustomMetadataEntryLevel = CustomMetadataEntryLevel;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }
        public CustomMetadataEntryLevel CustomMetadataEntryLevel { get; private set; }
    }
}
