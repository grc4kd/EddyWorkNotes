using System.Diagnostics;
using Eddy;

namespace test;

public class TaskTimerTest
{
    [Fact]
    public void CreateTaskTimer_WithValidValues_ShouldInitializeCorrectly()
    {
        // Given
        var timer = new TaskTimer(WorkDuration: 40, BreakDuration: 10);

        // Then
        Assert.False(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        // Timer initializes with zero RemainingTime before timer starts
        Assert.Equal(0, timer.RemainingTime);
    }

    [Fact]
    public async Task StartAsync_ShouldStartTimerCorrectly()
    {
        // Given
        TaskTimer timer = new(WorkDuration: 40, BreakDuration: 10);

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
        TaskTimer timer = new(WorkDuration: 10, BreakDuration: 5);
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
        TaskTimer timer = new(WorkDuration: 10, BreakDuration: 5);
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
        TaskTimer timer = new(WorkDuration: 1, BreakDuration: 0);
        Task task = timer.StartAsync();

        // When
        await Task.Delay(1);

        // Then
        // Timer should have original state, period has not elapsed
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.True(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        Assert.Equal(timer.WorkDuration, timer.RemainingTime);
    }

    [Fact]
    public async Task BreakTime_RemainingTimePeriodUpdatesOnBreakTime()
    {
        // Given
        TaskTimer timer = new(WorkDuration: 0, BreakDuration: 1);

        // Wait for work duration to complete
        await timer.StartAsync();

        // Then
        Assert.Equal(timer.BreakDuration, timer.RemainingTime);
    }

    [Fact]
    public async Task BreakTime_TimerCompletesAfterBreakDuration()
    {
        // Given
        TaskTimer timer = new(WorkDuration: 0, BreakDuration: 0);

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
        TaskTimer timer = new(WorkDuration: 0, BreakDuration: 0);

        // When - Wait for one work/break cycle
        await timer.StartAsync();
        await timer.StartAsync();

        // Then
        Assert.True(timer.IsWorkTime);
        Assert.Equal(timer.WorkDuration, timer.RemainingTime, 0.001d);
    }

    [Fact]
    public async Task OnTimeElapsed_ShouldTriggerEventWhenTimerStarts()
    {
        // Given
        TaskTimer timer = new(WorkDuration: 0, BreakDuration: 0);

        bool timeElapsed = false;

        void onTimeElapsed(object? sender, EventArgs eventArgs) => timeElapsed = true;

        timer.TimeElapsed += onTimeElapsed;

        try
        {
            // When
            await timer.StartAsync();

            // Then
            Assert.True(timeElapsed);
            Assert.True(timer.IsRunning);
        }
        finally
        {
            timer.TimeElapsed -= onTimeElapsed;
        }
    }
    
    [Fact]
    public async Task OnTimerCompleted_ShouldTriggerEventWhenTimerIsCancelled()
    {
        // Given
        TaskTimer timer = new(WorkDuration: 1, BreakDuration: 0);

        bool timerCompleted = false;

        void onTimerCompleted(object? sender, EventArgs eventArgs) => timerCompleted = true;

        timer.TimerCompleted += onTimerCompleted;

        try
        {
            // When
            var task = Task.Run(timer.StartAsync);
            timer.Cancel();
            await task;

            // Then
            Assert.False(task.IsCanceled);
            // Timer stops with existing state
            Assert.True(timer.IsRunning);
            // Timer marked as completed when disposed
            Assert.True(timerCompleted);
        }
        finally
        {
            timer.TimerCompleted -= onTimerCompleted;
        }
    }
}
