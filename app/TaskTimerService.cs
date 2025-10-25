using Eddy.Requests;
using Microsoft.Extensions.Logging;

namespace Eddy;

public class TaskTimerService(ILogger<TaskTimerService> logger, NotifierService notifier, CancellationTokenSource? cancellationTokenSource = null)
{
    private int elapsedCount = 0;

    public bool IsRunning { get; private set; } = false;
    public DateTime StopTimeUtc { get; private set; } = DateTime.UtcNow;
    public TimeSpan TimeRemaining => DateTime.UtcNow >= StopTimeUtc ? TimeSpan.Zero : StopTimeUtc - DateTime.UtcNow;

    public CancellationTokenSource CancellationTokenSource { get; } = cancellationTokenSource ?? new();

    public async Task StartAsync(TaskTimerRequest request)
    {
        CurrentPhase = request.Phase;
        StopTimeUtc = DateTime.UtcNow.Add(request.Duration);

        var timer = new PeriodicTimer(request.Duration);

        logger.LogInformation("Time/Now[{now}]: Starting task timer for {timespan} ending at time: {time}.", DateTime.Now, request.Duration, StopTimeUtc.ToLocalTime());

        if (!CancellationTokenSource.IsCancellationRequested)
        {
            IsRunning = true;
            await notifier.Update("timerStarted", (int)request.Duration.TotalSeconds);

            try
            {
                if (await timer.WaitForNextTickAsync(CancellationTokenSource.Token))
                {
                    elapsedCount++;
                    await notifier.Update("elapsedCount", elapsedCount);
                }
            }
            catch (OperationCanceledException) when (CancellationTokenSource.Token.IsCancellationRequested)
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

        // update count and notification service
        elapsedCount++;
        await notifier.Update("elapsedCount", elapsedCount);
    }

    public async Task CancelAsync()
    {
        // cancel the timer using class cancellation token source
        try
        {
            await CancellationTokenSource.CancelAsync();
        }
        catch (ObjectDisposedException ex)
        {
            logger.LogWarning("Object was disposed during timer cancellation: {message}", ex.Message);
        }
    }
}