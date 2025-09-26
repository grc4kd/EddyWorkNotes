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

        var time1 = DateTime.Now;
        var end = time1 + Period;

        logger.LogInformation("Starting task timer at local time: {t1}.", time1);

        while (DateTime.Now < end && !cancellationTokenSource.IsCancellationRequested)
        {
            logger.LogInformation("Tick at {now}", DateTime.Now);
            await timer.WaitForNextTickAsync(cancellationTokenSource.Token);
        }

        elapsedCount++;
        await notifier.Update("elapsedCount", elapsedCount);

        var time2 = DateTime.Now;
        logger.LogInformation("Task timer elapsed at local time: {localtime}.", time2);
    }

    public void Cancel() => cancellationTokenSource.Cancel();
    public Task CancelAsync() => cancellationTokenSource.CancelAsync();
}