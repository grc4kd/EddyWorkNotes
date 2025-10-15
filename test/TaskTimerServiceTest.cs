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
            var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);

            // Act & Assert
            Assert.False(taskTimerService.IsRunning);
            Assert.Equal(string.Empty, taskTimerService.CurrentPhase);
            Assert.Equal(TimeSpan.Zero, taskTimerService.TimeRemaining);
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
        public async Task StartAsync_WithCancellation_ShouldLeaveTimerRunning()
        {
            // Arrange
            var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);

            // Act
            var task = taskTimerService.StartAsync(TimeSpan.FromMinutes(5), "TestPhase");
            await taskTimerService.CancelAsync();

            // Assert
            await task; // Should complete without exception
            Assert.True(task.IsCompletedSuccessfully);
            Assert.True(taskTimerService.IsRunning);
        }

        [Fact]
        public void StartAsync_WhenNotCancelled_ShouldUpdateState()
        {
            // Arrange
            var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            var period = TimeSpan.FromMilliseconds(100);

            // Act
            var task = taskTimerService.StartAsync(period, "TestPhase");

            // Assert
            Assert.True(taskTimerService.IsRunning);
            Assert.Equal("TestPhase", taskTimerService.CurrentPhase);
            Assert.NotEqual(DateTime.MinValue, taskTimerService.StopTimeUtc);
        }

        [Fact]
        public async Task UpdateTimeAsync_WhenWorkTime_ShouldNotDecreaseRemainingTime()
        {
            // Given
            int expectedSeconds = 1500;
            var notifier = new NotifierService();
            var timer = new TaskTimerService(_loggerMock.Object, notifier);
            string result = string.Empty;

            notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

            // When
            var task = timer.StartAsync(TimeSpan.FromSeconds(expectedSeconds), "Work");
            await Task.Yield();

            // Then
            // Timer should have original state, period has not elapsed
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            Assert.Equal($"timerStarted {expectedSeconds}", result);
        }

        [Fact]
        public async Task CancelAsync_WhenAlreadyCancelled_ShouldNotThrow()
        {
            // Arrange
            var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            var cancellationTask = taskTimerService.CancelAsync();
            var cancellationTask2 = taskTimerService.CancelAsync();

            // Act & Assert
            await cancellationTask;
            await cancellationTask2;

            Assert.True(cancellationTask.IsCompletedSuccessfully);
            Assert.True(cancellationTask2.IsCompletedSuccessfully);
        }

        [Fact]
        public void TimeRemaining_WhenNotRunning_ShouldReturnZero()
        {
            // Arrange
            var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);

            // Act
            var result = taskTimerService.TimeRemaining;

            // Assert
            Assert.Equal(TimeSpan.Zero, result);
        }

        [Fact]
        public async Task TimeRemaining_WhenRunning_ShouldReturnRemainingTime()
        {
            // Arrange
            var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            var period = TimeSpan.FromMilliseconds(100);
            _ = taskTimerService.StartAsync(period, "TestPhase");

            // Act
            await Task.Delay(50); // Allow time for the timer to tick
            var result = taskTimerService.TimeRemaining;

            // Assert
            Assert.True(result > TimeSpan.Zero);
            Assert.True(result <= period);
        }
    }
}