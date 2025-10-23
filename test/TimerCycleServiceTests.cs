using Eddy;
using Xunit;

namespace test
{
    public class TimerCycleServiceTests
    {
        [Fact]
        public void TestInitialValues()
        {
            // Arrange
            var service = new TimerCycleService();
            
            // Act & Assert - initial state should be first cycle (Work)
            Assert.Equal("Work", service.CurrentCycleName);
            Assert.Equal(TimeSpan.FromMinutes(25), service.CurrentCycleTime);
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
            Assert.Equal("Work", service.CurrentCycleName);
            Assert.Equal(TimeSpan.FromMinutes(25), service.CurrentCycleTime);
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
            Assert.Equal("Work", service.CurrentCycleName);
            Assert.Equal(TimeSpan.FromMinutes(25), service.CurrentCycleTime);
        }

        [Fact]
        public void TestCurrentCycleNameWrapsCorrectly()
        {
            // Arrange
            var service = new TimerCycleService();
            
            // Act/Assert - test wrapping scenarios
            Assert.Equal("Work", service.CurrentCycleName); // 0
            service.Next(); // 1
            Assert.Equal("Break", service.CurrentCycleName);
            service.Next(); // 2 mod 2=0
            Assert.Equal("Work", service.CurrentCycleName);
        }

        [Fact]
        public void TestCurrentCycleTimeWrapsCorrectly()
        {
            // Arrange
            var service = new TimerCycleService();
            
            // Act/Assert - test wrapping scenarios
            Assert.Equal(TimeSpan.FromMinutes(25), service.CurrentCycleTime); // 0
            service.Next(); // 1
            Assert.Equal(TimeSpan.FromMinutes(5), service.CurrentCycleTime);
            service.Next(); // 2 mod 2=0
            Assert.Equal(TimeSpan.FromMinutes(25), service.CurrentCycleTime);
        }
    }
}
