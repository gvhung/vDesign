using Base.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Security.ObjectAccess.Policies
{
    public class EditCreatorOnlyAccessPolicy : DefaultAccessPolicy
    {
        public EditCreatorOnlyAccessPolicy(IUnitOfWorkFactory unitOfWorkFactory)
            : base(unitOfWorkFactory)
        {
        }

        public override ObjectAccessItem InitializeAccessItem(ObjectAccessItem accessItem, int userID, Type objectType)
        {
            var access = base.InitializeAccessItem(accessItem, userID, objectType);

            access.DeleteAll = access.UpdateAll = access.ChangeAccessAll = false;

            access.UserCategories = access.UserCategories ?? new List<UserCategoryAccess>();

            foreach (var category in access.UserCategories)
                category.Update = category.Delete = category.ChangeAccess = false;

            access.Users = access.Users ?? new List<UserAccess>();

            foreach (var user in access.Users.Where(x => x.UserID != userID))
                user.Update = user.Delete = user.ChangeAccess = false;

            return access;
        }
    }
}