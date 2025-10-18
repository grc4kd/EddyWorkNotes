namespace Eddy;

public class TimerCycleService
{
    // start with lists of corresponding elements by index
    private static readonly List<string> CycleNames = ["Work", "Break"];
    private static readonly List<TimeSpan> CycleTimes = [TimeSpan.FromMinutes(25), TimeSpan.FromMinutes(5)];
    private int _cycleCount = 0;

    /// <summary>
    /// Define a list of cycles, for iteration using the <see cref="Next"/> method
    /// </summary>
    public int LoopCount { get; init; } = 4;
    public int CycleId => _cycleCount % CycleNames.Count;

    public string CurrentCycleName => CycleId < CycleNames.Count ? CycleNames[CycleId] : string.Empty;
    public TimeSpan CurrentCycleTime => CycleId < CycleTimes.Count ? CycleTimes[CycleId] : TimeSpan.FromMilliseconds(1);

    public void Next()
    {
        _cycleCount++;
    }

    public void Reset()
    {
        _cycleCount = 0;
    }
}