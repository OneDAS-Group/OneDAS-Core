using System.Runtime.Serialization;

namespace OneDas.Plugin.DataGateway.DataGatewaySample
{
    [DataContract]
    [PluginContext(typeof(DataGatewaySampleGateway))]
    [PluginIdentification("DataGatewaySample", "Data-gateway sample", "For testing only.", @"WebClient.DataGatewaySampleView.html", @"WebClient.DataGatewaySample.js")]
    public class DataGatewaySampleSettings : ExtendedDataGatewayPluginSettingsBase
    {
        //
    }
}
