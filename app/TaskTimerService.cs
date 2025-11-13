using Eddy.Requests;
using Microsoft.Extensions.Logging;

namespace Eddy;

public class TaskTimerService(ILogger<TaskTimerService> logger, NotifierService notifier, CancellationTokenSource? cancellationTokenSource = null)
{
    private int ElapsedCount = 0;
    private PeriodicTimer timer = new(TimeSpan.FromMilliseconds(1));

    public bool IsRunning { get; private set; } = false;
    public DateTime StopTimeUtc { get; private set; } = DateTime.UtcNow;
    public TimeSpan TimeRemaining => DateTime.UtcNow >= StopTimeUtc ? TimeSpan.Zero : StopTimeUtc - DateTime.UtcNow;

    public CancellationTokenSource CancellationTokenSource { get; } = cancellationTokenSource ?? new();

    public async Task Wait(TaskTimerRequest request)
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

            ElapsedCount++;
            await notifier.Update("elapsedCount", ElapsedCount);
        }

        logger.LogInformation("Timer finished running at local time: {now}", DateTime.Now);
    }

    public void Stop()
    {
        IsRunning = false;
        StopTimeUtc = DateTime.UtcNow;
        logger.LogInformation("timer paused at local time: {now}", DateTime.Now);
    }

    public void Skip()
    {
        IsRunning = false;
        StopTimeUtc = DateTime.UtcNow;
        timer.Dispose();
        logger.LogInformation("timer skipped at local time: {now}", DateTime.Now);
    }

    public void Reset()
    {
        IsRunning = false;
        StopTimeUtc = DateTime.UtcNow;
        ElapsedCount = 0;
        logger.LogInformation("timer reset at local time: {now}", DateTime.Now);
    }
}