using System.ComponentModel;

namespace Base.Security
{
    public enum UserType
    {
        [Description("Пользователи")]
        Base = 0,
        [Description("Эксперты")]
        Expert = 5,
        [Description("Разработчики")]
        Developer = 10
    }
}
