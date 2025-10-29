using DataEntities;
using Microsoft.EntityFrameworkCore;
using Moq;
using ui.Data;

namespace test;

public class WorkNoteRepositoryTests
{
    [Fact]
    public async Task GetAllWorkNotes_Should_Return_All_WorkNotes()
    {
        // Arrange
        var workNotes = GetTestWorkNotes();
        var mockSet = new Mock<DbSet<WorkNote>>();
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.Provider).Returns(workNotes.Provider);
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.Expression).Returns(workNotes.Expression);
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.ElementType).Returns(workNotes.ElementType);
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.GetEnumerator()).Returns(workNotes.GetEnumerator());

        var mockContext = new Mock<EddyWorkNotesContext>();
        mockContext.Setup(c => c.WorkNote).Returns(mockSet.Object);

        var repository = new WorkNoteRepository(mockContext.Object);

        // Act
        var result = await repository.GetAllWorkNotes();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(workNotes.Count, result.Count);
        Assert.Equal(workNotes[0].Description, result[0].Description);
        Assert.Equal(workNotes[1].Description, result[1].Description);
    }

    [Fact]
    public async Task GetNotes_Should_Return_WorkNotes_In_Range()
    {
        // Arrange
        var workNotes = GetTestWorkNotes();
        var mockSet = new Mock<DbSet<WorkNote>>();
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.Provider).Returns(workNotes.Provider);
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.Expression).Returns(workNotes.Expression);
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.ElementType).Returns(workNotes.ElementType);
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.GetEnumerator()).Returns(workNotes.GetEnumerator());

        var mockContext = new Mock<EddyWorkNotesContext>();
        mockContext.Setup(c => c.WorkNote).Returns(mockSet.Object);

        var repository = new WorkNoteRepository(mockContext.Object);

        // Act
        var result = await repository.GetNotes(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(workNotes[0].Description, result[0].Description);
        Assert.Equal(workNotes[1].Description, result[1].Description);
    }

    [Fact]
    public async Task GetNotes_Should_Return_Empty_List_When_No_WorkNotes_In_Range()
    {
        // Arrange
        var workNotes = GetTestWorkNotes();
        var mockSet = new Mock<DbSet<WorkNote>>();
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.Provider).Returns(workNotes.Provider);
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.Expression).Returns(workNotes.Expression);
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.ElementType).Returns(workNotes.ElementType);
        mockSet.As<IQueryable<WorkNote>>().Setup(m => m.GetEnumerator()).Returns(workNotes.GetEnumerator());

        var mockContext = new Mock<EddyWorkNotesContext>();
        mockContext.Setup(c => c.WorkNote).Returns(mockSet.Object);

        var repository = new WorkNoteRepository(mockContext.Object);

        // Act
        var result = await repository.GetNotes(10, 20);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    private List<WorkNote> GetTestWorkNotes()
    {
        return new List<WorkNote>
        {
            new WorkNote("First work note", 1),
            new WorkNote("Second work note", 2),
            new WorkNote("Third work note", 3)
        };
    }
}
