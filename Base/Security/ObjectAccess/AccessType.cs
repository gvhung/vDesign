using System;

namespace Base.Security.ObjectAccess
{
    [Flags]
    public enum AccessType
    {
        None   = 0,
        Read   = 1 << 0,
        Update = 1 << 1,
        Delete = 1 << 2,

        Full   = Read | Update | Delete
    }
}
