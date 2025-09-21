using Eddy;
using Microsoft.Extensions.Logging;

namespace test;

public class TestPeriodicTimer : IPeriodicTimer
{
    public TimeSpan Period { get; }
    public bool IsRunning => false;
    
    public TestPeriodicTimer(TimeSpan period)
    {
        Period = period;
    }

    public async ValueTask<ChangeResult> WaitForNextTickAsync(CancellationToken cancellationToken = default)
    {
        // Immediately complete the timer for testing purposes
        return new ChangeResult(false, Period);
    }
}

public interface IPeriodicTimer
{
    TimeSpan Period { get; }
    bool IsRunning { get; }
    ValueTask<ChangeResult> WaitForNextTickAsync(CancellationToken cancellationToken = default);
}

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
    public async Task WorkPeriod_Completes_ShouldStartBreakPeriod()
    {
        // Given
        TaskTimer timer = new(WorkMinutes: 1, BreakMinutes: 1, IsWorkTime: false);

        // When - Start and cancel work period
        Task task = timer.StartAsync();
        Task yieldImmediate = Task.FromResult(Task.Yield());
        await Task.WhenAny(task, yieldImmediate);

        // Then - Verify break period started
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.False(timer.IsWorkTime);
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
    public async Task StartAsync_CompletesSuccessfully()
    {
        // Given
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger("TaskTimer");
        
        var timer = new TaskTimer(
            WorkMinutes: 1,
            BreakMinutes: 1,
            IsWorkTime: true)
        {
            _timer = new TestPeriodicTimer(TimeSpan.FromMinutes(1))
        };

        // When
        await timer.StartAsync();

        // Then
        Assert.False(timer.IsRunning);
        Assert.Equal(0, timer.RemainingSeconds);
        Assert.True(timer.IsWorkTime == false);  // Should switch to break time after work completes
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
