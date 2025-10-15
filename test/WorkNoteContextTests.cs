using System.Collections.Frozen;
using System.Threading.Tasks;
using DataEntities;
using DataEntities.Interfaces;
using Moq;
using ui.Data;

namespace test
{
    public class WorkNotesContextTests
    {
        private readonly Mock<IWorkNoteRepository> _mockWorkNoteRepository;

        public WorkNotesContextTests()
        {
            _mockWorkNoteRepository = new Mock<IWorkNoteRepository>();
        }


        [Fact]
        public async Task SelectEntities_WorkNotesFiltered_ReturnsSubsetOfTableData()
        {
            // Arrange
            _mockWorkNoteRepository.Setup(repo => repo.GetAllWorkNotes())
                          .ReturnsAsync(new List<WorkNote>([
                                new WorkNote(Description: "Work note 1"),
                                new WorkNote(Description: "Work note 2"),
                                new WorkNote(Description: "Work note 3"),
                            ]));

            // GetNotes(min, max) where min = 2, max = 2
            int noteId = 2;
            _mockWorkNoteRepository.Setup(repo => repo.GetNotes(noteId, noteId))
                          .ReturnsAsync(new List<WorkNote>([
                                new WorkNote(Description: "Work note 2"),
                            ]));

            // Act
            var allNotes = await _mockWorkNoteRepository.Object.GetAllWorkNotes();
            var subsetNotes = await _mockWorkNoteRepository.Object.GetNotes(noteId, noteId);
            var expectedNote = allNotes[1];

            // Assert
            Assert.Single(subsetNotes, actual =>
            {
                // Compare only the date/time components you care about
                var actualDate = actual.RecordedAtTimeUtc.Date;
                var expectedDate = expectedNote.RecordedAtTimeUtc.Date;
                var actualTime = actual.RecordedAtTimeUtc.TimeOfDay;
                var expectedTime = expectedNote.RecordedAtTimeUtc.TimeOfDay;

                return actualDate == expectedDate &&
                    Math.Abs(actualTime.TotalMinutes - expectedTime.TotalMinutes) < 0.1;
            });
        }
    }
}