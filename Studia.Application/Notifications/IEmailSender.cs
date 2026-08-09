namespace Studia.Application.Notifications;

public interface IEmailSender
{
    void Send(string toEmail, string subject, string body);
}
