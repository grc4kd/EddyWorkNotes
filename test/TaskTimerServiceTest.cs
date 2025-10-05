using Eddy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;
using Moq;

namespace test
{
    public class TaskTimerServiceTests
    {
        [Fact]
        public void Cancel_Should_LogErrorOnObjectDisposedException()
        {
            // Arrange
            var loggerProvider = new DebugLoggerProvider();
            var loggerFactory = LoggerFactory.Create(opt => opt.AddProvider(loggerProvider));
            ILogger<TaskTimerService> logger = loggerFactory.CreateLogger<TaskTimerService>();

            var notifierMock = new Mock<NotifierService>();
            var loggerMock = new Mock<ILogger<TaskTimerService>>();
            var taskTimerService = new TaskTimerService(loggerMock.Object, notifierMock.Object);

            // Setup the cancellationTokenSource to throw ObjectDisposedException when Cancel is called
            taskTimerService.cancellationTokenSource.Dispose(); // Dispose it first to ensure it's disposed

            // Act & Assert
            taskTimerService.Cancel();
            Assert.Single(loggerMock.Invocations);
        }
    }
}