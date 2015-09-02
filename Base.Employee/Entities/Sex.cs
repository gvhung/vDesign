using System.ComponentModel;

namespace Base.Employee.Entities
{
    public enum Sex
    {
        [Description("Мужской")]
        Man = 0,
        [Description("Женский")]
        Woman = 1
    }
}