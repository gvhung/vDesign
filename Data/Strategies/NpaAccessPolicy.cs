using Base.DAL;
using Base.Security.ObjectAccess;
using Base.Security.ObjectAccess.Policies;
using Base.Security.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Strategies
{
    public class NpaAccessPolicy : DefaultAccessPolicy
    {
        private readonly IUserCategoryService _userCategoryService;

        public NpaAccessPolicy(IUnitOfWorkFactory uowFactory, IUserCategoryService userCategoryService)
            : base(uowFactory)
        {
            _userCategoryService = userCategoryService;
        }

        public override ObjectAccessItem InitializeAccessItem(ObjectAccessItem accessItem, int userID, Type objectType)
        {
            accessItem.DeleteAll = accessItem.UpdateAll = accessItem.ChangeAccessAll = false;

            accessItem.Users.Add(new UserAccess
                {
                    UserID = userID
                });

            int? categoryID = _userCategoryService.GetAll(UnitOfWork).Where(x => x.SystemName == "Methodist").Select(x => x.ID).FirstOrDefault();

            if (categoryID != null)
            {
                accessItem.UserCategories = new List<UserCategoryAccess>()
                {
                    new UserCategoryAccess(){
                        UserCategoryID = (int)categoryID,
                        ObjectAccessItem = accessItem,
                        ChangeAccess = false
                    }
                };
            }

            return accessItem;
        }
    }
}
