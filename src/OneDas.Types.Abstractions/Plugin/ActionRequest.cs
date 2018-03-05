using System;
using System.Runtime.Serialization;

namespace OneDas.Plugin
{
    [DataContract]
    public class ActionRequest
    {
        public ActionRequest(string pluginId, int instanceId, string methodName, object data)
        {
            this.PluginId = pluginId;
            this.InstanceId = instanceId;
            this.MethodName = methodName;
            this.Data = data;
        }


        [DataMember]
        public string PluginId { get; private set; }

        [DataMember]
        public int InstanceId { get; private set; }

        [DataMember]
        public string MethodName { get; private set; }

        [DataMember]
        public object Data { get; private set; }

        public void Validate()
        {
            string errorMessage;

            if (!OneDasUtilities.CheckNamingConvention(this.PluginId, out errorMessage))
            {
                throw new Exception($"The plugin ID is invalid: { errorMessage }");
            }

            if (this.InstanceId < 0)
            {
                throw new Exception(ErrorMessage.ActionRequest_InstanceIdInvalid);
            }

            if (!OneDasUtilities.CheckNamingConvention(this.MethodName, out errorMessage))
            {
                throw new Exception($"The method name is invalid: { errorMessage }");
            }
        }
    }
}
