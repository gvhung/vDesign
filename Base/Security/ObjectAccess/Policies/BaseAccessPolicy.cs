using System;

namespace Base.Security.ObjectAccess.Policies
{
    public class BaseAccessPolicy : IAccessPolicy
    {
        public virtual ObjectAccessItem InitializeAccessItem(ObjectAccessItem accessItem, int userID, Type objectType)
        {
            return accessItem;
        }
    }
}