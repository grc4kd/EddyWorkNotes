using Eddy;

namespace test;

public class TestPeriodicTimer(TimeSpan period) : IPeriodicTimer
{
    public TimeSpan Period { get; } = period;
    public bool IsRunning { get; private set; } = false;

    public async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
    {
        IsRunning = true;

        // Immediately complete the timer for testing purposes
        return await ValueTask.FromResult(true);
    }
}

public class TaskTimerTest
{
    [Fact]
    public void CreateTaskTimer_WithValidValues_ShouldInitializeCorrectly()
    {
        // Given
        var WorkDuration = TimeSpan.FromMinutes(40);
        var timer = new TaskTimer(WorkDuration);

        // Then
        Assert.Equal(WorkDuration, timer.RemainingTime);
    }

    [Fact]
    public void StartAsync_ShouldStartTimerCorrectly()
    {
        // Given
        var WorkDuration = TimeSpan.FromMinutes(40);
        var timer = new TaskTimer(WorkDuration);

        // When
        Task task = timer.StartAsync();

        // Then
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.Equal(WorkDuration, timer.RemainingTime);
    }

    [Fact]
    public async Task StartAsync_WhenCancelled_ShouldCaptureRemainingTime()
    {
        // Given
        var timer = new TaskTimer(Duration: TimeSpan.FromMinutes(1));

        // When - Start and immediately cancel
        var task = timer.StartAsync();
        await timer.CancelAsync();

        // Then
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
        Assert.Equal(TaskStatus.Canceled, task.Status);
    }

    [Fact]
    public async Task StartAsync_WithCancellation_ShouldTriggerTimerCompletedEvent()
    {
        // Given
        TaskTimer timer = new(Duration: TimeSpan.FromMinutes(1));

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
        TaskTimer timer = new(Duration: TimeSpan.FromMinutes(1));

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
            // Timer marked as completed when disposed
            Assert.True(timerCompleted);
        }
        finally
        {
            timer.TimerCompleted -= onTimerCompleted;
        }
    }

    [Fact]
    public async Task StartAsync_CancelAlreadyCancelled_ShouldNotThrow()
    {
        // Given
        var timer = new TaskTimer(Duration: TimeSpan.FromMinutes(1));
        var task = timer.StartAsync();

        // When - Cancel twice
        await timer.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);

        // Second cancel call should not throw exception
        await timer.CancelAsync();

        // Then
        Assert.Equal(TaskStatus.Canceled, task.Status);
    }
}
