using Microsoft.Extensions.Logging;

namespace Eddy;

public class TaskTimerService(ILogger<TaskTimerService> logger, NotifierService notifier)
{
    private readonly ILogger<TaskTimerService> logger = logger;
    private readonly NotifierService notifier = notifier;
    public readonly CancellationTokenSource cancellationTokenSource = new();
    private int elapsedCount = 0;

    public bool IsRunning { get; private set; }
    public string CurrentPhase { get; private set; } = string.Empty;
    public TimeSpan TimeRemaining
    {
        get
        {
            if (!IsRunning)
                return TimeSpan.Zero;

            var timespan = StopTimeUtc - DateTime.UtcNow;
            if (timespan > TimeSpan.Zero)
                return timespan;
            
            return TimeSpan.Zero;
        }
    }

    public DateTime StopTimeUtc { get; private set; }

    public async Task StartAsync(TimeSpan Period, string Phase)
    {
        CurrentPhase = Phase;
        IsRunning = true;
        StopTimeUtc = DateTime.UtcNow.Add(Period);

        using var timer = new PeriodicTimer(Period);

        logger.LogInformation("Time/Now[{now}]: Starting task timer for {timespan} ending at time: {time}.", DateTime.Now, Period, StopTimeUtc.ToLocalTime());

        if (!cancellationTokenSource.IsCancellationRequested)
        {
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

        IsRunning = false;
        CurrentPhase = $"Stopped after {elapsedCount} timers elapsed.";
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

    public async Task CancelAsync()
    {
        // cancel the timer using class cancellation token source
        try
        {
            await cancellationTokenSource.CancelAsync();
            IsRunning = false;
        }
        catch (OperationCanceledException ex)
        {
            // update state flags when timer is cancelled by exception
            LogExceptionMessage(ex);
            IsRunning = false;
        }
        catch (OperationCanceledException ex)
        {
            // preserve state on unexpected exception subtypes
            LogExceptionMessage(ex);
        }
        catch (Exception ex)
        {
            // log and rethrow on unknown exceptions
            LogExceptionMessage(ex);
            throw;
        }
    }
}