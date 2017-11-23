using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OneDas.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OneDas.Main.Serialization
{
    public static class ProjectSerializationHelper
    {
        // Upgrade
        private static void Upgrade(JObject jObject)
        {
            try
            {
                int formatVersion;

                formatVersion = jObject["Description"]["FormatVersion"].Value<int>();

                while (true)
                {
                    int finalVersion;
                   
                    finalVersion = Project.FormatVersion;

                    //  cancellation criteria matched
                    if (formatVersion >= finalVersion)
                    {
                        jObject["Description"]["FormatVersion"] = formatVersion;

                        return;
                    }
                    else if (formatVersion >= 1)
                    {
                        ProjectSerializationHelper.Upgrade_1(jObject);
                        formatVersion = 2;
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

        private static void Upgrade_1(JObject jObject)
        {
            IEnumerable<JToken> jTokenSet;

            jTokenSet = jObject["ChannelHubSet"];

            jTokenSet.ToList().ForEach(jToken =>
            {
                jToken["TransferFunctionSet"] = new JArray();
            });
        }

        // load / save
        public static Project Load(string filePath)
        {
            JObject jObject;
            Project project;

            jObject = JObject.Parse(File.ReadAllText(filePath));
            ProjectSerializationHelper.Upgrade(jObject);

            try
            {
                project = jObject.ToObject<Project>(SerializationHelper.JsonSerializer);
                project.Validate();

                return project;
            }
            catch (Exception ex)
            {
                throw Bootloader.UnwrapException(ex);
            }
        }

        public static string Save(this Project project, string filePath)
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

                rawJson = JsonConvert.SerializeObject(project, Formatting.Indented, SerializationHelper.CreateDefaultSerializationSettings());
                new JsonTextWriter(streamWriter).WriteRaw(rawJson);
            }

            return filePath;
        }
    }
}