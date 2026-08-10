using Studia.Application.Cohorts;
using Studia.Application.Courses;
using Studia.Application.Enrollments;
using Studia.Application.Sections;
using Studia.Application.Tests.Activities;
using Studia.Application.Tests.Cohorts;
using Studia.Application.Tests.Enrollments;
using Studia.Application.Tests.Sections;
using Studia.Application.Tests.Submissions;
using Studia.Domain.Activities;
using Studia.Domain.Cohorts;
using Studia.Domain.Courses;
using Studia.Domain.Enrollments;
using Studia.Domain.Sections;

namespace Studia.Application.Tests.Courses;

public class DeleteCourseUseCaseTests
{
    [Fact]
    public void Execute_DeletesCourseAndAllItsDependents()
    {
        var courses = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        courses.Save(course);

        var sections = new FakeSectionRepository();
        var section = Section.Create(course.Id, "Semana 1", "");
        sections.Save(section);

        var activities = new FakeActivityRepository();
        var activity = Activity.Create(section.Id, "Tarea", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        activities.Save(activity);

        var submissions = new FakeSubmissionRepository();

        var enrollments = new FakeEnrollmentRepository();
        var enrollment = Enrollment.Enroll(course.Id, Guid.NewGuid());
        enrollments.Save(enrollment);

        var cohorts = new FakeCohortRepository();
        var cohort = Cohort.Create(course.Id, "Ficha A");
        cohorts.Save(cohort);

        var deleteSectionUseCase = new DeleteSectionUseCase(sections, activities, submissions);
        var useCase = new DeleteCourseUseCase(courses, sections, deleteSectionUseCase, enrollments, cohorts);

        useCase.Execute(new DeleteCourseCommand(course.Id));

        Assert.Null(courses.GetById(course.Id));
        Assert.Empty(sections.GetByCourseId(course.Id));
        Assert.Empty(activities.GetBySectionId(section.Id));
        Assert.Empty(enrollments.GetByCourseId(course.Id));
        Assert.Empty(cohorts.GetByCourseId(course.Id));
    }

    [Fact]
    public void Execute_WhenCourseDoesNotExist_Throws()
    {
        var courses = new FakeCourseRepository();
        var sections = new FakeSectionRepository();
        var deleteSectionUseCase = new DeleteSectionUseCase(sections, new FakeActivityRepository(), new FakeSubmissionRepository());
        var useCase = new DeleteCourseUseCase(courses, sections, deleteSectionUseCase, new FakeEnrollmentRepository(), new FakeCohortRepository());

        Assert.Throws<InvalidOperationException>(() => useCase.Execute(new DeleteCourseCommand(Guid.NewGuid())));
    }
}
