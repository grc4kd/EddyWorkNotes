using System.Threading.Tasks;
using Eddy;
using Eddy.Requests;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

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
            var request = new TaskTimerRequest(TimeSpan.FromMinutes(1), "Work");

            // Act
            var task = taskTimerService.StartAsync(request);

            // Assert
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            Assert.Equal("Work", taskTimerService.  CurrentPhase);
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
            Assert.Equal(TimeSpan.Zero, taskTimerService.TimeRemaining);
        }

        [Fact]
        public async Task StartAsync_WhenCancelled_ShouldCancelTimerTask()
        {
            // Given
            var loggerMock = new Mock<ILogger<TaskTimerService>>();
            var notifier = new NotifierService();
            var taskTimerService = new TaskTimerService(loggerMock.Object, notifier);
            var request = new TaskTimerRequest(TimeSpan.FromMinutes(25), "Work");
            
            string result = string.Empty;
            notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

            // When
            var task = taskTimerService.StartAsync(request);
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
            var request = new TaskTimerRequest(TimeSpan.FromMinutes(5), "TestPhase");

            // Act
            var task = taskTimerService.StartAsync(request);
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
            var phase = "TestPhase";
            var request = new TaskTimerRequest(TimeSpan.FromMilliseconds(100), phase);

            // Act
            _ = taskTimerService.StartAsync(request);

            // Assert
            Assert.True(taskTimerService.IsRunning);
            Assert.Equal(phase, taskTimerService.CurrentPhase);
            Assert.NotEqual(DateTime.MinValue, taskTimerService.StopTimeUtc);
        }

        [Fact]
        public async Task UpdateTimeAsync_WhenWorkTime_ShouldNotDecreaseRemainingTime()
        {
            // Given
            int expectedSeconds = 1500;
            var notifier = new NotifierService();
            var timer = new TaskTimerService(_loggerMock.Object, notifier);
            var request = new TaskTimerRequest(TimeSpan.FromSeconds(expectedSeconds), "Work");

            string result = string.Empty;

            notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

            // When
            var task = timer.StartAsync(request);
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
            var period = TimeSpan.FromMilliseconds(1000);
            var request = new TaskTimerRequest(period, "TestPhase");

            // Act
            _ = taskTimerService.StartAsync(request);
            await Task.Yield();
            var result = taskTimerService.TimeRemaining;

            // Assert
            Assert.True(result > TimeSpan.Zero);
            Assert.True(result <= period);
        }

        [Fact]
        public async Task CreateTaskTimer_WithRequestObject_ShouldInitializeTimerService()
        {
            // Given
            DateTime testStartUtcTime = DateTime.UtcNow;
            var taskTimerService = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            var duration = TimeSpan.FromMinutes(25);
            var breakDuration = TimeSpan.FromMinutes(5);
            var taskTimerRequest = new TaskTimerRequest(duration, "Phase Title");

            // When
            var timerTask = taskTimerService.StartAsync(taskTimerRequest);
            await Task.Yield(); // Yield control to test thread immediately

            // Then
            Assert.Equal(TaskStatus.WaitingForActivation, timerTask.Status);
            Assert.True(taskTimerService.IsRunning);
            Assert.True(duration >= taskTimerService.TimeRemaining);
            Assert.True(taskTimerService.StopTimeUtc > testStartUtcTime);
        }

        [Fact]
        public async Task StartAsync_WhenTimerCompletes_ShouldUpdateElapsedCount()
        {
            // Given
            var notifier = new NotifierService();
            var taskTimerService = new TaskTimerService(_loggerMock.Object, notifier);

            var shortDuration = TimeSpan.FromMilliseconds(100);
            var request = new TaskTimerRequest(shortDuration, "Test Phase");

            string result = string.Empty;
            notifier.Notify += new(async (s, i) => result = await Task.FromResult($"{s} {i}"));

            // When
            var task = taskTimerService.StartAsync(request);
            await task; // Wait for timer to complete

            // Then
            Assert.Equal("elapsedCount 1", result);
        }

        [Fact]
        public void Pause_WhenCalled_PausesTaskTimer()
        {
            // Given
            DateTime testStartUtcTime = DateTime.UtcNow;
            var notifier = new NotifierService();
            var taskTimerService = new TaskTimerService(_loggerMock.Object, notifier);
            var request = new TaskTimerRequest(TimeSpan.FromMinutes(5), "TestPhase");

            // When
            var task = taskTimerService.StartAsync(request);
            var wasRunning = taskTimerService.IsRunning;
            taskTimerService.Pause();

            // Then
            Assert.True(wasRunning);
            Assert.False(taskTimerService.IsRunning);
            Assert.True(taskTimerService.StopTimeUtc > testStartUtcTime);
            Assert.False(task.IsCanceled);
        }

        [Fact]
        public async Task Skip_WhenCalled_SkipsRemainingTime()
        {
            // Given
            DateTime testStartUtcTime = DateTime.UtcNow;
            var notifier = new NotifierService();
            var taskTimerService = new TaskTimerService(_loggerMock.Object, notifier);
            var request = new TaskTimerRequest(TimeSpan.FromMinutes(5), "TestPhase");

            // When
            var task = taskTimerService.StartAsync(request);
            var wasRunning = taskTimerService.IsRunning;
            await taskTimerService.SkipAsync();
        
            // Then
            Assert.True(wasRunning);
            Assert.False(taskTimerService.IsRunning);
            Assert.True(taskTimerService.StopTimeUtc > testStartUtcTime);
            Assert.False(task.IsCanceled);
        }
    }
}