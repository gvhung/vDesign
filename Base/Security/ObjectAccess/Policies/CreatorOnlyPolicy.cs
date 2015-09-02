using Base.DAL;
using System;
using System.Linq;

namespace Base.Security.ObjectAccess.Policies
{
    public class CreatorOnly : DefaultAccessPolicy
    {
        public CreatorOnly(IUnitOfWorkFactory unitOfWorkFactory)
            : base(unitOfWorkFactory)
        {
        }

        public override ObjectAccessItem InitializeAccessItem(ObjectAccessItem accessItem, int userID, Type objectType)
        {
            var access = base.InitializeAccessItem(accessItem, userID, objectType);
            var owner = access.Users.FirstOrDefault(x => x.UserID == userID);

            access.ReadAll = access.UpdateAll = access.DeleteAll = access.ChangeAccessAll = false;

            access.UserCategories.Clear();
            access.Users.Clear();

            access.Users.Add(owner);

            return accessItem;
        }
    }
}