using ui.Components.Models;

namespace test
{
    public class TaskTimerStateTests
    {
        [Fact]
        public void Constructor_Should_Set_TimerStatus()
        {
            var state = new TaskTimerState("Running");
            Assert.Equal("Running", state.TimerStatus);
        }

        [Fact]
        public void Constructor_Should_Use_Default_Value()
        {
            var state = new TaskTimerState();
            Assert.Equal("Stopped", state.TimerStatus);
        }

        [Fact]
        public void IsStopped_Should_Be_True_When_TimerStatus_Is_Stopped()
        {
            var state = new TaskTimerState("Stopped");
            Assert.True(state.IsStopped);
        }

        [Fact]
        public void IsStopped_Should_Be_False_When_TimerStatus_Is_Running()
        {
            var state = new TaskTimerState("Running");
            Assert.False(state.IsStopped);
        }

        [Fact]
        public void IsRunning_Should_Be_True_When_TimerStatus_Is_Running()
        {
            var state = new TaskTimerState("Running");
            Assert.True(state.IsRunning);
        }

        [Fact]
        public void IsRunning_Should_Be_False_When_TimerStatus_Is_Stopped()
        {
            var state = new TaskTimerState("Stopped");
            Assert.False(state.IsRunning);
        }

        [Fact]
        public void Stopped_Static_Property_Should_Return_Correct_Instance()
        {
            Assert.Equal("Stopped", TaskTimerState.Stopped.TimerStatus);
            Assert.True(TaskTimerState.Stopped.IsStopped);
            Assert.False(TaskTimerState.Stopped.IsRunning);
        }

        [Fact]
        public void Running_Static_Property_Should_Return_Correct_Instance()
        {
            Assert.Equal("Running", TaskTimerState.Running.TimerStatus);
            Assert.False(TaskTimerState.Running.IsStopped);
            Assert.True(TaskTimerState.Running.IsRunning);
        }

        [Fact]
        public void ToString_Should_Return_TimerStatus()
        {
            var stopped = new TaskTimerState("Stopped");
            var running = new TaskTimerState("Running");
            
            Assert.Equal("Stopped", stopped.ToString());
            Assert.Equal("Running", running.ToString());
        }

        [Fact]
        public void Equality_Should_Work_Correctly()
        {
            var stopped1 = new TaskTimerState("Stopped");
            var stopped2 = new TaskTimerState("Stopped");
            var running = new TaskTimerState("Running");
            
            Assert.Equal(stopped1, stopped2);
            Assert.NotEqual(stopped1, running);
            Assert.NotEqual(stopped1, TaskTimerState.Running);
            Assert.Equal(TaskTimerState.Stopped, new TaskTimerState("Stopped"));
        }
    }
}
