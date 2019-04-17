using OneDas.Infrastructure;
using OneDas.ProjectManagement;

namespace OneDas.Core.Serialization
{
    public interface IOneDasProjectSerializer
    {
        void Save(OneDasProjectSettings projectSettings, string filePath);

        OneDasProjectSettings Load(string filePath);

        OneDasCampaignDescription GetCampaignDescriptionFromFile(string filePath);
    }
}
