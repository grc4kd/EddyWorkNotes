using ui.Components.Models;

namespace test
{
    public class TaskTimerOptionsTest
    {
        [Fact]
        public void Constructor_WithParameters_ShouldSetProperties()
        {
            // Arrange
            var workTime = TimeSpan.FromMinutes(20);
            var breakTime = TimeSpan.FromMinutes(10);

            // Act
            var options = new TaskTimerOptions(workTime, breakTime);

            // Assert
            Assert.Equal(workTime, options.WorkTimeSpan);
            Assert.Equal(breakTime, options.BreakTimeSpan);
        }

        [Fact]
        public void Constructor_WithoutParameters_ShouldUseDefaultValues()
        {
            // Act
            var options = new TaskTimerOptions();

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(25), options.WorkTimeSpan);
            Assert.Equal(TimeSpan.FromMinutes(5), options.BreakTimeSpan);
        }

        [Fact]
        public void WorkTimeSpan_ShouldBeReadOnly()
        {
            // Arrange
            var options = new TaskTimerOptions();

            // Act & Assert - Should not be able to modify the property
            Assert.Equal(TimeSpan.FromMinutes(25), options.WorkTimeSpan);
        }

        [Fact]
        public void BreakTimeSpan_ShouldBeReadOnly()
        {
            // Arrange
            var options = new TaskTimerOptions();

            // Act & Assert - Should not be able to modify the property
            Assert.Equal(TimeSpan.FromMinutes(5), options.BreakTimeSpan);
        }
    }
}
