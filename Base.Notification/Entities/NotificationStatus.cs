using System.ComponentModel;

namespace Base.Notification.Entities
{
    public enum NotificationStatus
    {
        [Description("Новое")]
        New = 0,
        [Description("Просмотрено")]
        Viewed = 1
    }
}
