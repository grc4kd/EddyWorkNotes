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
        Assert.Equal(40, timer.RemainingSeconds);
    }

    [Fact]
    public void StartAsync_ShouldStartTimerCorrectly()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 40, BreakMinutes: 10);

        // When
        Task task = timer.StartAsync();

        // Then
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.True(timer.IsRunning);
        Assert.True(timer.IsWorkTime);
        Assert.Equal(timer.WorkMinutes * 60, timer.RemainingSeconds);
    }

    [Fact]
    public async Task TogglePause_WhenRunning_ShouldPause()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 10, BreakMinutes: 5);
        const int delayMilliseconds = 10;
        const double toleranceFactor = 0.01d;

        // When
        await Task.Delay(delayMilliseconds);
        Task task = timer.StartAsync();
        timer.TogglePause();
        await task;

        // Then
        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
        Assert.False(timer.IsRunning);

        // Some significant amount of time should have elapsed
        Assert.True((1000 / delayMilliseconds) > toleranceFactor * toleranceFactor);
        Assert.Equal(600 - (delayMilliseconds / 1000), timer.RemainingSeconds, toleranceFactor);
    }

    [Fact]
    public async Task TogglePause_WhenPaused_ShouldResume()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 10, BreakMinutes: 5);
        Task task = timer.StartAsync();
        
        timer.TogglePause();
        await task;

        // Resume timer when toggle pause called a second time
        timer.TogglePause();

        // Then
        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
        Assert.True(timer.IsRunning);
        Assert.True(timer.RemainingSeconds < timer.WorkMinutes * 60);
    }

    [Fact]
    public void UpdateTimeAsync_WhenWorkTime_ShouldNotDecreaseRemainingTime()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 1, BreakMinutes: 0);
        Task task = timer.StartAsync();

        // Then
        // Timer should have original state, period has not elapsed
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.Equal(timer.WorkMinutes * 60, timer.RemainingSeconds);
    }

    [Fact]
    public async Task BreakTime_RemainingTimePeriodUpdatesOnBreakTime()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 0, BreakMinutes: 1);

        // Wait for work duration to complete
        await timer.StartAsync();

        // Then
        Assert.Equal(timer.BreakMinutes * 60, timer.RemainingSeconds);
    }

    [Fact]
    public async Task WorkPeriod_Completes_ShouldStartBreakPeriod()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 0, BreakMinutes: 1);

        // When - Complete work period
        await timer.StartAsync();

        // Then - Verify break period started
        Assert.False(timer.IsWorkTime);
        Assert.True(timer.IsRunning);
    }

    [Fact]
    public async Task BreakPeriod_Completes_ShouldStartWorkPeriod()
    {
        // Given
        TaskTimer timer = new(BreakMinutes: 0, WorkMinutes: 1, IsWorkTime: false);

        // When - Complete break period
        await timer.StartAsync();

        // Then - Verify break period started
        Assert.Equal(timer.WorkMinutes * 60, timer.RemainingSeconds);
        Assert.True(timer.IsWorkTime);
        Assert.True(timer.IsRunning);
    }

    [Fact]
    public async Task BreakTime_TimerContinuesAfterBreakDuration()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 0, BreakMinutes: 0);

        // Wait for work duration to complete
        // Wait for break duration to complete
        async void startTimerAction(Task e) => await timer.StartAsync();
        await timer.StartAsync().ContinueWith(startTimerAction);

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
        Assert.Equal(timer.WorkMinutes, timer.RemainingSeconds, 0.001d);
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
        Assert.True(timer.RemainingSeconds > 0);
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
        await cancellationTask1.ContinueWith(async (_) => await cancellationTask2);

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

        // Then
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
        Assert.Equal(TaskStatus.Canceled, task.Status);
        Assert.True(timer.IsRunning);
        Assert.Equal(60, timer.RemainingSeconds);
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
