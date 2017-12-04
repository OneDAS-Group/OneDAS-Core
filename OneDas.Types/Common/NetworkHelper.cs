using System.Net;

namespace OneDas.Common
{
    public static class NetworkHelper
    {
        public static bool ValidateIPv4(string ipAddressString, out IPAddress ipAddress)
        {
            ipAddress = null;

            if (ipAddressString.Split('.').Length != 4)
            {
                return false;
            }

            return IPAddress.TryParse(ipAddressString, out ipAddress);
        }
    }
}
