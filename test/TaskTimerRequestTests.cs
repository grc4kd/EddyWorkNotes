using Eddy.Requests;

namespace test;

public class TaskTimerRequestTests
{
    [Fact]
    public void Initialize_WithValidValues_ShouldSetProperties()
    {
        // Given
        var duration = TimeSpan.FromMinutes(25);
        var phase = "Work";

        // Act
        var request = new TaskTimerRequest(duration, phase);

        // Assert
        Assert.Equal(duration, request.Duration);
        Assert.Equal(phase, request.Phase);
    }
}