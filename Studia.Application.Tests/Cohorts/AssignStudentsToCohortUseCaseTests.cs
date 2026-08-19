using Studia.Application.Cohorts;
using Studia.Application.Tests.Courses;
using Studia.Application.Tests.Notifications;
using Studia.Application.Tests.Users;
using Studia.Domain.Cohorts;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Cohorts;

public class AssignStudentsToCohortUseCaseTests
{
    private static User CreateStudent(string email) =>
        User.Register(Email.Create(email), "hashed-value", Role.Estudiante);

    private static AssignStudentsToCohortUseCase CreateUseCase(FakeCohortRepository cohorts, FakeUserRepository users) =>
        new(cohorts, new FakeCourseRepository(), users, new FakeNotificationRepository(), new FakeEmailSender());

    [Fact]
    public void Execute_WithMixOfEmailAndId_AssignsBoth()
    {
        var cohort = Cohort.Create(Guid.NewGuid(), "Ficha 123456");
        var cohortRepository = new FakeCohortRepository();
        cohortRepository.Save(cohort);

        var byEmail = CreateStudent("ana@sena.edu.co");
        var byId = CreateStudent("luis@sena.edu.co");
        var userRepository = new FakeUserRepository();
        userRepository.Save(byEmail);
        userRepository.Save(byId);

        var useCase = CreateUseCase(cohortRepository, userRepository);

        var result = useCase.Execute(new AssignStudentsToCohortCommand(cohort.Id, [byEmail.Email.Value, byId.Id.ToString()]));

        Assert.All(result.Outcomes, o => Assert.True(o.Success));
        Assert.Contains(byEmail.Id, result.Cohort.StudentIds);
        Assert.Contains(byId.Id, result.Cohort.StudentIds);
    }

    [Fact]
    public void Execute_WithUnknownIdentifier_ReportsFailureWithoutStoppingOthers()
    {
        var cohort = Cohort.Create(Guid.NewGuid(), "Ficha 123456");
        var cohortRepository = new FakeCohortRepository();
        cohortRepository.Save(cohort);

        var student = CreateStudent("ana@sena.edu.co");
        var userRepository = new FakeUserRepository();
        userRepository.Save(student);

        var useCase = CreateUseCase(cohortRepository, userRepository);

        var result = useCase.Execute(new AssignStudentsToCohortCommand(cohort.Id, [student.Email.Value, "no-existe@sena.edu.co"]));

        Assert.Equal(2, result.Outcomes.Count);
        Assert.True(result.Outcomes.Single(o => o.Identifier == student.Email.Value).Success);
        Assert.False(result.Outcomes.Single(o => o.Identifier == "no-existe@sena.edu.co").Success);
        Assert.Single(result.Cohort.StudentIds);
    }

    [Fact]
    public void Execute_WithNonStudentRole_ReportsFailure()
    {
        var cohort = Cohort.Create(Guid.NewGuid(), "Ficha 123456");
        var cohortRepository = new FakeCohortRepository();
        cohortRepository.Save(cohort);

        var teacher = User.Register(Email.Create("profe@sena.edu.co"), "hash", Role.Profesor, "Profe");
        var userRepository = new FakeUserRepository();
        userRepository.Save(teacher);

        var useCase = CreateUseCase(cohortRepository, userRepository);

        var result = useCase.Execute(new AssignStudentsToCohortCommand(cohort.Id, [teacher.Email.Value]));

        Assert.False(Assert.Single(result.Outcomes).Success);
    }

    [Fact]
    public void Execute_WhenStudentAlreadyInAnotherCohortOfSameCourse_ReportsFailure()
    {
        var courseId = Guid.NewGuid();
        var firstCohort = Cohort.Create(courseId, "Ficha 111111");
        var secondCohort = Cohort.Create(courseId, "Ficha 222222");

        var student = CreateStudent("ana@sena.edu.co");
        firstCohort.AssignStudent(student.Id);

        var cohortRepository = new FakeCohortRepository();
        cohortRepository.Save(firstCohort);
        cohortRepository.Save(secondCohort);

        var userRepository = new FakeUserRepository();
        userRepository.Save(student);

        var useCase = CreateUseCase(cohortRepository, userRepository);

        var result = useCase.Execute(new AssignStudentsToCohortCommand(secondCohort.Id, [student.Email.Value]));

        Assert.False(Assert.Single(result.Outcomes).Success);
    }

    [Fact]
    public void Execute_WhenCohortDoesNotExist_Throws()
    {
        var useCase = CreateUseCase(new FakeCohortRepository(), new FakeUserRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new AssignStudentsToCohortCommand(Guid.NewGuid(), ["a@b.com"])));
    }
}
