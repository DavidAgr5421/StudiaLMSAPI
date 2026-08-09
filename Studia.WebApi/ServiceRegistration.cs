using Studia.Application.Activities;
using Studia.Application.Auth;
using Studia.Application.Cohorts;
using Studia.Application.Courses;
using Studia.Application.Enrollments;
using Studia.Application.Notifications;
using Studia.Application.Sections;
using Studia.Application.Submissions;
using Studia.Application.Users;
using Studia.Infrastructure.Content;
using Studia.Infrastructure.Notifications;
using Studia.Infrastructure.Persistence;
using Studia.Infrastructure.Security;
using Studia.Infrastructure.Storage;

namespace Studia.WebApi;

public static class ServiceRegistration
{
    // Singleton: cada InMemory*Repository guarda su estado en un campo de instancia.
    // Si se registraran como Scoped/Transient, cada request vería un almacén vacío nuevo.
    public static IServiceCollection AddStudiaRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<ICourseRepository, InMemoryCourseRepository>();
        services.AddSingleton<ICohortRepository, InMemoryCohortRepository>();
        services.AddSingleton<IEnrollmentRepository, InMemoryEnrollmentRepository>();
        services.AddSingleton<ISectionRepository, InMemorySectionRepository>();
        services.AddSingleton<IActivityRepository, InMemoryActivityRepository>();
        services.AddSingleton<ISubmissionRepository, InMemorySubmissionRepository>();
        services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();
        services.AddSingleton<IRevokedTokenRepository, InMemoryRevokedTokenRepository>();

        return services;
    }

    public static IServiceCollection AddStudiaTechnicalPorts(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IHtmlSanitizer, AllowListHtmlSanitizer>();
        services.AddSingleton<IFileStorage, LocalFileStorage>();
        services.AddSingleton<IEmailSender, ConsoleEmailSender>();

        return services;
    }

    // Scoped: no guardan estado propio, se resuelven una vez por request.
    public static IServiceCollection AddStudiaUseCases(this IServiceCollection services)
    {
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        services.AddScoped<ILoginUseCase, LoginUseCase>();
        services.AddScoped<ILogoutUseCase, LogoutUseCase>();
        services.AddScoped<ISearchUsersUseCase, SearchUsersUseCase>();

        services.AddScoped<ICreateCourseUseCase, CreateCourseUseCase>();
        services.AddScoped<ISearchCoursesUseCase, SearchCoursesUseCase>();

        services.AddScoped<ICreateCohortUseCase, CreateCohortUseCase>();
        services.AddScoped<IAssignStudentToCohortUseCase, AssignStudentToCohortUseCase>();

        services.AddScoped<IEnrollStudentInOpenCourseUseCase, EnrollStudentInOpenCourseUseCase>();
        services.AddScoped<IRequestEnrollmentUseCase, RequestEnrollmentUseCase>();
        services.AddScoped<IApproveEnrollmentUseCase, ApproveEnrollmentUseCase>();
        services.AddScoped<IRejectEnrollmentUseCase, RejectEnrollmentUseCase>();
        services.AddScoped<IEnrollByInvitationUseCase, EnrollByInvitationUseCase>();

        services.AddScoped<ICreateSectionUseCase, CreateSectionUseCase>();
        services.AddScoped<ICreateActivityUseCase, CreateActivityUseCase>();

        services.AddScoped<ISubmitTextActivityUseCase, SubmitTextActivityUseCase>();
        services.AddScoped<ISubmitFilesActivityUseCase, SubmitFilesActivityUseCase>();
        services.AddScoped<IGradeSubmissionUseCase, GradeSubmissionUseCase>();

        services.AddScoped<INotifyNewActivityUseCase, NotifyNewActivityUseCase>();
        services.AddScoped<INotifyNewSectionUseCase, NotifyNewSectionUseCase>();
        services.AddScoped<ISendDueDateReminderUseCase, SendDueDateReminderUseCase>();
        services.AddScoped<IMarkNotificationAsReadUseCase, MarkNotificationAsReadUseCase>();

        return services;
    }
}
