namespace OneDas.Infrastructure
{
    public interface IOneDasProjectSerializer
    {
        void Save(Project project, string filePath);

        Project Load(string filePath);

        ProjectDescription GetProjectDescriptionFromFile(string filePath);
    }
}
