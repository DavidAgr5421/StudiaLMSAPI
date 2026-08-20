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
using Studia.Infrastructure.Persistence.EfCore.Repositories;
using Studia.Infrastructure.Security;
using Studia.Infrastructure.Storage;

namespace Studia.WebApi;

public static class ServiceRegistration
{
    // Scoped, no Singleton: cada Ef*Repository envuelve el mismo StudiaDbContext, que
    // AddDbContext registra como Scoped (una instancia por request). EF Core no es seguro
    // para usar desde múltiples threads/requests a la vez, así que el repositorio no puede
    // vivir más tiempo que el propio DbContext que envuelve.
    public static IServiceCollection AddStudiaRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<ICourseRepository, EfCourseRepository>();
        services.AddScoped<ICohortRepository, EfCohortRepository>();
        services.AddScoped<IEnrollmentRepository, EfEnrollmentRepository>();
        services.AddScoped<ISectionRepository, EfSectionRepository>();
        services.AddScoped<IActivityRepository, EfActivityRepository>();
        services.AddScoped<ISubmissionRepository, EfSubmissionRepository>();
        services.AddScoped<INotificationRepository, EfNotificationRepository>();
        services.AddScoped<IRevokedTokenRepository, EfRevokedTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, EfPasswordResetTokenRepository>();

        return services;
    }

    public static IServiceCollection AddStudiaTechnicalPorts(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IHtmlSanitizer, AllowListHtmlSanitizer>();
        services.AddScoped<IFileStorage, DatabaseFileStorage>();
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
        services.AddScoped<IGetUserByIdUseCase, GetUserByIdUseCase>();
        services.AddScoped<IUpdateNameUseCase, UpdateNameUseCase>();
        services.AddScoped<IChangeEmailUseCase, ChangeEmailUseCase>();
        services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
        services.AddScoped<IRequestPasswordResetUseCase, RequestPasswordResetUseCase>();
        services.AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>();
        services.AddScoped<ISetIdentificationUseCase, SetIdentificationUseCase>();

        services.AddScoped<ICreateCourseUseCase, CreateCourseUseCase>();
        services.AddScoped<ISearchCoursesUseCase, SearchCoursesUseCase>();
        services.AddScoped<IGetCourseByIdUseCase, GetCourseByIdUseCase>();
        services.AddScoped<IGetCoursesByProfesorUseCase, GetCoursesByProfesorUseCase>();
        services.AddScoped<IDeleteCourseUseCase, DeleteCourseUseCase>();
        services.AddScoped<IUpdateCourseColorUseCase, UpdateCourseColorUseCase>();
        services.AddScoped<ISetCourseCoverImageUseCase, SetCourseCoverImageUseCase>();
        services.AddScoped<IRemoveCourseCoverImageUseCase, RemoveCourseCoverImageUseCase>();
        services.AddScoped<IGetCourseCoverImageUseCase, GetCourseCoverImageUseCase>();

        services.AddScoped<ICreateCohortUseCase, CreateCohortUseCase>();
        services.AddScoped<IAssignStudentToCohortUseCase, AssignStudentToCohortUseCase>();
        services.AddScoped<IAssignStudentsToCohortUseCase, AssignStudentsToCohortUseCase>();
        services.AddScoped<IGetCohortsByCourseUseCase, GetCohortsByCourseUseCase>();

        services.AddScoped<IEnrollStudentInOpenCourseUseCase, EnrollStudentInOpenCourseUseCase>();
        services.AddScoped<IRequestEnrollmentUseCase, RequestEnrollmentUseCase>();
        services.AddScoped<IApproveEnrollmentUseCase, ApproveEnrollmentUseCase>();
        services.AddScoped<IRejectEnrollmentUseCase, RejectEnrollmentUseCase>();
        services.AddScoped<IEnrollByInvitationUseCase, EnrollByInvitationUseCase>();
        services.AddScoped<IAddStudentsToCourseUseCase, AddStudentsToCourseUseCase>();
        services.AddScoped<IGetEnrollmentsByCourseUseCase, GetEnrollmentsByCourseUseCase>();
        services.AddScoped<IGetEnrollmentsByStudentUseCase, GetEnrollmentsByStudentUseCase>();

        services.AddScoped<ICreateSectionUseCase, CreateSectionUseCase>();
        services.AddScoped<IGetSectionsByCourseUseCase, GetSectionsByCourseUseCase>();
        services.AddScoped<IDeleteSectionUseCase, DeleteSectionUseCase>();
        services.AddScoped<ICreateActivityUseCase, CreateActivityUseCase>();
        services.AddScoped<IGetActivitiesBySectionUseCase, GetActivitiesBySectionUseCase>();
        services.AddScoped<IGetActivityByIdUseCase, GetActivityByIdUseCase>();
        services.AddScoped<IGetActivityFileUseCase, GetActivityFileUseCase>();
        services.AddScoped<ICloseActivityUseCase, CloseActivityUseCase>();
        services.AddScoped<IReopenActivityUseCase, ReopenActivityUseCase>();

        services.AddScoped<ISubmitTextActivityUseCase, SubmitTextActivityUseCase>();
        services.AddScoped<ISubmitFilesActivityUseCase, SubmitFilesActivityUseCase>();
        services.AddScoped<IGradeSubmissionUseCase, GradeSubmissionUseCase>();
        services.AddScoped<IGetSubmissionsByActivityUseCase, GetSubmissionsByActivityUseCase>();
        services.AddScoped<IGetSubmissionForActivityUseCase, GetSubmissionForActivityUseCase>();
        services.AddScoped<IGetSubmissionFileUseCase, GetSubmissionFileUseCase>();
        services.AddScoped<IEditTextSubmissionUseCase, EditTextSubmissionUseCase>();
        services.AddScoped<IEditFilesSubmissionUseCase, EditFilesSubmissionUseCase>();

        services.AddScoped<INotifyNewActivityUseCase, NotifyNewActivityUseCase>();
        services.AddScoped<INotifyNewSectionUseCase, NotifyNewSectionUseCase>();
        services.AddScoped<ISendDueDateReminderUseCase, SendDueDateReminderUseCase>();
        services.AddScoped<IMarkNotificationAsReadUseCase, MarkNotificationAsReadUseCase>();
        services.AddScoped<IGetMyNotificationsUseCase, GetMyNotificationsUseCase>();

        return services;
    }
}
