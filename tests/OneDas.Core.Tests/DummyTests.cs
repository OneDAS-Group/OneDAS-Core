using Xunit;

namespace OneDas.Core.Tests
{ 
    public class DummyTests
    {
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