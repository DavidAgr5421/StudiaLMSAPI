using System.Security.Cryptography;
using Studia.Application.Activities;
using Studia.Application.Auth;
using Studia.Application.Cohorts;
using Studia.Application.Courses;
using Studia.Application.Enrollments;
using Studia.Application.Notifications;
using Studia.Application.Sections;
using Studia.Application.Submissions;
using Studia.Application.Users;
using Studia.Domain.Activities;
using Studia.Domain.Courses;
using Studia.Domain.Users;
using Studia.Infrastructure.Auth;
using Studia.Infrastructure.Content;
using Studia.Infrastructure.Notifications;
using Studia.Infrastructure.Persistence;
using Studia.Infrastructure.Security;
using Studia.Infrastructure.Storage;

ICourseRepository courseRepository = new InMemoryCourseRepository();
ICreateCourseUseCase createCourseUseCase = new CreateCourseUseCase(courseRepository);

IUserRepository userRepository = new InMemoryUserRepository();
IPasswordHasher passwordHasher = new Pbkdf2PasswordHasher();
IRegisterUserUseCase registerUserUseCase = new RegisterUserUseCase(userRepository, passwordHasher);

ICohortRepository cohortRepository = new InMemoryCohortRepository();
ICreateCohortUseCase createCohortUseCase = new CreateCohortUseCase(cohortRepository, courseRepository);
IAssignStudentToCohortUseCase assignStudentToCohortUseCase = new AssignStudentToCohortUseCase(cohortRepository, userRepository);

IEnrollmentRepository enrollmentRepository = new InMemoryEnrollmentRepository();
IEnrollStudentInOpenCourseUseCase enrollStudentInOpenCourseUseCase =
    new EnrollStudentInOpenCourseUseCase(enrollmentRepository, courseRepository, userRepository);
IRequestEnrollmentUseCase requestEnrollmentUseCase =
    new RequestEnrollmentUseCase(enrollmentRepository, courseRepository, userRepository);
IApproveEnrollmentUseCase approveEnrollmentUseCase = new ApproveEnrollmentUseCase(enrollmentRepository);
IRejectEnrollmentUseCase rejectEnrollmentUseCase = new RejectEnrollmentUseCase(enrollmentRepository);
IEnrollByInvitationUseCase enrollByInvitationUseCase =
    new EnrollByInvitationUseCase(enrollmentRepository, courseRepository, userRepository);

ISectionRepository sectionRepository = new InMemorySectionRepository();
IHtmlSanitizer htmlSanitizer = new AllowListHtmlSanitizer();
ICreateSectionUseCase createSectionUseCase = new CreateSectionUseCase(sectionRepository, courseRepository, htmlSanitizer);

IActivityRepository activityRepository = new InMemoryActivityRepository();
ICreateActivityUseCase createActivityUseCase = new CreateActivityUseCase(activityRepository, sectionRepository);

ISubmissionRepository submissionRepository = new InMemorySubmissionRepository();
IFileStorage fileStorage = new LocalFileStorage();
ISubmitTextActivityUseCase submitTextActivityUseCase =
    new SubmitTextActivityUseCase(submissionRepository, activityRepository, userRepository);
ISubmitFilesActivityUseCase submitFilesActivityUseCase =
    new SubmitFilesActivityUseCase(submissionRepository, activityRepository, userRepository, fileStorage);
IGradeSubmissionUseCase gradeSubmissionUseCase = new GradeSubmissionUseCase(submissionRepository);

var jwtSigningSecret = Environment.GetEnvironmentVariable("STUDIA_JWT_SECRET");
if (string.IsNullOrWhiteSpace(jwtSigningSecret))
{
    jwtSigningSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    Console.WriteLine("[dev] STUDIA_JWT_SECRET no configurado; usando una clave aleatoria válida solo para esta ejecución.");
    Console.WriteLine("[dev] En producción esta clave debe venir de configuración/secrets manager, nunca generarse al vuelo.");
}

IJwtTokenService jwtTokenService = new JwtTokenService(jwtSigningSecret);
IRevokedTokenRepository revokedTokenRepository = new InMemoryRevokedTokenRepository();
ILoginUseCase loginUseCase = new LoginUseCase(userRepository, passwordHasher, jwtTokenService);
ILogoutUseCase logoutUseCase = new LogoutUseCase(jwtTokenService, revokedTokenRepository);
IValidateTokenUseCase validateTokenUseCase = new ValidateTokenUseCase(jwtTokenService, revokedTokenRepository);

