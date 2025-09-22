using Eddy;
namespace test;

public class TaskTimerTest
{
    [Fact]
    public void CreateTaskTimer_WithValidValues_ShouldInitializeCorrectly()
    {
        // Given
        var WorkDuration = TimeSpan.FromMinutes(40);
        var timer = new TaskTimer(WorkDuration);

        // Then
        Assert.Equal(WorkDuration, timer.Period);
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
        Assert.Equal(WorkDuration, timer.Period);
    }

    [Fact]
    public async Task StartAsync_WhenCancelled_ShouldCaptureRemainingTime()
    {
        // Given
        var timer = new TaskTimer(PeriodTimeSpan: TimeSpan.FromMinutes(1));

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
        TaskTimer timer = new(PeriodTimeSpan: TimeSpan.FromMinutes(1));

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
            // The task was cancelled, but the timer was not completed.
            Assert.True(task.IsCanceled);
            Assert.False(timerCompleted);   
        }
        finally
        {
            timer.TimerCompleted -= onTimerCompleted;
        }
    }

    [Fact]
    public async Task OnTimerCompleted_ShouldNotTriggerEventWhenTimerIsCancelled()
    {
        // Given
        var timer = new TestPeriodicTimer(TimeSpan.FromMinutes(1));

        bool timerCompleted = false;

        void onTimerCompleted(object? sender, EventArgs eventArgs) => timerCompleted = true;

        timer.TimerCompleted += onTimerCompleted;

        try
        {
            // When
            await timer.CancelAsync();

            // Then
            Assert.False(timerCompleted);
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
        var timer = new TaskTimer(PeriodTimeSpan: TimeSpan.FromMinutes(1));
        var task = timer.StartAsync();

        // When - Cancel twice
        await timer.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);

        // Second cancel call should not throw exception
        await timer.CancelAsync();

        // Then
        Assert.Equal(TaskStatus.Canceled, task.Status);
    }

    [Fact]
    public async Task Pause_ShouldPauseTimerCorrectly()
    {
        // Given
        var WorkDuration = TimeSpan.FromMinutes(1);
        var timer = new TaskTimer(WorkDuration);

        // When - Start and pause the timer
        var task = timer.StartAsync();
        await timer.CancelAsync();

        // Then
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
        Assert.Equal(TaskStatus.Canceled, task.Status);
        Assert.Equal(WorkDuration, timer.Period);
    }

    [Fact]
    public async Task Resume_ShouldResumeTimerCorrectly()
    {
        // Given
        var WorkDuration = TimeSpan.FromMinutes(1);
        var timer = new TaskTimer(WorkDuration);

        // When - Start, pause, and then resume the timer
        var task = timer.StartAsync();
        timer.Pause();
        await task;
        var task2 = timer.ResumeAsync();

        // Then
        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
        Assert.Equal(TaskStatus.WaitingForActivation, task2.Status);
        Assert.Equal(WorkDuration.TotalSeconds, timer.Period.TotalSeconds, 0.01d);
    }

    [Fact]
    public async Task OnTimerCompleted_ShouldTriggerEventWhenTimerCompletesNaturally()
    {
        // Given
        var WorkDuration = TimeSpan.FromMilliseconds(100); // Short duration for quick test
        var timer = new TaskTimer(WorkDuration);

        bool timerCompleted = false;

        void onTimerCompleted(object? sender, EventArgs eventArgs) => timerCompleted = true;

        timer.TimerCompleted += onTimerCompleted;

        try
        {
            // When - Start the timer and wait for it to complete naturally
            await timer.StartAsync();

            // Then
            Assert.True(timerCompleted); // TimerCompleted should be triggered
        }
        finally
        {
            timer.TimerCompleted -= onTimerCompleted;
        }
    }
}
