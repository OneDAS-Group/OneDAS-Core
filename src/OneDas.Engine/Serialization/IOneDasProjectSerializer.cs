using OneDas.Engine.Core;
using OneDas.Infrastructure;

namespace OneDas.Engine.Serialization
{
    public interface IOneDasProjectSerializer
    {
        void Save(OneDasProjectSettings projectSettings, string filePath);

        OneDasProjectSettings Load(string filePath);

        OneDasCampaignDescription GetCampaignDescriptionFromFile(string filePath);
    }
}
