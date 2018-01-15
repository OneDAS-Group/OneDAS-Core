using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneDas.Common;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OneDas.Engine.Serialization
{
    public class OneDasProjectSerializer : IOneDasProjectSerializer
    {
        IOneDasSerializer _oneDasSerializer;

        public OneDasProjectSerializer(IOneDasSerializer oneDasSerializer)
        {
            _oneDasSerializer = oneDasSerializer;
        }

        public void Save(Project project, string filePath)
        {
            string directoryPath;

            directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                string rawJson;
                JsonTextWriter jsonTextWriter;

                rawJson = _oneDasSerializer.Serialize(project);

                jsonTextWriter = new JsonTextWriter(streamWriter);
                jsonTextWriter.WriteRaw(rawJson);
            }
        }

        public Project Load(string filePath)
        {
            JObject jObject;
            Project project;

            jObject = JObject.Parse(File.ReadAllText(filePath));

            this.Upgrade(jObject);

            try
            {
                project = _oneDasSerializer.Deserialize<Project>(jObject.ToString());
                project.Validate();

                return project;
            }
            catch (Exception ex)
            {
                throw ExceptionHelper.UnwrapException(ex);
            }
        }

        public ProjectDescription GetProjectDescriptionFromFile(string filePath)
        {
            return JObject.Parse(File.ReadAllText(filePath))["Description"].ToObject<ProjectDescription>();
        }

        private void Upgrade(JObject jObject)
        {
            try
            {
                int formatVersion;

                formatVersion = jObject["Description"]["FormatVersion"].Value<int>();

                while (true)
                {
                    int finalVersion;

                    finalVersion = Project.FormatVersion;

                    if (formatVersion <= 1)
                    {
                        this.Upgrade_1(jObject);
                        formatVersion = 2;
                    }
                    else if (formatVersion <= finalVersion)
                    {
                        jObject["Description"]["FormatVersion"] = formatVersion;

                        return;
                    }
                    else
                    {
                        throw new Exception(ErrorMessage.ProjectUpgrader_InvalidProjectVersion);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception(ErrorMessage.ProjectUpgrader_ErrorLoadingProject);
            }
        }

        private void Upgrade_1(JObject jObject)
        {
            IEnumerable<JToken> jTokenSet;

            jTokenSet = jObject["ChannelHubSet"];

            jTokenSet.ToList().ForEach(jToken =>
            {
                jToken["TransferFunctionSet"] = new JArray();
            });
        }
    }
}
