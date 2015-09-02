using Base.Attributes;

namespace Base.Security.ObjectAccess
{
    public class UserCategoryAccess : BaseObject
    {
        public int? ObjectAccessItemID { get; set; }
        public virtual ObjectAccessItem ObjectAccessItem { get; set; }

        public int UserCategoryID { get; set; }

        [DetailView("Группа пользователей", Required = true)]
        [ListView]
        public virtual UserCategory UserCategory { get; set; }

        [DetailView("Чтение")]
        [ListView]
        public bool Read { get; set; }
        [DetailView("Изменение")]
        [ListView]
        public bool Update { get; set; }
        [DetailView("Удаление")]
        [ListView]
        public bool Delete { get; set; }
        [DetailView("Изменение доступа")]
        [ListView]
        public bool ChangeAccess { get; set; }

        public UserCategoryAccess()
        {
            this.Read = true;
            this.Update = true;
            this.Delete = true;
            this.ChangeAccess = true;
        }
    }
}
