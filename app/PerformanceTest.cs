namespace Eddy;

public record PerformanceTest(float Value, string MetricType)
{
    public override string ToString()
    {
        return string.Format("Metric {0}: value -> {{{1}}}", MetricType, Value);
    }
}
