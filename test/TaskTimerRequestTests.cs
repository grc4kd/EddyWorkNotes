using Eddy.Requests;

namespace test;

public class TaskTimerRequestTests
{
    [Fact]
    public void Initialize_WithValidValues_ShouldSetProperties()
    {
        // Given
        var workDuration = TimeSpan.FromMinutes(25);
        var breakDuration = TimeSpan.FromMinutes(5);

        // Act
        var request = new TaskTimerRequest(workDuration, breakDuration);

        // Assert
        Assert.Equal(workDuration, request.WorkTimeSpan);
        Assert.Equal(breakDuration, request.BreakTimeSpan);
        Assert.Equal(workDuration + breakDuration, request.TotalDuration);
    }
}