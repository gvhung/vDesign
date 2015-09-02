using Base.Attributes;
using Newtonsoft.Json;
using System;

namespace Base.Security
{
    public class Permission: BaseObject
    {
        public int RoleID { get; set; }
        [JsonIgnore]
        public Role Role { get; set; }

        [DetailView(Name = "Объект")]
        [ListView]
        [PropertyDataType("ListBaseObjects")]
        public string FullName { get; set; }
        [DetailView(Name = "Чтение")]
        [ListView]
        public bool AllowRead { get; set; }
        [DetailView(Name = "Запись")]
        [ListView]
        public bool AllowWrite { get; set; }
        [DetailView(Name = "Создание")]
        [ListView]
        public bool AllowCreate { get; set; }
        [DetailView(Name = "Удаление")]
        [ListView]
        public bool AllowDelete { get; set; }
        [DetailView(Name = "Навигация")]
        [ListView]
        public bool AllowNavigate { get; set; }
    }

    [Flags]
    public enum TypePermission
    {
        Read = 0,
        Write = 1,
        Create = 2,
        Delete = 3,
        Navigate = 4
    }
}
