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

    private PeriodicTimer timer = new(TimeSpan.FromSeconds(1));

    public async Task Wait(TaskTimerRequest request)
    {
        StopTimeUtc = DateTime.UtcNow.Add(request.Duration);

        // Validate timer object is not null before disposal
        ArgumentNullException.ThrowIfNull(timer);

        // Always call Dispose() before starting a new timer, release resources
        timer.Dispose();
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

    public void Skip()
    {
        // stop current timer now
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
        catch (ObjectDisposedException ex)
        {
            logger.LogWarning("Object was disposed during timer cancellation: {message}", ex.Message);
        }
    }
}