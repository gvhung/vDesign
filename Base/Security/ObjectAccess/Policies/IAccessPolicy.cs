using System;

namespace Base.Security.ObjectAccess.Policies
{
    public interface IAccessPolicy
    {
        ObjectAccessItem InitializeAccessItem(ObjectAccessItem accessItem, int userID, Type objectType);
    }
}