INotificationRepository notificationRepository = new InMemoryNotificationRepository();
IEmailSender emailSender = new ConsoleEmailSender();
INotifyNewActivityUseCase notifyNewActivityUseCase =
    new NotifyNewActivityUseCase(notificationRepository, activityRepository, sectionRepository, enrollmentRepository, userRepository, emailSender);
INotifyNewSectionUseCase notifyNewSectionUseCase =
    new NotifyNewSectionUseCase(notificationRepository, sectionRepository, enrollmentRepository, userRepository, emailSender);
ISendDueDateReminderUseCase sendDueDateReminderUseCase =
    new SendDueDateReminderUseCase(notificationRepository, activityRepository, sectionRepository, enrollmentRepository, submissionRepository, userRepository, emailSender);
IMarkNotificationAsReadUseCase markNotificationAsReadUseCase = new MarkNotificationAsReadUseCase(notificationRepository);

Console.WriteLine("=== LMS SENA ===");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. Create course");
    Console.WriteLine("2. Register user");
    Console.WriteLine("3. Create cohort (ficha)");
    Console.WriteLine("4. Assign student to cohort");
    Console.WriteLine("5. Enroll student in open course");
    Console.WriteLine("6. Request enrollment (needs approval)");
    Console.WriteLine("7. Approve enrollment");
    Console.WriteLine("8. Reject enrollment");
    Console.WriteLine("9. Enroll student by invitation code");
    Console.WriteLine("10. Create section");
    Console.WriteLine("11. Create activity");
    Console.WriteLine("12. Submit text activity");
    Console.WriteLine("13. Submit file activity");
    Console.WriteLine("14. Grade submission");
    Console.WriteLine("15. Login");
    Console.WriteLine("16. Logout");
    Console.WriteLine("17. Validate token");
    Console.WriteLine("18. Notify new activity");
    Console.WriteLine("19. Notify new section");
    Console.WriteLine("20. Send due date reminder");
    Console.WriteLine("21. Mark notification as read");
    Console.WriteLine("0. Exit");
    Console.Write("Choose an option: ");

    var option = Console.ReadLine();

    if (option == "0")
        break;

    switch (option)
    {
        case "1":
            RunCreateCourse();
            break;
        case "2":
            RunRegisterUser();
            break;
        case "3":
            RunCreateCohort();
            break;
        case "4":
            RunAssignStudentToCohort();
            break;
        case "5":
            RunEnrollStudentInOpenCourse();
            break;
        case "6":
            RunRequestEnrollment();
            break;
        case "7":
            RunApproveEnrollment();
            break;
        case "8":
            RunRejectEnrollment();
            break;
        case "9":
            RunEnrollByInvitation();
            break;
        case "10":
            RunCreateSection();
            break;
        case "11":
            RunCreateActivity();
            break;
        case "12":
            RunSubmitTextActivity();
            break;
        case "13":
            RunSubmitFilesActivity();
            break;
        case "14":
            RunGradeSubmission();
            break;
        case "15":
            RunLogin();
            break;
        case "16":
            RunLogout();
            break;
        case "17":
            RunValidateToken();
            break;
        case "18":
            RunNotifyNewActivity();
            break;
        case "19":
            RunNotifyNewSection();
            break;
        case "20":
            RunSendDueDateReminder();
            break;
        case "21":
            RunMarkNotificationAsRead();
            break;
        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}

void RunCreateCourse()
{
    Console.Write("Course name: ");
    var name = Console.ReadLine() ?? string.Empty;

    Console.Write($"Enrollment mode ({string.Join(" / ", Enum.GetNames<EnrollmentMode>())}): ");
    var enrollmentModeText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Enum.TryParse<EnrollmentMode>(enrollmentModeText, ignoreCase: true, out var enrollmentMode))
            throw new ArgumentException($"Invalid enrollment mode: '{enrollmentModeText}'.");

        var result = createCourseUseCase.Execute(new CreateCourseCommand(name, enrollmentMode));

        Console.WriteLine();
        Console.WriteLine("Course created successfully:");
        Console.WriteLine($"  Id:     {result.Id}");
        Console.WriteLine($"  Name:   {result.Name}");
        Console.WriteLine($"  Mode:   {result.EnrollmentMode}");
        Console.WriteLine($"  Status: {result.Status}");
        if (result.InvitationCode is not null)
            Console.WriteLine($"  Invitation code: {result.InvitationCode}");
    });
}

