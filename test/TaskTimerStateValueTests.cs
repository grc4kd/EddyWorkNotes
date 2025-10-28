using ui.Components.Models;

namespace test
{
    public class TaskTimerStateValueTests
    {
        [Fact]
        public void Stopped_Should_ReturnStoppedString()
        {
            Assert.Equal("Stopped", TaskTimerState.Stopped.ToString());
        }

        [Fact]
        public void Running_Should_ReturnRunningString()
        {
            Assert.Equal("Running", TaskTimerState.Running.ToString());
        }

        [Fact]
        public void Equality_Should_WorkCorrectly()
        {
            var stopped1 = new TaskTimerStateValue();
            var stopped2 = new TaskTimerStateValue();
            var running = new TaskTimerStateValue(TaskTimerState.Running);

            Assert.Equal(stopped1, stopped2);
            Assert.NotEqual(stopped1, running);
        }
    }
}
