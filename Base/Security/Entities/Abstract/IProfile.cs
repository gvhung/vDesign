
namespace Base.Security
{
    public interface IProfile
    {
        FileData Image { get; set; }
        string Login { get; set; }
        string LastName { get; set; }
        string FirstName { get; set; }
        string MiddleName { get; set; }
        string FullName { get; }
        string Email { get; set; }
        string Password { get; set; }
    }
}
