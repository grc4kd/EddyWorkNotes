using Eddy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Moq;

namespace test
{
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
        public async Task Start_Should_UpdateTimerState()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TaskTimerService>>();
            var notifierMock = new Mock<NotifierService>();
            var taskTimerService = new TaskTimerService(loggerMock.Object, notifierMock.Object);

            var workTime = TimeSpan.FromMinutes(1);
            var phase = "Work";

            // Act
            var task = taskTimerService.StartAsync(workTime, phase);

            // Assert
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            Assert.Equal(phase, taskTimerService.CurrentPhase);
            Assert.True(taskTimerService.IsRunning);

            // Cancel running timer on async task scheduler after test assertions
            await taskTimerService.CancelAsync();
        }

        [Fact]
        public void CreateTaskTimer_WithValidValues_ShouldInitializeCorrectly()
        {
            // Arrange
            var service = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);

            // Act & Assert
            Assert.False(service.IsRunning);
            Assert.Equal(string.Empty, service.CurrentPhase);
            Assert.Equal(TimeSpan.Zero, service.TimeRemaining);
            Assert.Equal(DateTime.MinValue, service.StopTimeUtc);
        }

        [Fact]
        public async Task StartAsync_WhenCancelled_ShouldCancelTimerTask()
        {
            // Given
            var loggerMock = new Mock<ILogger<TaskTimerService>>();
            var notifier = new NotifierService();
            var taskTimerService = new TaskTimerService(loggerMock.Object, notifier);
            string result = string.Empty;

            notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

            // When - Start and immediately cancel
            var task = taskTimerService.StartAsync(TimeSpan.FromMinutes(25), "Work");
            await taskTimerService.CancelAsync();
            await task;

            // Then
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
        }

        [Fact]
        public async Task StartAsync_WithCancellation_ShouldCancelElapsedEvent()
        {
            // Arrange
            var service = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);

            // Act
            var task = service.StartAsync(TimeSpan.FromMinutes(5), "TestPhase");
            await service.CancelAsync();

            // Assert
            await task; // Should complete without exception
            Assert.False(service.IsRunning);
        }

        [Fact]
        public async Task StartAsync_WhenNotCancelled_ShouldUpdateState()
        {
            // Arrange
            var service = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            var period = TimeSpan.FromMilliseconds(100);

            // Act
            var task = service.StartAsync(period, "TestPhase");

            // Assert
            Assert.True(service.IsRunning);
            Assert.Equal("TestPhase", service.CurrentPhase);
            Assert.NotEqual(DateTime.MinValue, service.StopTimeUtc);
        }

        [Fact]
        public async Task UpdateTimeAsync_WhenWorkTime_ShouldNotDecreaseRemainingTime()
        {
            // Given
            var notifier = new NotifierService();
            var timer = new TaskTimerService(_loggerMock.Object, notifier);
            string result = string.Empty;

            notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

            // When
            var task = timer.StartAsync(TimeSpan.FromMinutes(25), "Work");
            await Task.Yield();

            // Then
            // Timer should have original state, period has not elapsed
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task CancelAsync_WhenAlreadyCancelled_ShouldNotThrow()
        {
            // Arrange
            var service = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            var cancellationTask = service.CancelAsync();
            var cancellationTask2 = service.CancelAsync();

            // Act & Assert
            await cancellationTask;
            await cancellationTask2;

            Assert.True(cancellationTask.IsCompletedSuccessfully);
            Assert.True(cancellationTask2.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task CancelAsync_WhenNotCancelled_ShouldSetIsRunningFalse()
        {
            // Arrange
            var service = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            var period = TimeSpan.FromMilliseconds(100);
            var task = service.StartAsync(period, "TestPhase");

            // Act
            await service.CancelAsync();

            // Assert
            Assert.False(service.IsRunning);
        }

        [Fact]
        public void TimeRemaining_WhenNotRunning_ShouldReturnZero()
        {
            // Arrange
            var service = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);

            // Act
            var result = service.TimeRemaining;

            // Assert
            Assert.Equal(TimeSpan.Zero, result);
        }

        [Fact]
        public async Task TimeRemaining_WhenRunning_ShouldReturnRemainingTime()
        {
            // Arrange
            var service = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            var period = TimeSpan.FromMilliseconds(100);
            var task = service.StartAsync(period, "TestPhase");

            // Act
            await Task.Delay(50); // Allow time for the timer to tick
            var result = service.TimeRemaining;

            // Assert
            Assert.True(result > TimeSpan.Zero);
            Assert.True(result <= period);
        }
    }
}