void RunRegisterUser()
{
    Console.Write("Email: ");
    var email = Console.ReadLine() ?? string.Empty;

    Console.Write("Password: ");
    var password = Console.ReadLine() ?? string.Empty;

    Console.Write($"Role ({string.Join(" / ", Enum.GetNames<Role>())}): ");
    var roleText = Console.ReadLine() ?? string.Empty;

    Console.Write("Name (optional): ");
    var nameInput = Console.ReadLine();
    var name = string.IsNullOrWhiteSpace(nameInput) ? null : nameInput;

    RunSafely(() =>
    {
        if (!Enum.TryParse<Role>(roleText, ignoreCase: true, out var role))
            throw new ArgumentException($"Invalid role: '{roleText}'.");

        var result = registerUserUseCase.Execute(new RegisterUserCommand(email, password, role, name));

        Console.WriteLine();
        Console.WriteLine("User registered successfully:");
        Console.WriteLine($"  Id:    {result.Id}");
        Console.WriteLine($"  Email: {result.Email}");
        Console.WriteLine($"  Name:  {result.Name ?? "(not provided)"}");
        Console.WriteLine($"  Role:  {result.Role}");
    });
}

void RunCreateCohort()
{
    Console.Write("Course id: ");
    var courseIdText = Console.ReadLine() ?? string.Empty;

    Console.Write("Cohort name: ");
    var name = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(courseIdText, out var courseId))
            throw new ArgumentException($"Invalid course id: '{courseIdText}'.");

        var result = createCohortUseCase.Execute(new CreateCohortCommand(courseId, name));

        Console.WriteLine();
        Console.WriteLine("Cohort created successfully:");
        Console.WriteLine($"  Id:       {result.Id}");
        Console.WriteLine($"  CourseId: {result.CourseId}");
        Console.WriteLine($"  Name:     {result.Name}");
    });
}

void RunAssignStudentToCohort()
{
    Console.Write("Cohort id: ");
    var cohortIdText = Console.ReadLine() ?? string.Empty;

    Console.Write("Student id: ");
    var studentIdText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(cohortIdText, out var cohortId))
            throw new ArgumentException($"Invalid cohort id: '{cohortIdText}'.");

        if (!Guid.TryParse(studentIdText, out var studentId))
            throw new ArgumentException($"Invalid student id: '{studentIdText}'.");

        var result = assignStudentToCohortUseCase.Execute(new AssignStudentToCohortCommand(cohortId, studentId));

        Console.WriteLine();
        Console.WriteLine("Student assigned successfully:");
        Console.WriteLine($"  Cohort:   {result.Name}");
        Console.WriteLine($"  Students: {string.Join(", ", result.StudentIds)}");
    });
}

void RunEnrollStudentInOpenCourse()
{
    Console.Write("Course id: ");
    var courseIdText = Console.ReadLine() ?? string.Empty;

    Console.Write("Student id: ");
    var studentIdText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(courseIdText, out var courseId))
            throw new ArgumentException($"Invalid course id: '{courseIdText}'.");

        if (!Guid.TryParse(studentIdText, out var studentId))
            throw new ArgumentException($"Invalid student id: '{studentIdText}'.");

        var result = enrollStudentInOpenCourseUseCase.Execute(new EnrollStudentInOpenCourseCommand(courseId, studentId));

        PrintEnrollmentResult(result);
    });
}

void RunRequestEnrollment()
{
    Console.Write("Course id: ");
    var courseIdText = Console.ReadLine() ?? string.Empty;

    Console.Write("Student id: ");
    var studentIdText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(courseIdText, out var courseId))
            throw new ArgumentException($"Invalid course id: '{courseIdText}'.");

        if (!Guid.TryParse(studentIdText, out var studentId))
            throw new ArgumentException($"Invalid student id: '{studentIdText}'.");

        var result = requestEnrollmentUseCase.Execute(new RequestEnrollmentCommand(courseId, studentId));

        PrintEnrollmentResult(result);
    });
}

