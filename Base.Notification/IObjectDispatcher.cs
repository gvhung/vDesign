
namespace Base.Notification
{
    public interface IObjectDispatcher
    {
        int ObjectID { get; set; }
        string GetTemplateName();
    }
}