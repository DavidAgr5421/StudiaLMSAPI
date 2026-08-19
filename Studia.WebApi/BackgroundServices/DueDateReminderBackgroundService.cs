using Studia.Application.Activities;
using Studia.Application.Cohorts;
using Studia.Application.Enrollments;
using Studia.Application.Notifications;
using Studia.Application.Sections;
using Studia.Application.Submissions;
using Studia.Application.Users;
using Studia.Domain.Activities;
using Studia.Domain.Enrollments;
using Studia.Domain.Notifications;
using Studia.Domain.Sections;

namespace Studia.WebApi.BackgroundServices;

// Recorre periódicamente las actividades próximas a vencer y manda un recordatorio a los
// 24h, 8h, 4h y 1h antes de la fecha límite a quienes todavía no entregaron. Cada tier se
// manda una sola vez por estudiante -- se deduplica revisando si ya existe una notificación
// con ese mismo título para esa actividad y ese estudiante.
public class DueDateReminderBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<DueDateReminderBackgroundService> logger) : BackgroundService
{
    private static readonly (int Hours, string Label)[] Tiers =
    [
        (24, "24 horas"),
        (8, "8 horas"),
        (4, "4 horas"),
        (1, "1 hora")
    ];

    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(CheckInterval);

        do
        {
            try
            {
                CheckDueDates();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error revisando recordatorios de fecha límite.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private void CheckDueDates()
    {
        using var scope = scopeFactory.CreateScope();
        var services = scope.ServiceProvider;

        var activityRepository = services.GetRequiredService<IActivityRepository>();
        var sectionRepository = services.GetRequiredService<ISectionRepository>();
        var enrollmentRepository = services.GetRequiredService<IEnrollmentRepository>();
        var submissionRepository = services.GetRequiredService<ISubmissionRepository>();
        var cohortRepository = services.GetRequiredService<ICohortRepository>();
        var userRepository = services.GetRequiredService<IUserRepository>();
        var notificationRepository = services.GetRequiredService<INotificationRepository>();
        var emailSender = services.GetRequiredService<IEmailSender>();

        var now = DateTime.UtcNow;
        var maxTierHours = Tiers.Max(t => t.Hours);
        var activities = activityRepository.GetWithDueDateBetween(now, now.AddHours(maxTierHours));

        foreach (var activity in activities)
        {
            if (activity.Status == ActivityStatus.Oculto)
                continue;

            var section = sectionRepository.GetById(activity.SectionId);
            if (section is null || section.Status == SectionStatus.Oculto)
                continue;

            var hoursUntilDue = (activity.DueDateUtc - now).TotalHours;
            var pendingStudentIds = GetPendingStudentIds(activity, section, enrollmentRepository, submissionRepository, cohortRepository);

            foreach (var (tierHours, label) in Tiers)
            {
                if (hoursUntilDue > tierHours)
                    continue;

                var title = $"Fecha límite próxima: quedan {label}";

                foreach (var studentId in pendingStudentIds)
                {
                    var alreadySent = notificationRepository.GetByRecipientUserId(studentId)
                        .Any(n => n.Type == NotificationType.RecordatorioFechaLimite && n.RelatedEntityId == activity.Id && n.Title == title);

                    if (alreadySent)
                        continue;

                    var student = userRepository.GetById(studentId);
                    if (student is null)
                        continue;

                    var notification = Notification.Create(
                        student.Id,
                        NotificationType.RecordatorioFechaLimite,
                        title,
                        $"Quedan {label} para la fecha límite de '{activity.Title}' ({activity.DueDateUtc:yyyy-MM-dd HH:mm} UTC) y aún no la has entregado.",
                        activity.Id);

                    emailSender.Send(student.Email.Value, notification.Title, notification.Message);
                    notification.MarkEmailSent();

                    notificationRepository.Save(notification);
                }
            }
        }
    }

    private static IReadOnlyCollection<Guid> GetPendingStudentIds(
        Activity activity,
        Section section,
        IEnrollmentRepository enrollmentRepository,
        ISubmissionRepository submissionRepository,
        ICohortRepository cohortRepository)
    {
        var studentsWhoSubmitted = submissionRepository.GetByActivityId(activity.Id)
            .Select(s => s.StudentId)
            .ToHashSet();

        var approvedStudentIds = enrollmentRepository.GetByCourseId(section.CourseId)
            .Where(e => e.Status == EnrollmentStatus.Aprobada)
            .Select(e => e.StudentId)
            .Where(studentId => !studentsWhoSubmitted.Contains(studentId));

        if (activity.CohortIds.Count == 0)
            return approvedStudentIds.ToList();

        var myCohortsByStudent = cohortRepository.GetByCourseId(section.CourseId)
            .Where(c => activity.CohortIds.Contains(c.Id))
            .SelectMany(c => c.StudentIds)
            .ToHashSet();

        return approvedStudentIds.Where(myCohortsByStudent.Contains).ToList();
    }
}
