using Base.DAL;
using Base.Security.ObjectAccess;
using Base.Security.ObjectAccess.Policies;
using Base.Security.Service;
using System;
using System.Collections.Generic;

namespace Data.Strategies
{
    public class NpaQuestionTemplateAccessPolicy
     : DefaultAccessPolicy
    {
        private readonly IUserCategoryService _userCategoryService;

        public NpaQuestionTemplateAccessPolicy(IUnitOfWorkFactory uowFactory, IUserCategoryService userCategoryService)
            : base(uowFactory)
        {
            _userCategoryService = userCategoryService;
        }

        public override ObjectAccessItem InitializeAccessItem(ObjectAccessItem accessItem, int userID, Type objectType)
        {
            accessItem.DeleteAll = accessItem.ReadAll = accessItem.UpdateAll = accessItem.ChangeAccessAll = false;

            accessItem.Users = new List<UserAccess> { new UserAccess { UserID = userID } };
            accessItem.UserCategories = new List<UserCategoryAccess>();

            return accessItem;
        }
    }
}
