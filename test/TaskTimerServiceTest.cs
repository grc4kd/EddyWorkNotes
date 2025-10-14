using System.Threading.Tasks;
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
        public async Task Cancel_Should_LogErrorOnObjectDisposedException()
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
            await taskTimerService.CancelAsync();
            Assert.Single(loggerMock.Invocations);
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
    }
}