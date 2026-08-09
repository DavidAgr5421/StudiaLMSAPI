using Studia.Domain.Cohorts;

namespace Studia.Domain.Tests.Cohorts;

public class CohortTests
{
    [Fact]
    public void Create_WithValidData_StartsWithoutStudents()
    {
        var courseId = Guid.NewGuid();

        var cohort = Cohort.Create(courseId, "Ficha 123456");

        Assert.Equal(courseId, cohort.CourseId);
        Assert.Equal("Ficha 123456", cohort.Name);
        Assert.Empty(cohort.StudentIds);
    }

    [Fact]
    public void Create_WithEmptyCourseId_Throws()
    {
        Assert.Throws<ArgumentException>(() => Cohort.Create(Guid.Empty, "Ficha 123456"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankName_Throws(string blankName)
    {
        Assert.Throws<ArgumentException>(() => Cohort.Create(Guid.NewGuid(), blankName));
    }

    [Fact]
    public void AssignStudent_AddsStudentToCohort()
    {
        var cohort = Cohort.Create(Guid.NewGuid(), "Ficha 123456");
        var studentId = Guid.NewGuid();

        cohort.AssignStudent(studentId);

        Assert.Contains(studentId, cohort.StudentIds);
    }

    [Fact]
    public void AssignStudent_WhenAlreadyAssigned_Throws()
    {
        var cohort = Cohort.Create(Guid.NewGuid(), "Ficha 123456");
        var studentId = Guid.NewGuid();
        cohort.AssignStudent(studentId);

        Assert.Throws<InvalidOperationException>(() => cohort.AssignStudent(studentId));
    }
}
