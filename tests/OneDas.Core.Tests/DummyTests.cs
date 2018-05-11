using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NuGet.PackageManagement;
using OneDas.Extensibility;
using OneDas.Extensibility.PackageManagement;
using Xunit;

namespace OneDas.Core.Tests
{ 
    public class DummyTests
    {
        [Fact(Skip="Incomplete")]
        public async void OneDasPackageManagerCreatesAssetsFile()
        {
            var extensionFactory = Mock.Of<IExtensionFactory>();
            var installationCompatiblity = Mock.Of<IInstallationCompatibility>();

            var optionsMock = new Mock<IOptions<OneDasOptions>>();
            optionsMock.SetupGet(x => x.Value).Returns(new OneDasOptions());

            var loggerFactory = new LoggerFactory();

            var packageManager = new OneDasPackageManager(extensionFactory, installationCompatiblity, optionsMock.Object, loggerFactory);
            await packageManager.InstallAsync("OneDas.Extension.DataGatewaySample", "https://www.myget.org/F/onedas/api/v3/index.json");
        }

        [Fact]
        public void FactTest()
        {
            Assert.False(true == false, "true should be equal to true");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void TheoryTest(int value)
        {
            switch (value)
            {
                case -1:
                    Assert.True(value < 1, $"{value} should be < 1");
                    break;

                case 0:
                    Assert.True(value < 1, $"{value} should be < 1");
                    break;

                default:
                    Assert.False(value < 1, $"{value} should be < 1");
                    break;
            }
        }
    }
}