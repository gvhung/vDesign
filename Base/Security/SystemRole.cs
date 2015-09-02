using System.ComponentModel;

namespace Base.Security
{
    public enum SystemRole
    {
        [Description("Администраторы")]
        Admin = 0,
        [Description("Администраторы бизнес-процессов")]
        AdminWF = 1,
        [Description("Высшие должностные лица")]
        Ceo = 10,
        [Description("Методисты")]
        Methodist = 11,
        [Description("Разработчики")]
        Developer = 16,
        [Description("Эксперты")]
        Expert = 13,
        [Description("Эксперты по антикоррупции")]
        AnticorruptionExpert = 15,
        [Description("Базовая")]
        Base = 20,
    }
}
