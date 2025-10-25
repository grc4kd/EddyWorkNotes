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
        Assert.Equal(60 * timer.WorkMinutes, timer.RemainingSeconds);
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

        // When
        var task = timer.StartAsync();
        await Task.WhenAny(task, Task.Delay(10));
        timer.TogglePause();

        // Then
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.False(timer.IsRunning);
    }

    [Fact]
    public async Task TogglePause_WhenPaused_ShouldResume()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 10, BreakMinutes: 5);


        var task = timer.StartAsync();
        await Task.WhenAny(task, Task.Delay(10));
        timer.TogglePause();

        // Resume timer when toggle pause called a second time
        timer.TogglePause();

        // Then
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
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
        var task = timer.StartAsync();
        await Task.WhenAny(task, Task.Delay(100));

        // Then
        Assert.Equal(timer.BreakMinutes * 60, timer.RemainingSeconds);
    }

    [Fact]
    public async Task WorkPeriod_Completes_ShouldStartBreakPeriod()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 0, BreakMinutes: 1);

        // When - Complete work period
        var task = timer.StartAsync();

        // Then - Verify break period started
        await Assert.ThrowsAsync<TimeoutException>(() => task.WaitAsync(TimeSpan.FromMilliseconds(100)));
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.False(timer.IsWorkTime);
        Assert.True(timer.IsRunning);
    }

    [Fact]
    public async Task BreakPeriod_Completes_ShouldStartWorkPeriod()
    {
        // Given
        TaskTimer timer = new(BreakMinutes: 0, WorkMinutes: 1, IsWorkTime: false);

        // When - Complete break period
        await Task.WhenAny(timer.StartAsync(), Task.Delay(100));

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
        var task = timer.StartAsync();
        await Task.WhenAny(task, Task.Delay(100));

        // Then
        Assert.True(timer.IsRunning);
    }

    [Fact]
    public async Task BreakTime_TimerCyclesWorkAndBreak()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 1, BreakMinutes: 1);

        // When - Wait for one work/break cycle
        await Task.WhenAny(Task.Delay(10), timer.StartAsync(), timer.StartAsync());

        // Then
        Assert.True(timer.IsWorkTime);
        Assert.Equal(60 * timer.WorkMinutes, timer.RemainingSeconds);
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

        // When - Cancel during break
        // Complete work time first
        var task = timer.StartAsync();
        await Task.Delay(100);
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
