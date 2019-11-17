namespace OneDas.Extension.Mat73
{
    public class TextEntry
    {
        public TextEntry(string path, string name, string content)
        {
            this.Path = path;
            this.Name = name;
            this.Content = content;
        }

        public string Path { get; private set; }
        public string Name { get; private set; }
        public string Content { get; set; }
    }
}
