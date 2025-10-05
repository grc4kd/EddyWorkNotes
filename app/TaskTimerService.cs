using Microsoft.Extensions.Logging;

namespace Eddy;

public class TaskTimerService(ILogger<TaskTimerService> logger, NotifierService notifier)
{
    public readonly CancellationTokenSource cancellationTokenSource = new();
    private int elapsedCount = 0;
    private readonly ILogger<TaskTimerService> logger = logger;
    private readonly NotifierService notifier = notifier;

    public async Task StartAsync(TimeSpan Period)
    {
        using var timer = new PeriodicTimer(Period);

        var startTime = DateTime.Now;
        var endTime = startTime + Period;

        logger.LogInformation("Starting task timer at local time: {t1}, ending at time: {t2}.", startTime, endTime);

        if (!cancellationTokenSource.IsCancellationRequested)
        {
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
            logger.LogError("Object was disposed during timer cancellation: {message}", ex.Message);
        }

        if (ex is AggregateException ae)
        {
            logger.LogError("Exception during timer cancellation: {message}", ae.GetBaseException());
            foreach (var ie in ae.InnerExceptions)
            {
                logger.LogError("Exception details: {message}", ie.Message);
            }
        }
    }

    public void Cancel()
    {
        try
        {
            cancellationTokenSource.Cancel();
        }
        catch (Exception ex)
        {
            LogExceptionMessage(ex);
        }
    }
}