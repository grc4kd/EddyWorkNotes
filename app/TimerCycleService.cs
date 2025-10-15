namespace Eddy;

public class TimerCycleService
{
    // start with lists of corresponding elements by index
    private static readonly List<string> CycleNames = ["Work", "Break"];
    private static readonly List<TimeSpan> CycleTimes = [TimeSpan.FromMinutes(25), TimeSpan.FromMinutes(5)];
    private int CycleId => CycleCount % CycleNames.Count;
    public int CycleCount { get; private set; } = 0;

    // use sensible defaults when null can be avoided in member accessors
    public string CurrentCycleName => CycleId < CycleNames.Count ? CycleNames[CycleId] : string.Empty;
    public TimeSpan CurrentCycleTime => CycleId < CycleTimes.Count ? CycleTimes[CycleId] : TimeSpan.FromMilliseconds(1);

    public void Next()
    {
        CycleCount++;
    }

    public void Reset()
    {
        CycleCount = 0;
    }
}