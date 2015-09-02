using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Security.Entities.Concrete
{
    public class UserStatus : IUserStatus
    {
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
        public bool Online { get; set; }
        public DateTime LastDate { get; set; }
    }
}
