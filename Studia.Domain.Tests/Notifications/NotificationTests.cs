using Studia.Domain.Notifications;

namespace Studia.Domain.Tests.Notifications;

public class NotificationTests
{
    [Fact]
    public void Create_WithValidData_StartsUnreadAndEmailNotSent()
    {
        var recipientId = Guid.NewGuid();
        var relatedId = Guid.NewGuid();

        var notification = Notification.Create(recipientId, NotificationType.NuevaActividad, "Título", "Mensaje", relatedId);

        Assert.Equal(recipientId, notification.RecipientUserId);
        Assert.Equal(relatedId, notification.RelatedEntityId);
        Assert.Null(notification.ReadAtUtc);
        Assert.False(notification.EmailSent);
    }

    [Fact]
    public void Create_WithEmptyRecipient_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Notification.Create(Guid.Empty, NotificationType.NuevaActividad, "Título", "Mensaje"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankTitle_Throws(string blankTitle)
    {
        Assert.Throws<ArgumentException>(() =>
            Notification.Create(Guid.NewGuid(), NotificationType.NuevaActividad, blankTitle, "Mensaje"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankMessage_Throws(string blankMessage)
    {
        Assert.Throws<ArgumentException>(() =>
            Notification.Create(Guid.NewGuid(), NotificationType.NuevaActividad, "Título", blankMessage));
    }

    [Fact]
    public void MarkAsRead_SetsReadAtUtc()
    {
        var notification = Notification.Create(Guid.NewGuid(), NotificationType.NuevaActividad, "Título", "Mensaje");

        notification.MarkAsRead();

        Assert.NotNull(notification.ReadAtUtc);
    }

    [Fact]
    public void MarkAsRead_CalledTwice_KeepsFirstTimestamp()
    {
        var notification = Notification.Create(Guid.NewGuid(), NotificationType.NuevaActividad, "Título", "Mensaje");

        notification.MarkAsRead();
        var firstReadAt = notification.ReadAtUtc;
        notification.MarkAsRead();

        Assert.Equal(firstReadAt, notification.ReadAtUtc);
    }

    [Fact]
    public void MarkEmailSent_SetsEmailSentTrue()
    {
        var notification = Notification.Create(Guid.NewGuid(), NotificationType.NuevaActividad, "Título", "Mensaje");

        notification.MarkEmailSent();

        Assert.True(notification.EmailSent);
    }
}
