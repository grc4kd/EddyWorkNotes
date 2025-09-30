using Microsoft.Extensions.Logging;

namespace Eddy;

public class TaskTimerService(ILogger<TaskTimerService> logger, NotifierService notifier)
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private int elapsedCount = 0;
    private readonly ILogger<TaskTimerService> logger = logger;
    private readonly NotifierService notifier = notifier;

    public async Task StartAsync(TimeSpan Period)
    {
        using var timer = new PeriodicTimer(Period);

        var startTime = DateTime.Now;
        var endTime = startTime + Period;

        logger.LogInformation("Starting task timer at local time: {t1}, ending at time: {t2}.", startTime, endTime);

        while (DateTime.Now < endTime && !cancellationTokenSource.IsCancellationRequested)
        {
            await timer.WaitForNextTickAsync(cancellationTokenSource.Token);
            logger.LogInformation("Done waiting for next tick on task timer at {now}", DateTime.Now);
        }

        elapsedCount++;
        await notifier.Update("elapsedCount", elapsedCount);
    }

    public void Cancel() => cancellationTokenSource.Cancel();
    public Task CancelAsync() => cancellationTokenSource.CancelAsync();
}