void RunApproveEnrollment()
{
    Console.Write("Enrollment id: ");
    var enrollmentIdText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(enrollmentIdText, out var enrollmentId))
            throw new ArgumentException($"Invalid enrollment id: '{enrollmentIdText}'.");

        var result = approveEnrollmentUseCase.Execute(new ApproveEnrollmentCommand(enrollmentId));

        PrintEnrollmentResult(result);
    });
}

void RunRejectEnrollment()
{
    Console.Write("Enrollment id: ");
    var enrollmentIdText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(enrollmentIdText, out var enrollmentId))
            throw new ArgumentException($"Invalid enrollment id: '{enrollmentIdText}'.");

        var result = rejectEnrollmentUseCase.Execute(new RejectEnrollmentCommand(enrollmentId));

        PrintEnrollmentResult(result);
    });
}

void RunEnrollByInvitation()
{
    Console.Write("Invitation code: ");
    var invitationCode = Console.ReadLine() ?? string.Empty;

    Console.Write("Student id: ");
    var studentIdText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(studentIdText, out var studentId))
            throw new ArgumentException($"Invalid student id: '{studentIdText}'.");

        var result = enrollByInvitationUseCase.Execute(new EnrollByInvitationCommand(invitationCode, studentId));

        PrintEnrollmentResult(result);
    });
}

void RunCreateSection()
{
    Console.Write("Course id: ");
    var courseIdText = Console.ReadLine() ?? string.Empty;

    Console.Write("Section title: ");
    var title = Console.ReadLine() ?? string.Empty;

    Console.Write("Description (HTML allowed): ");
    var descriptionHtml = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(courseIdText, out var courseId))
            throw new ArgumentException($"Invalid course id: '{courseIdText}'.");

        var result = createSectionUseCase.Execute(new CreateSectionCommand(courseId, title, descriptionHtml));

        Console.WriteLine();
        Console.WriteLine("Section created successfully:");
        Console.WriteLine($"  Id:               {result.Id}");
        Console.WriteLine($"  CourseId:         {result.CourseId}");
        Console.WriteLine($"  Title:            {result.Title}");
        Console.WriteLine($"  DescriptionHtml:  {result.DescriptionHtml}");
    });
}

void RunCreateActivity()
{
    Console.Write("Section id: ");
    var sectionIdText = Console.ReadLine() ?? string.Empty;

    Console.Write("Activity title: ");
    var title = Console.ReadLine() ?? string.Empty;

    Console.Write("Description: ");
    var description = Console.ReadLine() ?? string.Empty;

    Console.Write("Due date (yyyy-MM-dd): ");
    var dueDateText = Console.ReadLine() ?? string.Empty;

    Console.Write($"Type ({string.Join(" / ", Enum.GetNames<ActivityType>())}): ");
    var typeText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(sectionIdText, out var sectionId))
            throw new ArgumentException($"Invalid section id: '{sectionIdText}'.");

        if (!DateTime.TryParse(dueDateText, out var dueDate))
            throw new ArgumentException($"Invalid due date: '{dueDateText}'.");

        if (!Enum.TryParse<ActivityType>(typeText, ignoreCase: true, out var type))
            throw new ArgumentException($"Invalid activity type: '{typeText}'.");

        int? maxFiles = null;
        if (type == ActivityType.ConArchivo)
        {
            Console.Write("Max files: ");
            var maxFilesText = Console.ReadLine() ?? string.Empty;

            if (!int.TryParse(maxFilesText, out var parsedMaxFiles))
                throw new ArgumentException($"Invalid max files: '{maxFilesText}'.");

            maxFiles = parsedMaxFiles;
        }

        var result = createActivityUseCase.Execute(
            new CreateActivityCommand(sectionId, title, description, dueDate.ToUniversalTime(), type, maxFiles));

        Console.WriteLine();
        Console.WriteLine("Activity created successfully:");
        Console.WriteLine($"  Id:         {result.Id}");
        Console.WriteLine($"  SectionId:  {result.SectionId}");
        Console.WriteLine($"  Title:      {result.Title}");
        Console.WriteLine($"  DueDateUtc: {result.DueDateUtc:O}");
        Console.WriteLine($"  Type:       {result.Type}");
        Console.WriteLine($"  MaxFiles:   {result.MaxFiles?.ToString() ?? "(n/a)"}");
    });
}

