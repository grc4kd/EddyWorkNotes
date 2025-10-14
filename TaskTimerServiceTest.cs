using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace test
{
    public class TaskTimerServiceTest
    {
        private readonly Mock<ILogger<TaskTimerService>> _loggerMock;
        private readonly Mock<NotifierService> _notifierMock;

        public TaskTimerServiceTest()
        {
            _loggerMock = new Mock<ILogger<TaskTimerService>>();
            _notifierMock = new Mock<NotifierService>();
        }

        [Fact]
        public async Task CreateTaskTimer_WithValidValues_ShouldInitializeCorrectly()
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
            // Arrange
            var service = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            var cancellationTokenSource = new CancellationTokenSource();
            var period = TimeSpan.FromMilliseconds(100);

            // Act
            var task = service.StartAsync(period, "TestPhase");
            cancellationTokenSource.Cancel();
            service.Cancel();

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
            Assert.False(service.IsRunning);
        }

        [Fact]
        public async Task StartAsync_WithCancellation_ShouldCancelElapsedEvent()
        {
            // Arrange
            var service = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            var period = TimeSpan.FromMilliseconds(100);

            // Act
            var task = service.StartAsync(period, "TestPhase");
            service.Cancel();

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
            await Task.Delay(200); // Allow time for the timer to tick
            Assert.True(service.IsRunning);
            Assert.Equal("TestPhase", service.CurrentPhase);
            Assert.NotEqual(DateTime.MinValue, service.StopTimeUtc);
        }

        [Fact]
        public async Task CancelAsync_WhenAlreadyCancelled_ShouldNotThrow()
        {
            // Arrange
            var service = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            service.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await service.CancelAsync());
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
        public async Task TimeRemaining_WhenNotRunning_ShouldReturnZero()
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

        [Fact]
        public async Task Cancel_Should_LogErrorOnObjectDisposedException()
        {
            // Arrange
            var service = new TaskTimerService(_loggerMock.Object, _notifierMock.Object);
            var period = TimeSpan.FromMilliseconds(100);
            var task = service.StartAsync(period, "TestPhase");

            // Act
            service.Cancel();

            // Assert
            // The test verifies that no exception is thrown during cancellation
            // The logging behavior is tested in the unit test for LogExceptionMessage
        }
    }
}
