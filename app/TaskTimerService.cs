using Eddy.Requests;
using Microsoft.Extensions.Logging;

namespace Eddy;

public class TaskTimerService(ILogger<TaskTimerService> logger, NotifierService notifier, CancellationTokenSource? cancellationTokenSource = null)
{
    private int elapsedCount = 0;

    public bool IsRunning { get; private set; } = false;
    public DateTime StopTimeUtc { get; private set; } = DateTime.UtcNow;
    public TimeSpan TimeRemaining => DateTime.UtcNow >= StopTimeUtc ? TimeSpan.Zero : StopTimeUtc - DateTime.UtcNow;

    public async Task StartAsync(TaskTimerRequest request) => await StartAsync(request.Duration, request.Phase);

    private async Task StartAsync(TimeSpan Period, string Phase)
    {
        StopTimeUtc = DateTime.UtcNow.Add(request.Duration);
        timer = new PeriodicTimer(request.Duration);

        logger.LogInformation("Time/Now[{now}]: Starting task timer for {timespan} ending at time: {time}.", DateTime.Now, request.Duration, StopTimeUtc.ToLocalTime());

        IsRunning = true;
        await notifier.Update("timerStarted", (int)request.Duration.TotalSeconds);
        
        if (!CancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(CancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                logger.LogWarning("TaskTimer was cancelled at {now}. {Message}:", DateTime.Now, ex.Message);
            }

            elapsedCount++;
            await notifier.Update("elapsedCount", elapsedCount);
        }

        logger.LogInformation("Timer finished running at local time: {now}", DateTime.Now);
    }

    // Write a unit test for the Pause() method. The test should belong to `test\TaskTimerServiceTests.cs`. AI!
    public void Pause()
    {
        IsRunning = false;
        StopTimeUtc = DateTime.UtcNow;
        logger.LogInformation("timer paused at local time: {now}", DateTime.Now);
    }

    public async Task SkipAsync()
    {
        // stop and clear any time remaining
        IsRunning = false;
        timer.Dispose();
    }

    public async Task CancelAsync()
    {
        // cancel the timer using class cancellation token source
        try
        {
            await CancellationTokenSource.CancelAsync();
        }
        catch (Exception ex)
        {
            // log all caught exceptions
            if (ex is ObjectDisposedException)
            {
                logger.LogWarning("Object was disposed during timer cancellation: {message}", ex.Message);
            }

            if (ex is AggregateException ae)
            {
                logger.LogError("{ExceptionType} during timer cancellation: {Message}. Inner exception: {InnerException}", ae.GetBaseException().GetType(), ae.GetBaseException().Message, ae.InnerException);
                foreach (var ie in ae.InnerExceptions)
                {
                    logger.LogError("Exception details: {message}", ie.Message);
                }
            }

            // handle operation cancelled exceptions
            if (ex is OperationCanceledException ocex)
            {
                logger.LogInformation("Handled {exception} in handler {handlerName}. Exception message: {Message}", ocex, nameof(CancelAsync), ocex.Message);
            }

            // rethrow all user-unhandled exceptions here
            throw;
        }
    }
}