void RunSubmitTextActivity()
{
    Console.Write("Activity id: ");
    var activityIdText = Console.ReadLine() ?? string.Empty;

    Console.Write("Student id: ");
    var studentIdText = Console.ReadLine() ?? string.Empty;

    Console.Write("Text content: ");
    var textContent = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(activityIdText, out var activityId))
            throw new ArgumentException($"Invalid activity id: '{activityIdText}'.");

        if (!Guid.TryParse(studentIdText, out var studentId))
            throw new ArgumentException($"Invalid student id: '{studentIdText}'.");

        var result = submitTextActivityUseCase.Execute(new SubmitTextCommand(activityId, studentId, textContent));

        PrintSubmissionResult(result);
    });
}

void RunSubmitFilesActivity()
{
    Console.Write("Activity id: ");
    var activityIdText = Console.ReadLine() ?? string.Empty;

    Console.Write("Student id: ");
    var studentIdText = Console.ReadLine() ?? string.Empty;

    var filePaths = new List<string>();
    Console.WriteLine("Enter file paths, one per line (blank line to finish):");
    while (true)
    {
        Console.Write("File path: ");
        var path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path))
            break;

        filePaths.Add(path);
    }

    RunSafely(() =>
    {
        if (!Guid.TryParse(activityIdText, out var activityId))
            throw new ArgumentException($"Invalid activity id: '{activityIdText}'.");

        if (!Guid.TryParse(studentIdText, out var studentId))
            throw new ArgumentException($"Invalid student id: '{studentIdText}'.");

        var files = filePaths.Select(path =>
        {
            byte[] content;
            try
            {
                content = File.ReadAllBytes(path);
            }
            catch (IOException ex)
            {
                throw new ArgumentException($"Could not read file '{path}': {ex.Message}");
            }

            return new SubmittedFileInput(Path.GetFileName(path), content);
        }).ToList();

        var result = submitFilesActivityUseCase.Execute(new SubmitFilesCommand(activityId, studentId, files));

        PrintSubmissionResult(result);
    });
}

void RunGradeSubmission()
{
    Console.Write("Submission id: ");
    var submissionIdText = Console.ReadLine() ?? string.Empty;

    Console.Write("Score (0-100): ");
    var scoreText = Console.ReadLine() ?? string.Empty;

    Console.Write("Feedback (optional): ");
    var feedback = Console.ReadLine();

    RunSafely(() =>
    {
        if (!Guid.TryParse(submissionIdText, out var submissionId))
            throw new ArgumentException($"Invalid submission id: '{submissionIdText}'.");

        if (!int.TryParse(scoreText, out var score))
            throw new ArgumentException($"Invalid score: '{scoreText}'.");

        var result = gradeSubmissionUseCase.Execute(new GradeSubmissionCommand(submissionId, score, feedback));

        PrintSubmissionResult(result);
    });
}

void PrintSubmissionResult(SubmissionResult result)
{
    Console.WriteLine();
    Console.WriteLine("Submission:");
    Console.WriteLine($"  Id:             {result.Id}");
    Console.WriteLine($"  ActivityId:     {result.ActivityId}");
    Console.WriteLine($"  StudentId:      {result.StudentId}");
    Console.WriteLine($"  Status:         {result.Status}");
    Console.WriteLine($"  SubmittedAtUtc: {result.SubmittedAtUtc:O}");
    if (result.TextContent is not null)
        Console.WriteLine($"  Text:           {result.TextContent}");
    if (result.Files.Count > 0)
    {
        Console.WriteLine("  Files:");
        foreach (var file in result.Files)
            Console.WriteLine($"    - {file.FileName} ({file.SizeBytes} bytes) -> {file.StorageKey}");
    }
    Console.WriteLine($"  Score:          {result.Score?.ToString() ?? "(not graded)"}");
    Console.WriteLine($"  Feedback:       {result.Feedback ?? "(none)"}");
}

void RunLogin()
{
    Console.Write("Email: ");
    var email = Console.ReadLine() ?? string.Empty;

    Console.Write("Password: ");
    var password = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        var result = loginUseCase.Execute(new LoginCommand(email, password));

        Console.WriteLine();
        Console.WriteLine("Login successful:");
        Console.WriteLine($"  UserId:    {result.UserId}");
        Console.WriteLine($"  Email:     {result.Email}");
        Console.WriteLine($"  Role:      {result.Role}");
        Console.WriteLine($"  ExpiresAt: {result.ExpiresAtUtc:O}");
        Console.WriteLine($"  Token:     {result.Token}");
    });
}

