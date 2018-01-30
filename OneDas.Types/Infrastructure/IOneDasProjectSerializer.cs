namespace OneDas.Infrastructure
{
    public interface IOneDasProjectSerializer
    {
        void Save(OneDasProjectSettings projectSettings, string filePath);

        OneDasProjectSettings Load(string filePath);

        OneDasProjectDescription GetProjectDescriptionFromFile(string filePath);
    }
}
