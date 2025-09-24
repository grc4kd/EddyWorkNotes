using Eddy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace test;

public class TaskTimerServiceTest
{
    private LoggerFactory loggerFactory;

    public TaskTimerServiceTest()
    {
        loggerFactory = new LoggerFactory();
        loggerFactory.AddProvider(NullLoggerProvider.Instance);
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
        var logger = new Logger<TaskTimerService>(loggerFactory);
        var notifier = new NotifierService();
        var timer = new TaskTimerService(logger, notifier);
        string result = string.Empty;

        notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

        // When
        var task = timer.StartAsync(TimeSpan.FromMinutes(25));
        await Task.Yield();

        // Then
        // Timer should have original state, period has not elapsed
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task StartAsync_WhenCancelled_ShouldCancelTimerTask()
    {
        // Given
        var logger = new Logger<TaskTimerService>(loggerFactory);
        var notifier = new NotifierService();
        var timer = new TaskTimerService(logger, notifier);
        string result = string.Empty;

        notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

        // When - Start and immediately cancel
        var task = timer.StartAsync(TimeSpan.FromMinutes(25));
        timer.Cancel();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);

        // Then
        Assert.Equal(TaskStatus.Canceled, task.Status);
    }

    [Fact]
    public async Task StartAsync_WithCancellation_ShouldCancelElapsedEvent()
    {
        // Given
        var logger = new Logger<TaskTimerService>(loggerFactory);
        var notifier = new NotifierService();
        var timer = new TaskTimerService(logger, notifier);
        string result = string.Empty;

        notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

        // When - Start and immediately cancel
        var task = timer.StartAsync(TimeSpan.FromMinutes(25));
        timer.Cancel();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);

        // Then
        Assert.Equal(string.Empty, result);
    }
}
