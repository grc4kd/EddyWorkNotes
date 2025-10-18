using Eddy.Requests;

namespace test;

public class TaskTimerRequestTests
{
    [Fact]
    public void Initialize_WithValidValues_ShouldSetProperties()
    {
        // Given
        var duration = TimeSpan.FromMinutes(25);

        // Act
        var request = new TaskTimerRequest(duration);

        // Assert
        Assert.Equal(duration, request.Duration);
    }
}