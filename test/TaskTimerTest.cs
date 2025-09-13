using Eddy;

namespace test;

public class TaskTimerTest
{
    [Fact]
    public void CreateTaskTimer_WithValidValues_ShouldInitializeCorrectly()
    {
        // Given
        var timer = new TaskTimer
        {
            WorkDuration = 40,
            BreakDuration = 10
        };

        // Then
        Assert.False(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        Assert.Equal(0, timer.RemainingTime);
    }

    [Fact]
    public async Task StartAsync_ShouldStartTimerCorrectly()
    {
        // Given
        var timer = new TaskTimer { WorkDuration = 40 };
        
        // When
        await timer.StartAsync();

        // Then
        Assert.True(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        Assert.Equal(40, timer.RemainingTime);
    }

    [Fact]
    public async Task TogglePause_WhenRunning_ShouldPause()
    {
        // Given
        var timer = new TaskTimer { WorkDuration = 10 };
        await timer.StartAsync();

        // When
        timer.TogglePause();

        // Then
        Assert.False(timer.IsRunning);
        Assert.Equal(10, timer.RemainingTime); // Time should not advance while paused
    }

    [Fact]
    public async Task TogglePause_WhenPaused_ShouldResume()
    {
        // Given
        var timer = new TaskTimer { WorkDuration = 10 };
        await timer.StartAsync();
        timer.TogglePause();

        // When
        timer.TogglePause();

        // Then
        Assert.True(timer.IsRunning);
        Assert.Equal(10, timer.RemainingTime); // Time should remain same when resumed
    }

    [Fact]
    public async Task UpdateTimeAsync_WhenWorkTime_ShouldDecreaseRemainingTime()
    {
        // Given
        var timer = new TaskTimer { WorkDuration = 10 };
        await timer.StartAsync();

        // When
        // Let time advance by 2 seconds
        await Task.Delay(2000);

        // Then
        Assert.True(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        Assert.Equal(8, timer.RemainingTime); // Approximate value
    }
}
