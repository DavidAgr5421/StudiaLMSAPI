using Studia.Application.Activities;
using Studia.Application.Tests.Courses;
using Studia.Application.Tests.Sections;
using Studia.Domain.Activities;
using Studia.Domain.Courses;
using Studia.Domain.Sections;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Activities;

public class GetActivityByIdUseCaseTests
{
    [Fact]
    public void Execute_WhenActivityDoesNotExist_ReturnsNull()
    {
        var useCase = new GetActivityByIdUseCase(new FakeActivityRepository(), new FakeSectionRepository(), new FakeCourseRepository());

        var result = useCase.Execute(new GetActivityByIdQuery(Guid.NewGuid(), Guid.NewGuid(), Role.Estudiante));

        Assert.Null(result);
    }

    [Fact]
    public void Execute_HiddenActivity_ReturnsNullForStudent_ButResultForOwner()
    {
        var profesorId = Guid.NewGuid();
        var courses = new FakeCourseRepository();
        var course = Course.Create("Curso", EnrollmentMode.Abierta, profesorId);
        courses.Save(course);

        var sections = new FakeSectionRepository();
        var section = Section.Create(course.Id, "Semana 1", "");
        sections.Save(section);

        var activities = new FakeActivityRepository();
        var hidden = Activity.Create(
            section.Id, "Borrador", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null, status: ActivityStatus.Oculto);
        activities.Save(hidden);

        var useCase = new GetActivityByIdUseCase(activities, sections, courses);

        var asStudent = useCase.Execute(new GetActivityByIdQuery(hidden.Id, Guid.NewGuid(), Role.Estudiante));
        var asOwner = useCase.Execute(new GetActivityByIdQuery(hidden.Id, profesorId, Role.Profesor));
        var asAdmin = useCase.Execute(new GetActivityByIdQuery(hidden.Id, Guid.NewGuid(), Role.Administrador));

        Assert.Null(asStudent);
        Assert.NotNull(asOwner);
        Assert.NotNull(asAdmin);
    }
}
