using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Eddy;
using Xunit;

namespace test;

public class TaskTimerServiceTest
{
    private LoggerFactory loggerFactory;

        // Then
        Assert.False(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        Assert.Equal(40, timer.RemainingTime);
    }

    [Fact]
    public async Task CreateTaskTimer_WithValidValues_ShouldInitializeCorrectly()
    {
        // Given
        var logger = new Logger<TaskTimerService>(loggerFactory);
        var notifier = new NotifierService();
        var timer = new TaskTimerService(logger, notifier);
        string result = string.Empty;

        notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

        // When
        await timer.StartAsync(TimeSpan.FromMilliseconds(1));

        // Then
        Assert.Equal("elapsedCount 1", result);
    }

    [Fact]
    public async Task UpdateTimeAsync_WhenWorkTime_ShouldNotDecreaseRemainingTime()
    {
        // Given
        TaskTimer timer = new(WorkDuration: 2, BreakDuration: 1);
        Task task = timer.StartAsync();

        // When
        // Let time advance by some seconds
        int seconds = 1;
        await Assert.ThrowsAsync<TimeoutException>(async () => await task.WaitAsync(TimeSpan.FromSeconds(seconds)));

        // Then
        // Timer should have original state, period has not elapsed
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.True(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        Assert.Equal(timer.WorkDuration, timer.RemainingTime);
        Assert.NotEqual(timer.WorkDuration, timer.RemainingTime - seconds);
    }

    [Fact]
    public async Task BreakTime_ShouldTransitionToBreakTimeAfterWorkDuration()
    {
        // Given
        TaskTimer timer = new(WorkDuration: 2, BreakDuration: 1);

        // Wait for work duration to complete
        await timer.StartAsync();

        // Then
        Assert.False(timer.IsWorkTime);
        Assert.Equal(timer.BreakDuration, timer.RemainingTime);
    }

    [Fact]
    public async Task BreakTime_RemainingTimePeriodDecreasesDuringBreak()
    {
        // Given
        TaskTimer timer = new(WorkDuration: 2, BreakDuration: 1);

        // Wait for work duration to complete
        await timer.StartAsync();

        // Then
        Assert.Equal(timer.BreakDuration, timer.RemainingTime);
    }

    [Fact]
    public async Task BreakTime_TimerCompletesAfterBreakDuration()
    {
        // Given
        TaskTimer timer = new(WorkDuration: 1, BreakDuration: 2);

        // Wait for work duration to complete
        await timer.StartAsync();

        // Wait for break duration to complete
        await timer.StartAsync();

        // Then
        Assert.False(timer.IsRunning);
    }

    [Fact]
    public async Task BreakTime_TimerCyclesWorkAndBreak()
    {
        // Given
        TaskTimer timer = new(WorkDuration: 2, BreakDuration: 1);        

        // When - Wait for one work/break cycle
        await timer.StartAsync();
        await timer.StartAsync();

        // Then
        Assert.True(timer.IsWorkTime);
        Assert.Equal(timer.WorkDuration, timer.RemainingTime);
    }
}
