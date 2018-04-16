using System.Runtime.Serialization;

namespace OneDas.Extensibility.DataGateway.DataGatewaySample
{
    [DataContract]
    [ExtensionContext(typeof(DataGatewaySampleGateway))]
    [ExtensionIdentification("DataGatewaySample", "Data-gateway sample", "For testing only.", @"WebClient.DataGatewaySampleView.html", @"WebClient.DataGatewaySample.js")]
    public class DataGatewaySampleSettings : ExtendedDataGatewayExtensionSettingsBase
    {
        //
    }
}
