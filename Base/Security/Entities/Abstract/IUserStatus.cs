using System;

namespace Base.Security
{
    public interface IUserStatus
    {
        int UserId { get; set; }
        string ConnectionId { get; set; }
        bool Online { get; set; }
        DateTime LastDate { get; set; }
    }
}