using Eddy;
namespace test;

public class TaskTimerServiceTest
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
        var timer = new TaskTimer(InitialTimeSpan: TimeSpan.FromMinutes(1));

        // When - Start and immediately cancel
        var task = timer.StartAsync();
        await timer.CancelAsync();

        // Then
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
        Assert.Equal(TaskStatus.Canceled, task.Status);
    }

    [Fact]
    public async Task StartAsync_WithCancellation_ShouldNotTriggerTimerCompletedEvent()
    {
        // Given
        TaskTimer timer = new(InitialTimeSpan: TimeSpan.FromMinutes(1));

        // setup a callback for the event handler that marks timer complete
        bool timerCompleted = false;
        Task onTimerCompleted(object? sender, EventArgs eventArgs) =>
            Task.FromResult(() => timerCompleted = true);

        async Task onTimerCompletedAction() => await onTimerCompleted(this, EventArgs.Empty);

        timer.TimerCompleted += onTimerCompletedAction;

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
            timer.TimerCompleted -= onTimerCompletedAction;
        }
    }

    [Fact]
    public async Task OnTimerCompleted_ShouldNotTriggerEventWhenTimerIsCancelled()
    {
        // Given
        var timer = new TestPeriodicTimer(TimeSpan.FromMinutes(1));

        // setup a callback for the event handler that marks timer complete
        bool timerCompleted = false;
        async Task onTimerCompleted(object? sender, EventArgs eventArgs) => await Task.FromResult(timer.TimerCompleted);
        async Task onTimerCompletedAction() => await onTimerCompleted(this, EventArgs.Empty);
        timer.TimerCompleted += onTimerCompletedAction;

        try
        {
            // When
            var timerTask = timer.StartAsync();
            await timer.CancelAsync();

            // Then
            Assert.False(timerCompleted);
        }
        finally
        {
            timer.TimerCompleted -= onTimerCompletedAction;
        }
    }

    [Fact]
    public async Task StartAsync_CancelAlreadyCancelled_ShouldNotThrow()
    {
        // Given
        var timer = new TaskTimer(InitialTimeSpan: TimeSpan.FromMinutes(1));
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
        var task2 = timer.StartAsync();

        // Then
        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
        Assert.Equal(TaskStatus.WaitingForActivation, task2.Status);
        Assert.Equal(WorkDuration.TotalSeconds, timer.Period.TotalSeconds, 0.01d);
    }

    [Fact]
    public async Task OnTimerCompleted_ShouldTriggerEventWhenTimerCompletesNaturally()
    {
        // Given
        var WorkDuration = TimeSpan.FromMilliseconds(1);
        var timer = new TaskTimer(WorkDuration);

        // setup a callback for the event handler that marks timer complete
        bool timerCompleted = false;
        Task onTimerCompleted(object? sender, EventArgs eventArgs)
        {
            timerCompleted = true;
            return Task.CompletedTask;
        }

        async Task onTimerCompletedAction() => await onTimerCompleted(this, EventArgs.Empty);
        timer.TimerCompleted += onTimerCompletedAction;

        try
        {
            // When - Start the timer and wait for it to complete naturally
            await timer.StartAsync();

            // Then
            Assert.True(timerCompleted);
        }
        finally
        {
            timer.TimerCompleted -= onTimerCompletedAction;
        }
    }

    [Fact]
    public async Task NewTimer_ShouldStartAfterPreviousCompletes()
    {
        // Given
        var Duration = TimeSpan.FromMilliseconds(100); // Short duration for quick test
        var timer = new TestPeriodicTimer(Duration);

        bool firstTimerCompleted = false;
        bool secondTimerStarted = false;

        Task onFirstTimerCompleted()
        {
            firstTimerCompleted = true;
            // Simulate starting a new timer here
            _ = StartNewTimer();
            return Task.CompletedTask;
        }

        async Task StartNewTimer()
        {
            var newTimer = new TestPeriodicTimer(Duration);
            newTimer.TimerCompleted += new Func<Task>(newTimer.StartAsync);
            await newTimer.StartAsync();
        }

        timer.TimerCompleted += onFirstTimerCompleted;

        try
        {
            // When - Start the first timer and wait for it to complete naturally
            await timer.StartAsync();

            // Then
            Assert.True(firstTimerCompleted);
            Assert.False(secondTimerStarted);
        }
        finally
        {
            timer.TimerCompleted -= onFirstTimerCompleted;
        }
    }
}
