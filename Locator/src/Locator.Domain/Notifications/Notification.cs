namespace Locator.Domain.Notifications;

public class Notification
{
    public Notification(string text, Guid[] userId)
    {
        Text = text;
        UserId = userId;
    }
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Guid[] UserId { get; set; }
}