using OneDas.Infrastructure;

namespace OneDas.Engine.Core
{
    public interface IOneDasProjectSerializer
    {
        void Save(OneDasProjectSettings projectSettings, string filePath);

        OneDasProjectSettings Load(string filePath);

        OneDasCampaignDescription GetCampaignDescriptionFromFile(string filePath);
    }
}
