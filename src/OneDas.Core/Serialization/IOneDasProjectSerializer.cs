using OneDas.Core.ProjectManagement;
using OneDas.Infrastructure;

namespace OneDas.Core.Serialization
{
    public interface IOneDasProjectSerializer
    {
        void Save(OneDasProjectSettings projectSettings, string filePath);

        OneDasProjectSettings Load(string filePath);

        OneDasCampaignDescription GetCampaignDescriptionFromFile(string filePath);
    }
}
