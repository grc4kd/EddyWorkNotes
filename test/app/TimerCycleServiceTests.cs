using Eddy;

namespace test.app;

public class TimerCycleServiceTests
{
    private static void GetActualValues(TimerCycleService service, out string CurrentCycleName, out TimeSpan CurrentCycleTime, out int CycleCount)
    {
        CurrentCycleName = service.CycleName;
        CurrentCycleTime = service.CycleTime;
        CycleCount = service.CycleCount;
    }

    [Fact]
    public void TestInitialValues()
    {
        // Arrange
        var service = new TimerCycleService();

        // Act & Assert - initial state should be first cycle (Work)
        Assert.Equal("Work", service.CycleName);
        Assert.Equal(TimeSpan.FromMinutes(25), service.CycleTime);
        Assert.Equal(0, service.CycleCount);
    }

    [Fact]
    public void TestNextCycles()
    {
        // Arrange
        var service = new TimerCycleService();

        // Act - advance through cycles
        service.Next(); // should switch to Break
        service.Next(); // should wrap around back to Work

        // Assert
        Assert.Equal("Work", service.CycleName);
        Assert.Equal(TimeSpan.FromMinutes(25), service.CycleTime);
        Assert.Equal(2, service.CycleCount);
    }

    [Fact]
    public void TestReset()
    {
        // Arrange
        var service = new TimerCycleService();
        service.Next(); // advance to Break

        // Act
        service.Reset();

        // Assert
        Assert.Equal("Work", service.CycleName);
        Assert.Equal(TimeSpan.FromMinutes(25), service.CycleTime);
        Assert.Equal(0, service.CycleCount);
    }

    [Fact]
    public void TestCurrentCycleNameWrapsCorrectly()
    {
        var service = new TimerCycleService();

        GetActualValues(service,
            out string actualCycleName1,
            out TimeSpan actualCycleTime1,
            out int actualCycleCount1);

        service.Next();

        GetActualValues(service,
            out string actualCycleName2,
            out TimeSpan actualCycleTime2,
            out int actualCycleCount2);

        service.Next();

        Assert.Equal("Work", actualCycleName1);
        Assert.Equal(TimeSpan.FromMinutes(25), actualCycleTime1);
        Assert.Equal(0, actualCycleCount1);

        Assert.Equal("Break", actualCycleName2);
        Assert.Equal(TimeSpan.FromMinutes(5), actualCycleTime2);
        Assert.Equal(1, actualCycleCount2);

        Assert.Equal("Work", service.CycleName);
        Assert.Equal(TimeSpan.FromMinutes(25), service.CycleTime);
        Assert.Equal(2, service.CycleCount);
    }

    [Fact]
    public void TestCurrentCycleTimeWrapsCorrectly()
    {
        var service = new TimerCycleService();

        GetActualValues(service,
            out var actualCycleName1,
            out var actualCycleTime1,
            out var actualCycleCount1);

        service.Next();

        GetActualValues(service,
            out var actualCycleName2,
            out var actualCycleTime2,
            out var actualCycleCount2);

        service.Next();

        Assert.Equal("Work", actualCycleName1);
        Assert.Equal(TimeSpan.FromMinutes(25), actualCycleTime1);
        Assert.Equal(0, actualCycleCount1);

        Assert.Equal("Break", actualCycleName2);
        Assert.Equal(TimeSpan.FromMinutes(5), actualCycleTime2);
        Assert.Equal(1, actualCycleCount2);

        Assert.Equal("Work", service.CycleName);
        Assert.Equal(TimeSpan.FromMinutes(25), service.CycleTime);
        Assert.Equal(2, service.CycleCount);
    }
}