void RunLogout()
{
    Console.Write("Token: ");
    var token = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        logoutUseCase.Execute(new LogoutCommand(token));

        Console.WriteLine();
        Console.WriteLine("Logged out. Token revoked.");
    });
}

void RunValidateToken()
{
    Console.Write("Token: ");
    var token = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        var result = validateTokenUseCase.Execute(new ValidateTokenCommand(token));

        Console.WriteLine();
        Console.WriteLine("Token is valid:");
        Console.WriteLine($"  UserId: {result.UserId}");
        Console.WriteLine($"  Email:  {result.Email}");
        Console.WriteLine($"  Role:   {result.Role}");
    });
}

void PrintEnrollmentResult(EnrollmentResult result)
{
    Console.WriteLine();
    Console.WriteLine("Enrollment:");
    Console.WriteLine($"  Id:             {result.Id}");
    Console.WriteLine($"  CourseId:       {result.CourseId}");
    Console.WriteLine($"  StudentId:      {result.StudentId}");
    Console.WriteLine($"  Status:         {result.Status}");
    Console.WriteLine($"  RequestedAtUtc: {result.RequestedAtUtc:O}");
    Console.WriteLine($"  DecidedAtUtc:   {result.DecidedAtUtc?.ToString("O") ?? "(pending)"}");
}

void RunNotifyNewActivity()
{
    Console.Write("Activity id: ");
    var activityIdText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(activityIdText, out var activityId))
            throw new ArgumentException($"Invalid activity id: '{activityIdText}'.");

        var results = notifyNewActivityUseCase.Execute(new NotifyNewActivityCommand(activityId));

        PrintNotificationResults(results);
    });
}

void RunNotifyNewSection()
{
    Console.Write("Section id: ");
    var sectionIdText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(sectionIdText, out var sectionId))
            throw new ArgumentException($"Invalid section id: '{sectionIdText}'.");

        var results = notifyNewSectionUseCase.Execute(new NotifyNewSectionCommand(sectionId));

        PrintNotificationResults(results);
    });
}

void RunSendDueDateReminder()
{
    Console.Write("Activity id: ");
    var activityIdText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(activityIdText, out var activityId))
            throw new ArgumentException($"Invalid activity id: '{activityIdText}'.");

        var results = sendDueDateReminderUseCase.Execute(new SendDueDateReminderCommand(activityId));

        PrintNotificationResults(results);
    });
}

void RunMarkNotificationAsRead()
{
    Console.Write("Notification id: ");
    var notificationIdText = Console.ReadLine() ?? string.Empty;

    Console.Write("Your user id (recipient): ");
    var requestingUserIdText = Console.ReadLine() ?? string.Empty;

    RunSafely(() =>
    {
        if (!Guid.TryParse(notificationIdText, out var notificationId))
            throw new ArgumentException($"Invalid notification id: '{notificationIdText}'.");

        if (!Guid.TryParse(requestingUserIdText, out var requestingUserId))
            throw new ArgumentException($"Invalid user id: '{requestingUserIdText}'.");

        var result = markNotificationAsReadUseCase.Execute(new MarkNotificationAsReadCommand(notificationId, requestingUserId));

        Console.WriteLine();
        Console.WriteLine("Notification marked as read:");
        Console.WriteLine($"  Id:       {result.Id}");
        Console.WriteLine($"  ReadAtUtc: {result.ReadAtUtc:O}");
    });
}

void PrintNotificationResults(IReadOnlyCollection<NotificationResult> results)
{
    Console.WriteLine();
    Console.WriteLine($"{results.Count} notification(s) created:");
    foreach (var result in results)
    {
        Console.WriteLine($"  - Id: {result.Id} | Recipient: {result.RecipientUserId} | Type: {result.Type} | EmailSent: {result.EmailSent}");
        Console.WriteLine($"    Title:   {result.Title}");
        Console.WriteLine($"    Message: {result.Message}");
    }
}

void RunSafely(Action action)
{
    try
    {
        action();
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        Console.WriteLine();
        Console.WriteLine($"Validation error: {ex.Message}");
    }
}
