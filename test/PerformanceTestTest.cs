using Eddy;

namespace test;

public class PerformanceTestTest
{
    [Fact]
    public void CreatePerformanceTest_WithValidValues_ShouldInitializeCorrectly()
    {
        // Arrange
        var expectedValue = 50.5f;
        var expectedMetricType = "ms";

        // Act
        var test = new PerformanceTest(expectedValue, expectedMetricType);

        // Assert
        Assert.Equal(expectedValue, test.Value);
        Assert.Equal(expectedMetricType, test.MetricType);
    }

    [Fact]
    public void ToString_Test_WithValidValues_ShouldReturnCorrectString()
    {
        // Arrange
        var test = new PerformanceTest(123.45f, "iterations");
        
        // Act
        var result = test.ToString();

        // Assert
        Assert.Equal("Metric iterations: value -> {123.45}", result);
    }

    [Fact]
    public void CreatePerformanceTest_WithNegativeValue_ShouldInitialize()
    {
        // Arrange
        var expectedValue = -42.1f;
        var expectedMetricType = "requests";

        // Act
        var test = new PerformanceTest(expectedValue, expectedMetricType);

        // Assert
        Assert.Equal(expectedValue, test.Value);
        Assert.Equal(expectedMetricType, test.MetricType);
    }

    [Fact]
    public void CreatePerformanceTest_WithZeroValue_ShouldInitialize()
    {
        // Arrange
        var expectedValue = 0f;
        var expectedMetricType = "operations";

        // Act
        var test = new PerformanceTest(expectedValue, expectedMetricType);

        // Assert
        Assert.Equal(expectedValue, test.Value);
        Assert.Equal(expectedMetricType, test.MetricType);
    }

    [Fact]
    public void CreatePerformanceTest_WithLargeValue_ShouldInitialize()
    {
        // Arrange
        var expectedValue = 99999.9f;
        var expectedMetricType = "seconds";

        // Act
        var test = new PerformanceTest(expectedValue, expectedMetricType);

        // Assert
        Assert.Equal(expectedValue, test.Value);
        Assert.Equal(expectedMetricType, test.MetricType);
    }

    [Fact]
    public void ToString_Test_WithEmptyMetricType_ShouldStillFormat()
    {
        // Arrange
        var test = new PerformanceTest(5.2f, "");

        // Act
        var result = test.ToString();

        // Assert
        Assert.Equal("Metric : value -> {5.2}", result);
    }

    [Fact]
    public void ToString_Test_WithNullMetricType_ShouldHandleNull()
    {
        // Arrange
        // null! is intentional for testing null value handling
        var test = new PerformanceTest(10.1f, null!);

        // Act
        var result = test.ToString();

        // Assert
        Assert.Equal("Metric : value -> {10.1}", result);
    }
}
