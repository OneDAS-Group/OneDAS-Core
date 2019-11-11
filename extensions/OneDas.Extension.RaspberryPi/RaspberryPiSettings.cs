using OneDas.Extensibility;
using System.Runtime.Serialization;

namespace OneDas.Extension.RaspberryPi
{
    [DataContract]
    [ExtensionContext(typeof(RaspberryPiGateway))]
    [ExtensionIdentification("RaspberryPi", "RaspberryPi", "Send and receive data via CAN bus", @"WebClient.RaspberryPiView.html", @"WebClient.RaspberryPi.js")]
    public class RaspberryPiSettings : ExtendedDataGatewayExtensionSettingsBase
    {
        //
    }
}
