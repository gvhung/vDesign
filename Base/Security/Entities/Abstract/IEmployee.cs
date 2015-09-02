
namespace Base.Security
{
    public interface IEmployee
    {
        FileData Image { get; set; }
        int? PostID { get; set; }
        Post Post { get; set; }
        string LastName { get; set; }
        string FirstName { get; set; }
        string MiddleName { get; set; }
        string FullName { get; }
        string Email { get; set; }
        string OfficePhone { get; set; }
        //string InternalPhone { get; set; }
        string PersonPhone { get; set; }
        string MailAddress { get; set; }
    }
}
