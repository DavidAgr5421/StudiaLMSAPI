using Studia.Domain.Submissions;

namespace Studia.Domain.Tests.Submissions;

public class SubmittedFileTests
{
    [Fact]
    public void Create_WithValidData_SetsFields()
    {
        var file = SubmittedFile.Create("tarea.pdf", "storage-key-1", 1024);

        Assert.Equal("tarea.pdf", file.FileName);
        Assert.Equal("storage-key-1", file.StorageKey);
        Assert.Equal(1024, file.SizeBytes);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankFileName_Throws(string blankName)
    {
        Assert.Throws<ArgumentException>(() => SubmittedFile.Create(blankName, "key", 1024));
    }

    [Fact]
    public void Create_WithZeroSize_Throws()
    {
        Assert.Throws<ArgumentException>(() => SubmittedFile.Create("a.pdf", "key", 0));
    }

    [Fact]
    public void Create_ExceedingMaxSize_Throws()
    {
        Assert.Throws<ArgumentException>(() => SubmittedFile.Create("a.pdf", "key", SubmittedFile.MaxSizeBytes + 1));
    }

    [Fact]
    public void Create_AtMaxSize_Succeeds()
    {
        var file = SubmittedFile.Create("a.pdf", "key", SubmittedFile.MaxSizeBytes);

        Assert.Equal(SubmittedFile.MaxSizeBytes, file.SizeBytes);
    }
}
