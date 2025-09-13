using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Eddy;
using Xunit;

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
        TaskTimer timer = new() { WorkDuration = 40 };

        // When
        Task task = timer.StartAsync();
        await Task.Yield();

        // Then
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.True(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        Assert.Equal(40, timer.RemainingTime);
    }

    [Fact]
    public async Task TogglePause_WhenRunning_ShouldPause()
    {
        // Given
        var timer = new TaskTimer { WorkDuration = 10 };
        Task task = timer.StartAsync();
        await Task.Yield();

        // When
        timer.TogglePause();

        // Then
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.False(timer.IsRunning);
        Assert.Equal(10, timer.RemainingTime); // Time should not advance while paused
    }

    [Fact]
    public async Task TogglePause_WhenPaused_ShouldResume()
    {
        // Given
        TaskTimer timer = new() { WorkDuration = 10 };
        Task task = timer.StartAsync();
        await Task.Yield();
        timer.TogglePause();

        // When second pause called
        timer.TogglePause();

        // Then
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.True(timer.IsRunning);
        Assert.Equal(10, timer.RemainingTime); // Time should remain same when resumed
    }

    [Fact]
    public async Task UpdateTimeAsync_WhenWorkTime_ShouldNotDecreaseRemainingTime()
    {
        // Given
        TaskTimer timer = new() { WorkDuration = 10 };
        Task task = timer.StartAsync();

        // When
        // Let time advance by 2 seconds
        await Assert.ThrowsAsync<TimeoutException>(async () => await task.WaitAsync(TimeSpan.FromSeconds(2)));

        // Then
        // Timer should have original state, period has not elapsed
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.True(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        Assert.Equal(10, timer.RemainingTime);
    }

    [Fact]
    public async Task BreakTime_ShouldTransitionToBreakTimeAfterWorkDuration()
    {
        // Given
        TaskTimer timer = new() { WorkDuration = 10, BreakDuration = 5 };
        Task task = timer.StartAsync();
        await Task.Yield();

        // Wait for work duration to complete
        await task.WaitAsync(TimeSpan.FromSeconds(10));

        // Then
        Assert.False(timer.IsWorkTime);
        Assert.Equal(5, timer.RemainingTime);
    }

    [Fact]
    public async Task BreakTime_RemainingTimeDecreasesDuringBreak()
    {
        // Given
        TaskTimer timer = new() { WorkDuration = 10, BreakDuration = 5 };
        Task task = timer.StartAsync();
        await Task.Yield();

        // Wait for work duration to complete
        await task.WaitAsync(TimeSpan.FromSeconds(10));

        // When
        await task.WaitAsync(TimeSpan.FromSeconds(2));

        // Then
        Assert.Equal(3, timer.RemainingTime);
    }

    [Fact]
    public async Task BreakTime_TimerCompletesAfterBreakDuration()
    {
        // Given
        TaskTimer timer = new() { WorkDuration = 10, BreakDuration = 5 };
        Task task = timer.StartAsync();
        await Task.Yield();

        // Wait for work duration to complete
        await task.WaitAsync(TimeSpan.FromSeconds(10));

        // When
        await task.WaitAsync(TimeSpan.FromSeconds(5));

        // Then
        Assert.False(timer.IsRunning);
    }

    [Fact]
    public async Task BreakTime_TimerCyclesWorkAndBreak()
    {
        // Given
        TaskTimer timer = new() { WorkDuration = 5, BreakDuration = 2 };
        Task task = timer.StartAsync();
        await Task.Yield();

        // When - Wait for one work/break cycle
        await task.WaitAsync(TimeSpan.FromSeconds(7));

        // Then
        Assert.True(timer.IsWorkTime);
        Assert.Equal(5, timer.RemainingTime);
    }
}
