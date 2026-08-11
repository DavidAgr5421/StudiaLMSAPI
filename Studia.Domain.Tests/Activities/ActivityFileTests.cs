using Studia.Domain.Activities;

namespace Studia.Domain.Tests.Activities;

public class ActivityFileTests
{
    [Fact]
    public void Create_WithValidData_SetsFields()
    {
        var file = ActivityFile.Create("guia.pdf", "storage-key-1", 1024);

        Assert.Equal("guia.pdf", file.FileName);
        Assert.Equal("storage-key-1", file.StorageKey);
        Assert.Equal(1024, file.SizeBytes);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankFileName_Throws(string blankName)
    {
        Assert.Throws<ArgumentException>(() => ActivityFile.Create(blankName, "key", 1024));
    }

    [Fact]
    public void Create_WithZeroSize_Throws()
    {
        Assert.Throws<ArgumentException>(() => ActivityFile.Create("a.pdf", "key", 0));
    }

    [Fact]
    public void Create_ExceedingMaxSize_Throws()
    {
        Assert.Throws<ArgumentException>(() => ActivityFile.Create("a.pdf", "key", ActivityFile.MaxSizeBytes + 1));
    }

    [Fact]
    public void Create_AtMaxSize_Succeeds()
    {
        var file = ActivityFile.Create("a.pdf", "key", ActivityFile.MaxSizeBytes);

        Assert.Equal(ActivityFile.MaxSizeBytes, file.SizeBytes);
    }
}
