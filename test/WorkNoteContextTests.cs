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

        [Fact]
        public async Task CreateWorkNote_DescriptionIsSet()
        {
            var expectedNote = new WorkNote(Description: "Test note 1");
            _mockWorkNoteRepository.Setup(repo => repo.GetAllWorkNotes())
                                .ReturnsAsync(new List<WorkNote> { expectedNote });

            var workNotes = await _mockWorkNoteRepository.Object.GetAllWorkNotes();

            Assert.IsType<List<WorkNote>>(workNotes);
            Assert.Single(workNotes);
            Assert.Equal("Test note 1", workNotes[0].Description);
        }

        [Fact]
        public async Task WorkNote_UpdateDescription_EntityStateUpdated()
        {
            var expectedNote = new WorkNote(Description: "Test note");
            _mockWorkNoteRepository.Setup(repo => repo.GetAllWorkNotes())
                                .ReturnsAsync(new List<WorkNote> { expectedNote });

            var workNotes = await _mockWorkNoteRepository.Object.GetAllWorkNotes();

            workNotes[0].Description = "Test: note 2 updated description";

            Assert.IsType<List<WorkNote>>(workNotes);
            Assert.Single(workNotes);
            Assert.Equal("Test: note 2 updated description", workNotes[0].Description);
        }

        [Fact]
        public async Task SetWorkNoteId_ShouldSetCorrectId()
        {
            // Arrange
            int expectedId = 1;
            _mockWorkNoteRepository.Setup(repo => repo.GetAllWorkNotes())
                                .ReturnsAsync(new List<WorkNote>([
                                        new WorkNote(Description: "Test note", Id: expectedId),
                                    ]));

            // GetNotes(min, max) where min = 1, max = 1
            _mockWorkNoteRepository.Setup(repo => repo.GetNotes(expectedId, expectedId))
                                .ReturnsAsync(new List<WorkNote>([
                                        new WorkNote(Description: "Test note", Id: expectedId),
                                    ]));

            // Act
            var allNotes = await _mockWorkNoteRepository.Object.GetAllWorkNotes();
            var subsetNotes = await _mockWorkNoteRepository.Object.GetNotes(expectedId, expectedId);

            // Assert
            Assert.Single(subsetNotes);
            Assert.Equal(expectedId, subsetNotes[0].Id);

            // Optional: Verify description and date/time if needed
            Assert.Equal("Test note", subsetNotes[0].Description);
            Assert.True(subsetNotes[0].RecordedAtTimeUtc - DateTime.UtcNow < TimeSpan.FromMinutes(1));
        }
    }
}