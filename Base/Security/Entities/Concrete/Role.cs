using Base.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Base.Security
{
    public class Role: BaseObject
    {
        public Role()
        {
            ChildRoles = new List<ChildRole>();
            Permissions = new List<Permission>();
        }

        [DetailView(Name = "Наименование", Required = true)]
        [MaxLength(255)]
        [ListView]
        public string Name { get; set; }        

        [DetailView(TabName = "[1]Разрешения")]
        public virtual ICollection<Permission> Permissions { get; set; }

        [DetailView("Роли", TabName = "[2]Роли")]
        [PropertyDataType("EasyCollection", Params = "Role;Role;Name")]
        [InverseProperty("Parent")]
        public virtual ICollection<ChildRole> ChildRoles { get; set; }

        [DetailView("Системная роль", TabName = "[2]Роли")]
        public SystemRole? SystemRole { get; set; }

        [JsonIgnore]
        public ICollection<User> Users { get; set; }

        [JsonIgnore]
        public IEnumerable<Role> Roles
        {
            get
            {
                return this.ChildRoles != null ? this.ChildRoles.Select(m => m.Role).AsEnumerable() : null;
            }
        }
    }

    public class ChildRole: BaseObject
    {
        public int? ParentID { get; set; }
        [JsonIgnore]
        public virtual Role Parent { get; set; }

        public int RoleID { get; set; }
        [DetailView(Name = "Роль", Required = true)]
        [ListView]
        public virtual Role Role { get; set; }
    }
}
