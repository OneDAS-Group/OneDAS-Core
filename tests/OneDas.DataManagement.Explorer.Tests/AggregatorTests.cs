using OneDas.DataManagement.Explorer.Core;
using Xunit;

namespace OneDas.DataManagement.Explorer.Tests
{
    public class AggregatorTests
    {
        [Theory(Skip = "Repair")]

        [InlineData(AggregationMethod.Min, 0.90, new double[] { 0, 1, 2, 3, -4, 5, 6, 7, double.NaN, 2.5, 97, 12.5 }, -4)]
        [InlineData(AggregationMethod.Min, 0.99, new double[] { 0, 1, 2, 3, -4, 5, 6, 7, double.NaN, 2.5, 97, 12.5 }, double.NaN)]

        [InlineData(AggregationMethod.Max, 0.90, new double[] { 0, 1, 2, 3, -4, 5, 6, 7, double.NaN, 2.5, 97, 12.5 }, 97)]
        [InlineData(AggregationMethod.Max, 0.99, new double[] { 0, 1, 2, 3, -4, 5, 6, 7, double.NaN, 2.5, 97, 12.5 }, double.NaN)]

        [InlineData(AggregationMethod.Mean, 0.90, new double[] { 0, 1, 2, 3, -4, 5, 6, 7, double.NaN, 2.5, 97, 12.5 }, 12)]
        [InlineData(AggregationMethod.Mean, 0.99, new double[] { 0, 1, 2, 3, -4, 5, 6, 7, double.NaN, 2.5, 97, 12.5 }, double.NaN)]

        [InlineData(AggregationMethod.MeanPolar, 0.90, new double[] { 0, 1, 2, 3, -4, 5, 6, 7, double.NaN, 2.5, 97, 12.5 }, 9.248975696093401)]
        [InlineData(AggregationMethod.MeanPolar, 0.99, new double[] { 0, 1, 2, 3, -4, 5, 6, 7, double.NaN, 2.5, 97, 12.5 }, double.NaN)]

        [InlineData(AggregationMethod.Sum, 0.90, new double[] { 0, 1, 2, 3, -4, 5, 6, 7, double.NaN, 2.5, 97, 12.5 }, 132)]
        [InlineData(AggregationMethod.Sum, 0.99, new double[] { 0, 1, 2, 3, -4, 5, 6, 7, double.NaN, 2.5, 97, 12.5 }, double.NaN)]
        public void CanAggregate1(AggregationMethod method, double nanLimit, double[] data, double expected)
        {
            //// Arrange
            //var aggregator = new AggregationService(1, NullLogger.Instance, LoggerFactory.Create(_ => { }));
            //var kernelSize = data.Length;
            //var config = new AggregationConfig() { Method = method, Argument = "360" };

            //// Act
            //var actual = aggregator.ApplyAggregationFunction(config, kernelSize, data, nanLimit, NullLogger.Instance);

            //// Assert
            //Assert.Equal(expected, actual[0]);
        }

        [Theory(Skip = "Repair")]

        [InlineData(AggregationMethod.MinBitwise, 0.90, new int[] { 2, 2, 2, 3, 2, 3, 65, 2, 98, 14 }, new byte[] { 1, 1, 1, 1, 1, 1, 0, 1, 1, 1 }, 2)]
        [InlineData(AggregationMethod.MinBitwise, 0.99, new int[] { 2, 2, 2, 3, 2, 3, 65, 2, 98, 14 }, new byte[] { 1, 1, 1, 1, 1, 1, 0, 1, 1, 1 }, double.NaN)]

        [InlineData(AggregationMethod.MaxBitwise, 0.90, new int[] { 2, 2, 2, 3, 2, 3, 65, 2, 98, 14 }, new byte[] { 1, 1, 1, 1, 1, 1, 0, 1, 1, 1 }, 111)]
        [InlineData(AggregationMethod.MaxBitwise, 0.99, new int[] { 2, 2, 2, 3, 2, 3, 65, 2, 98, 14 }, new byte[] { 1, 1, 1, 1, 1, 1, 0, 1, 1, 1 }, double.NaN)]
        public void CanAggregate2(AggregationMethod method, double nanLimit, int[] data, byte[] status, double expected)
        {
            //// Arrange
            //var command = new AggregationService(1, NullLogger.Instance, LoggerFactory.Create(_ => { }));
            //var kernelSize = data.Length;
            //var config = new AggregationUnit() { Method = method, Argument = "360" };

            //// Act
            //var actual = command.ApplyAggregationFunction(config, kernelSize, data, status, nanLimit, NullLogger.Instance);

            //// Assert
            //Assert.Equal(expected, actual[0]);
        }
    }
}