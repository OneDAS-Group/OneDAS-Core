using Microsoft.Extensions.Logging.Abstractions;
using OneDas.DataManagement.Extensions;
using Xunit;

namespace OneDas.Core.Tests
{
    public class HdfDataReaderTests
    {
#warning move this to correct assembly

        [Fact]
        public void CanProvideCampaigns()
        {
            // Arrange
            var dataReader = new HdfDataReader(@"Y:\RAW\DB_MDAS", NullLogger.Instance);
            var campaign = dataReader.GetCampaign("abc");

            // Act

            // Assert
        }
    }
}