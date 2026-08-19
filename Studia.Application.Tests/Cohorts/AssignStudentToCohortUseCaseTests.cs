using Studia.Application.Cohorts;
using Studia.Application.Tests.Courses;
using Studia.Application.Tests.Notifications;
using Studia.Application.Tests.Users;
using Studia.Domain.Cohorts;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Cohorts;

public class AssignStudentToCohortUseCaseTests
{
    private static User CreateStudent(string email = "estudiante@sena.edu.co") =>
        User.Register(Email.Create(email), "hashed-value", Role.Estudiante);

    private static AssignStudentToCohortUseCase CreateUseCase(FakeCohortRepository cohorts, FakeUserRepository users) =>
        new(cohorts, new FakeCourseRepository(), users, new FakeNotificationRepository(), new FakeEmailSender());

    [Fact]
    public void Execute_WithValidStudent_AssignsToCohort()
    {
        var cohort = Cohort.Create(Guid.NewGuid(), "Ficha 123456");
        var cohortRepository = new FakeCohortRepository();
        cohortRepository.Save(cohort);

        var student = CreateStudent();
        var userRepository = new FakeUserRepository();
        userRepository.Save(student);

        var useCase = CreateUseCase(cohortRepository, userRepository);

        var result = useCase.Execute(new AssignStudentToCohortCommand(cohort.Id, student.Id));

        Assert.Contains(student.Id, result.StudentIds);
    }

    [Fact]
    public void Execute_WhenCohortDoesNotExist_Throws()
    {
        var useCase = CreateUseCase(new FakeCohortRepository(), new FakeUserRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new AssignStudentToCohortCommand(Guid.NewGuid(), Guid.NewGuid())));
    }

    [Fact]
    public void Execute_WhenStudentDoesNotExist_Throws()
    {
        var cohort = Cohort.Create(Guid.NewGuid(), "Ficha 123456");
        var cohortRepository = new FakeCohortRepository();
        cohortRepository.Save(cohort);

        var useCase = CreateUseCase(cohortRepository, new FakeUserRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new AssignStudentToCohortCommand(cohort.Id, Guid.NewGuid())));
    }

    [Fact]
    public void Execute_WhenUserIsNotAStudent_Throws()
    {
        var cohort = Cohort.Create(Guid.NewGuid(), "Ficha 123456");
        var cohortRepository = new FakeCohortRepository();
        cohortRepository.Save(cohort);

        var teacher = User.Register(Email.Create("profe@sena.edu.co"), "hashed-value", Role.Profesor);
        var userRepository = new FakeUserRepository();
        userRepository.Save(teacher);

        var useCase = CreateUseCase(cohortRepository, userRepository);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new AssignStudentToCohortCommand(cohort.Id, teacher.Id)));
    }

    [Fact]
    public void Execute_WhenStudentAlreadyInAnotherCohortOfSameCourse_Throws()
    {
        var courseId = Guid.NewGuid();
        var firstCohort = Cohort.Create(courseId, "Ficha 111111");
        var secondCohort = Cohort.Create(courseId, "Ficha 222222");

        var student = CreateStudent();
        firstCohort.AssignStudent(student.Id);

        var cohortRepository = new FakeCohortRepository();
        cohortRepository.Save(firstCohort);
        cohortRepository.Save(secondCohort);

        var userRepository = new FakeUserRepository();
        userRepository.Save(student);

        var useCase = CreateUseCase(cohortRepository, userRepository);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new AssignStudentToCohortCommand(secondCohort.Id, student.Id)));
    }
}
