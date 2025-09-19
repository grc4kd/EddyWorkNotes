using Eddy;

namespace test;

public class TaskTimerTest
{
    [Fact]
    public void CreateTaskTimer_WithValidValues_ShouldInitializeCorrectly()
    {
        // Given
        var timer = new TaskTimer(WorkMinutes: 40, BreakMinutes: 10);

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
        TaskTimer timer = new(WorkMinutes: 40, BreakMinutes: 10);

        // When
        Task task = timer.StartAsync();
        await Task.Yield();

        // Then
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.True(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        Assert.Equal(timer.WorkMinutes * 60, timer.RemainingTime);
    }

    [Fact]
    public async Task TogglePause_WhenRunning_ShouldPause()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 10, BreakMinutes: 5);
        Task task = timer.StartAsync();

        // When
        timer.TogglePause();
        await task;

        // Then
        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
        Assert.False(timer.IsRunning);
        // Some time should have elapsed
        Assert.True(timer.RemainingTime < timer.WorkMinutes * 60);
    }

    [Fact]
    public async Task TogglePause_WhenPaused_ShouldResume()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 10, BreakMinutes: 5);
        Task task = timer.StartAsync();
        await Task.Yield();
        timer.TogglePause();

        // When second pause called
        timer.TogglePause();

        // Then
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.True(timer.IsRunning);
        Assert.True(timer.RemainingTime < timer.WorkMinutes * 60);
    }

    [Fact]
    public async Task UpdateTimeAsync_WhenWorkTime_ShouldNotDecreaseRemainingTime()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 1, BreakMinutes: 0);
        Task task = timer.StartAsync();

        // When
        await Task.Yield();

        // Then
        // Timer should have original state, period has not elapsed
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.True(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        Assert.Equal(timer.WorkMinutes * 60, timer.RemainingTime);
    }

    [Fact]
    public async Task BreakTime_RemainingTimePeriodUpdatesOnBreakTime()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 0, BreakMinutes: 1);

        // Wait for work duration to complete
        await timer.StartAsync();

        // Then
        Assert.Equal(timer.BreakMinutes * 60, timer.RemainingTime);
    }

    [Fact]
    public async Task WorkPeriod_Completes_ShouldStartBreakPeriod()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 0, BreakMinutes: 1);

        // When - Complete work period
        await timer.StartAsync();

        // Then - Verify break period started
        Assert.Equal(timer.BreakMinutes * 60, timer.RemainingTime);
        Assert.False(timer.IsWorkTime);
        Assert.True(timer.IsRunning);
    }

    [Fact]
    public async Task BreakTime_TimerContinuesAfterBreakDuration()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 0, BreakMinutes: 0);

        // Wait for work duration to complete
        await timer.StartAsync();

        // Wait for break duration to complete
        await timer.StartAsync();

        // Then
        Assert.True(timer.IsRunning);
    }

    [Fact]
    public async Task BreakTime_TimerCyclesWorkAndBreak()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 0, BreakMinutes: 0);

        // When - Wait for one work/break cycle
        await timer.StartAsync();
        await timer.StartAsync();

        // Then
        Assert.True(timer.IsWorkTime);
        Assert.Equal(timer.WorkMinutes, timer.RemainingTime, 0.001d);
    }

    [Fact]
    public async Task StartAsync_WhenCancelled_ShouldCaptureRemainingTime()
    {
        // Given
        var timer = new TaskTimer(WorkMinutes: 1, BreakMinutes: 0);

        // When - Start and immediately cancel
        var task = timer.StartAsync();
        await timer.CancelAsync();

        // Then
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
        Assert.Equal(TaskStatus.Canceled, task.Status);
        Assert.True(timer.IsRunning);
        Assert.True(timer.RemainingTime > 0);
    }

    [Fact]
    public async Task StartAsync_WithCancellation_ShouldTriggerTimerCompletedEvent()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 1, BreakMinutes: 0);

        bool timerCompleted = false;

        void onTimerCompleted(object? sender, EventArgs eventArgs) => timerCompleted = true;

        timer.TimerCompleted += onTimerCompleted;

        try
        {
            // When - Start and immediately cancel
            var task = timer.StartAsync();
            await timer.CancelAsync();

            // Then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
            Assert.True(task.IsCanceled);
            Assert.True(timer.IsRunning);
            Assert.True(timerCompleted);
        }
        finally
        {
            timer.TimerCompleted -= onTimerCompleted;
        }
    }

    [Fact]
    public async Task OnTimerCompleted_ShouldTriggerEventWhenTimerIsCancelled()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 1, BreakMinutes: 0);

        bool timerCompleted = false;

        void onTimerCompleted(object? sender, EventArgs eventArgs) => timerCompleted = true;

        timer.TimerCompleted += onTimerCompleted;

        try
        {
            // When
            var task = Task.Run(timer.StartAsync);
            await timer.CancelAsync();
            await task;

            // Then
            Assert.False(task.IsCanceled);
            // Timer stays running when cancelled
            Assert.True(timer.IsRunning);
            // Timer marked as completed when disposed
            Assert.True(timerCompleted);
        }
        finally
        {
            timer.TimerCompleted -= onTimerCompleted;
        }
    }

    [Fact]
    public async Task StartAsync_MultipleConcurrentCancellations_ShouldHandleGracefully()
    {
        // Given
        var timer = new TaskTimer(WorkMinutes: 1, BreakMinutes: 0);
        var task = timer.StartAsync();

        // When - Cancel twice concurrently
        var cancellationTask1 = timer.CancelAsync();
        var cancellationTask2 = timer.CancelAsync();
        await Task.WhenAll(cancellationTask1, cancellationTask2);

        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);

        // Then - Verify state remains consistent
        Assert.Equal(TaskStatus.RanToCompletion, cancellationTask1.Status);
        Assert.Equal(TaskStatus.RanToCompletion, cancellationTask2.Status);
        Assert.True(task.IsCanceled);
        Assert.True(timer.IsRunning);
    }

    [Fact]
    public async Task StartAsync_CancelDuringBreakTime_ShouldWorkAsExpected()
    {
        // Given
        var timer = new TaskTimer(WorkMinutes: 0, BreakMinutes: 1);
        await timer.StartAsync(); // Complete work time

        // When - Cancel during break
        var task = timer.StartAsync();
        await timer.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);

        // Then
        Assert.Equal(TaskStatus.Canceled, task.Status);
        Assert.True(timer.IsRunning);
        Assert.Equal(60, timer.RemainingTime);
    }

    [Fact]
    public async Task StartAsync_CancelAlreadyCancelled_ShouldNotThrow()
    {
        // Given
        var timer = new TaskTimer(WorkMinutes: 1, BreakMinutes: 0);
        var task = timer.StartAsync();

        // When - Cancel twice
        await timer.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);

        // Second cancel call should not throw exception
        await timer.CancelAsync();

        // Then
        Assert.Equal(TaskStatus.Canceled, task.Status);
        Assert.True(timer.IsRunning);
    }
}
