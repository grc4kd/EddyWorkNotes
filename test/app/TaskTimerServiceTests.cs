using Eddy;
using Eddy.Requests;
using Microsoft.Extensions.Logging;
using Moq;

namespace test.app;

public class TaskTimerServiceTests
{
    private readonly Mock<ILogger<TaskTimerService>> _loggerMock;
    private readonly Mock<NotifierService> _notifierMock;

    public TaskTimerServiceTests()
    {
        _loggerMock = new Mock<ILogger<TaskTimerService>>();
        _notifierMock = new Mock<NotifierService>();
    }

    [Fact]
    public void Start_Should_UpdateTimerState()
    {
        // Arrange
        var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
        var request = new TaskTimerRequest(TimeSpan.FromMinutes(1));

        // Act
        var task = taskTimerService.Wait(request);

        // Assert
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.True(taskTimerService.IsRunning);
    }

    [Fact]
    public void CreateTaskTimer_WithValidValues_ShouldInitializeCorrectly()
    {
        // Arrange
        var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);

        // Act & Assert
        Assert.False(taskTimerService.IsRunning);
        Assert.Equal(TimeSpan.Zero, taskTimerService.TimeRemaining);
    }

    [Fact]
    public async Task StartAsync_WhenCancelled_ShouldCancelTimerTask()
    {
        var loggerMock = new Mock<ILogger<TaskTimerService>>();
        var notifier = new NotifierService();
        var taskTimerService = new TaskTimerService(loggerMock.Object, notifier);
        var request = new TaskTimerRequest(TimeSpan.FromMinutes(25));

        string result = string.Empty;
        notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

        var task = taskTimerService.Wait(request);
        await taskTimerService.CancellationTokenSource.CancelAsync();
        await task;

        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
    }

    [Fact]
    public async Task StartAsync_WithCancellation_ShouldLeaveTimerRunning()
    {
        var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
        var request = new TaskTimerRequest(TimeSpan.FromMinutes(5));

        var task = taskTimerService.Wait(request);
        await taskTimerService.CancellationTokenSource.CancelAsync();

        await task; // Should complete without exception
        Assert.True(task.IsCompletedSuccessfully);
        Assert.True(taskTimerService.IsRunning);
    }

    [Fact]
    public void StartAsync_WhenNotCancelled_ShouldUpdateState()
    {
        var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
        var request = new TaskTimerRequest(TimeSpan.FromMilliseconds(100));

        _ = taskTimerService.Wait(request);

        Assert.True(taskTimerService.IsRunning);
        Assert.NotEqual(DateTime.MinValue, taskTimerService.StopTimeUtc);
    }

    [Fact]
    public async Task UpdateTimeAsync_WhenWorkTime_ShouldNotDecreaseRemainingTime()
    {
        int expectedSeconds = 1500;
        var notifier = new NotifierService();
        var timer = new TaskTimerService(_loggerMock.Object, notifier);
        var request = new TaskTimerRequest(TimeSpan.FromSeconds(expectedSeconds));

        string result = string.Empty;

        notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

        var task = timer.Wait(request);
        await Task.Yield();

        // Timer should have original state, period has not elapsed
        Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        Assert.Equal($"timerStarted {expectedSeconds}", result);
    }

    [Fact]
    public async Task CancelAsync_WhenAlreadyCancelled_ShouldNotThrow()
    {
        var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
        var cancellationTask = taskTimerService.CancellationTokenSource.CancelAsync();
        var cancellationTask2 = taskTimerService.CancellationTokenSource.CancelAsync();

        await cancellationTask;
        await cancellationTask2;

        Assert.True(cancellationTask.IsCompletedSuccessfully);
        Assert.True(cancellationTask2.IsCompletedSuccessfully);
    }

    [Fact]
    public void TimeRemaining_WhenNotRunning_ShouldReturnZero()
    {

        var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);


        var result = taskTimerService.TimeRemaining;

        // Assert
        Assert.Equal(TimeSpan.Zero, result);
    }

    [Fact]
    public async Task TimeRemaining_WhenRunning_ShouldReturnRemainingTime()
    {
        var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
        var period = TimeSpan.FromMilliseconds(1000);
        var request = new TaskTimerRequest(period);

        _ = taskTimerService.Wait(request);
        await Task.Yield();
        var result = taskTimerService.TimeRemaining;

        Assert.True(result > TimeSpan.Zero);
        Assert.True(result <= period);
    }

    [Fact]
    public async Task CreateTaskTimer_WithRequestObject_ShouldInitializeTimerService()
    {

        DateTime testStartUtcTime = DateTime.UtcNow;
        var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
        var duration = TimeSpan.FromMinutes(25);
        var breakDuration = TimeSpan.FromMinutes(5);
        var taskTimerRequest = new TaskTimerRequest(duration);


        var timerTask = taskTimerService.Wait(taskTimerRequest);
        await Task.Yield(); // Yield control to test thread immediately


        Assert.Equal(TaskStatus.WaitingForActivation, timerTask.Status);
        Assert.True(taskTimerService.IsRunning);
        Assert.True(duration >= taskTimerService.TimeRemaining);
        Assert.True(taskTimerService.StopTimeUtc > testStartUtcTime);
    }

    [Fact]
    public async Task StartAsync_WhenTimerCompletes_ShouldUpdateElapsedCount()
    {

        var notifier = new NotifierService();
        var taskTimerService = new TaskTimerService(_loggerMock.Object, notifier);

        var shortDuration = TimeSpan.FromMilliseconds(100);
        var request = new TaskTimerRequest(shortDuration);

        string result = string.Empty;
        notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));


        var task = taskTimerService.Wait(request);
        await task; // Wait for timer to complete


        Assert.Equal("elapsedCount 1", result);
    }

    [Fact]
    public void Pause_WhenCalled_PausesTaskTimer()
    {

        DateTime testStartUtcTime = DateTime.UtcNow;
        var notifier = new NotifierService();
        var taskTimerService = new TaskTimerService(_loggerMock.Object, notifier);
        var request = new TaskTimerRequest(TimeSpan.FromMinutes(5));


        var task = taskTimerService.Wait(request);
        var wasRunning = taskTimerService.IsRunning;
        taskTimerService.Stop();


        Assert.True(wasRunning);
        Assert.False(taskTimerService.IsRunning);
        Assert.True(taskTimerService.StopTimeUtc > testStartUtcTime);
        Assert.False(task.IsCanceled);
    }

    [Fact]
    public void CancelAsync_WhenCancellationTokenSourceIsDisposed_ShouldHandleGracefully()
    {

        var loggerMock = new Mock<ILogger<TaskTimerService>>();
        var notifierMock = new Mock<NotifierService>();
        var cancellationTokenSource = new CancellationTokenSource(1);
        var taskTimerService = new TaskTimerService(loggerMock.Object, notifierMock.Object, cancellationTokenSource);

        cancellationTokenSource.Dispose();
        var startTask = taskTimerService.Wait(new TaskTimerRequest(TimeSpan.FromMinutes(1)));

        Assert.False(startTask.IsCanceled);
        Assert.False(cancellationTokenSource.IsCancellationRequested);
    }
}