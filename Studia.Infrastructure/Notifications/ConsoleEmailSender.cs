using Studia.Application.Notifications;

namespace Studia.Infrastructure.Notifications;

public class ConsoleEmailSender : IEmailSender
{
    public void Send(string toEmail, string subject, string body)
    {
        Console.WriteLine();
        Console.WriteLine($"[dev-email] To: {toEmail}");
        Console.WriteLine($"[dev-email] Subject: {subject}");
        Console.WriteLine($"[dev-email] Body: {body}");
    }
}
