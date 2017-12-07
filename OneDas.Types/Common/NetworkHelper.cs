using System.Net;

namespace OneDas.Common
{
    public static class NetworkHelper
    {
        public static bool ValidateIPv4(string ipAddressString, out IPAddress ipAddress)
        {
            ipAddress = null;

            return IPAddress.TryParse(ipAddressString, out ipAddress) && ipAddressString.Split('.').Length == 4;
        }
    }
}
