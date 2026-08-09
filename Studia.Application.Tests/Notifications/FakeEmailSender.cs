using Studia.Application.Notifications;

namespace Studia.Application.Tests.Notifications;

public class FakeEmailSender : IEmailSender
{
    public List<(string ToEmail, string Subject, string Body)> SentEmails { get; } = [];

    public void Send(string toEmail, string subject, string body) => SentEmails.Add((toEmail, subject, body));
}
