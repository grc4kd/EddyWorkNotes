using Microsoft.Extensions.Logging;

namespace Eddy;

/// <summary>
/// The TaskTimer record type encapsulates the behavior of a work/break timer.
/// After the initial work period, a break period is started, then a new work period is started again.
/// The timer continues like this indefinetely unless interrupted by the user or an exception.
/// </summary>
/// <param name="WorkMinutes">The length of the work cycle in minutes.</param>
/// <param name="BreakMinutes">The length of the break cycle in minutes.</param>
/// <param name="IsWorkTime">Start with work time? Exposes additional state so method can be called starting with break time.</param>
public record TaskTimer(TimeSpan PeriodTimeSpan) : ITaskTimer
{
    private static readonly ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
    private readonly ILogger logger = factory.CreateLogger("TaskTimer");

    private readonly CancellationTokenSource _cts = new();
    protected DateTimeOffset LastEventTimeUtc { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the remaining time in seconds for the current period.
    /// </summary>
    public TimeSpan Period { get; set; } = PeriodTimeSpan;

    public bool IsRunning => throw new NotImplementedException();

    /// <summary>
    /// Event raised when the timer completes or is cancelled.
    /// </summary>
    public event EventHandler? TimerCompleted;

    private PeriodicTimer Timer = new(PeriodTimeSpan);

    /// <summary>
    /// Cancels the current timer operation and raises the TimerCompleted event.
    /// </summary>
    /// <returns>A task that represents the asynchronous cancellation.</returns>
    public async Task CancelAsync()
    {
        if (!_cts.IsCancellationRequested)
        {
            await _cts.CancelAsync();
        }

        OnTimerCompleted(EventArgs.Empty);
    }

    /// <summary>
    /// Raises the TimerCompleted event.
    /// </summary>
    /// <param name="e">EventArgs to pass with the event. Empty EventArgs will still raise a normal event.</param>
    public virtual void OnTimerCompleted(EventArgs e)
    {
        TimerCompleted?.Invoke(this, e);
    }

    /// <summary>
    /// Starts or restarts the timer.
    /// If cancellation was requested, skips clock time updates.
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// </summary>
    public async Task StartAsync()
    {
        LastEventTimeUtc = DateTimeOffset.UtcNow;

        Timer = new PeriodicTimer(PeriodTimeSpan);
        Period = Timer.Period;

        logger.LogInformation("Starting timer for {TimerPeriod} at {LastEventTimeUtc}.", Timer.Period, LastEventTimeUtc);

        // If cancellation was requested, skip clock time updates
        if (!_cts.IsCancellationRequested)
        {
            await Timer.WaitForNextTickAsync(_cts.Token);
            
            OnTimerCompleted(EventArgs.Empty);
        }
    }

    public void Pause()
    {
        var elapsed = DateTimeOffset.UtcNow - LastEventTimeUtc;
        Period -= elapsed;
        LastEventTimeUtc = DateTimeOffset.UtcNow;
        Timer.Dispose();
    }

    public async Task ResumeAsync()
    {
        if (Period > TimeSpan.Zero)
        {
            LastEventTimeUtc = DateTimeOffset.UtcNow;
            Timer = new PeriodicTimer(Period);
            await Timer.WaitForNextTickAsync();
        }
    }

    public ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
