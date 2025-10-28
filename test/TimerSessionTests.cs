using ui.Components.Models;
using WorkNote = DataEntities.WorkNote;

namespace test
{
    public class TimerSessionTests
    {
        [Fact]
        public void TimerSession_Constructor_FromWorkNote_Should_SetProperties()
        {
            // Arrange
            var workNote = new WorkNote
            {
                Id = 1,
                Description = "Test work note",
                RecordedAtTimeUtc = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var timerSession = new TimerSession(workNote);

            // Assert
            Assert.Equal("Work", timerSession.Message);
            Assert.Equal(workNote.RecordedAtTimeUtc.ToUniversalTime(), timerSession.CompletedAtUtc);
            Assert.Equal(workNote.Description, timerSession.WorkNotes);
            Assert.Equal(workNote.RecordedAtTimeUtc.ToLocalTime(), timerSession.CompletedAtLocaltime);
        }

        [Fact]
        public void TimerSession_Constructor_WithNullWorkNotes_Should_SetNull()
        {
            // Arrange
            var workNote = new WorkNote
            {
                Id = 1,
                Description = null,
                RecordedAtTimeUtc = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var timerSession = new TimerSession(workNote);

            // Assert
            Assert.Equal("Work", timerSession.Message);
            Assert.Equal(workNote.RecordedAtTimeUtc.ToUniversalTime(), timerSession.CompletedAtUtc);
            Assert.Null(timerSession.WorkNotes);
            Assert.Equal(workNote.RecordedAtTimeUtc.ToLocalTime(), timerSession.CompletedAtLocaltime);
        }

        [Fact]
        public void TimerSession_Constructor_WithAllParameters_Should_SetProperties()
        {
            // Arrange
            var utcTime = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var localTime = utcTime.ToLocalTime();
            var message = "Test message";
            var workNotes = "Test work notes";

            // Act
            var timerSession = new TimerSession(message, utcTime, workNotes);

            // Assert
            Assert.Equal(message, timerSession.Message);
            Assert.Equal(utcTime, timerSession.CompletedAtUtc);
            Assert.Equal(workNotes, timerSession.WorkNotes);
            Assert.Equal(localTime, timerSession.CompletedAtLocaltime);
        }

        [Fact]
        public void TimerSession_CompletedAtLocaltime_Should_Be_Correctly_Computed()
        {
            // Arrange
            var utcTime = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var localTime = utcTime.ToLocalTime();

            // Act
            var timerSession = new TimerSession("Test", utcTime, null);

            // Assert
            Assert.Equal(localTime, timerSession.CompletedAtLocaltime);
        }
    }
}
