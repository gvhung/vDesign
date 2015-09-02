
namespace Base.Security
{
    public interface IUserCategorizedItem : ICategorizedItem
    {
        UserCategory UserCategory { get; set; }
    }
}
