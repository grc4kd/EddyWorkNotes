using Microsoft.Extensions.Logging;

namespace Eddy;

public class TaskTimerService(ILogger<TaskTimerService> logger, NotifierService notifier)
{
    private PeriodicTimer? timer;
    public readonly CancellationTokenSource cancellationTokenSource = new();
    private int elapsedCount = 0;

    public bool IsRunning { get; private set; } = false;
    public string CurrentPhase { get; private set; } = string.Empty;
    public TimeSpan TimeRemaining => StopTimeUtc - DateTime.UtcNow;
    public DateTime StopTimeUtc { get; private set; } = DateTime.UtcNow;

    public async Task StartAsync(TimeSpan Period, string Phase)
    {
        CurrentPhase = Phase;
        StopTimeUtc = DateTime.UtcNow.Add(Period);

        timer = new PeriodicTimer(Period);

        logger.LogInformation("Time/Now[{now}]: Starting task timer for {timespan} ending at time: {time}.", DateTime.Now, Period, StopTimeUtc.ToLocalTime());

        if (!cancellationTokenSource.IsCancellationRequested)
        {
            IsRunning = true;
            await notifier.Update("timerStarted", (int)Period.TotalSeconds);

            try
            {
                if (await timer.WaitForNextTickAsync(cancellationTokenSource.Token))
                {
                    elapsedCount++;
                    await notifier.Update("elapsedCount", elapsedCount);
                }
            }
            catch (OperationCanceledException) when (cancellationTokenSource.Token.IsCancellationRequested)
            {
                logger.LogWarning("TaskTimer was cancelled at {now}.", DateTime.Now);
            }
        }

        logger.LogInformation("Timer finished running at local time: {now}", DateTime.Now);
    }

    private void LogExceptionMessage(Exception ex, LogLevel logLevel = LogLevel.Error)
    {
        if (ex is ObjectDisposedException)
        {
            logger.Log(logLevel, "Object was disposed during timer cancellation: {message}", ex.Message);
        }

        if (ex is AggregateException ae)
        {
            logger.LogError("Exception during timer cancellation: {message}", ae.GetBaseException());
            foreach (var ie in ae.InnerExceptions)
            {
                logger.Log(logLevel, "Exception details: {message}", ie.Message);
            }
        }
    }

    public async Task SkipAsync()
    {
        // stop and clear any time remaining
        IsRunning = false;
        timer?.Dispose();

        // update count and notification service
        elapsedCount++;
        await notifier.Update("elapsedCount", elapsedCount);
    }

    public async Task CancelAsync()
    {
        // cancel the timer using class cancellation token source
        try
        {
            await cancellationTokenSource.CancelAsync();
        }
        catch (OperationCanceledException ex)
        {
            LogExceptionMessage(ex);
        }
        catch (OperationCanceledException ex)
        {
            // log and rethrow on unknown exceptions
            LogExceptionMessage(ex);
            throw;
        }
        catch (Exception ex)
        {
            // log and rethrow on unknown exceptions
            LogExceptionMessage(ex);
            throw;
        }
    }
}