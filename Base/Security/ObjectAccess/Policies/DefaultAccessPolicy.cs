using Base.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Security.ObjectAccess.Policies
{
    public class DefaultAccessPolicy : BaseAccessPolicy
    {
        private readonly ISystemUnitOfWork _systemUnitOfWork;

        public DefaultAccessPolicy(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _systemUnitOfWork = unitOfWorkFactory.CreateSystem();
        }

        protected IUnitOfWork UnitOfWork
        {
            get { return _systemUnitOfWork; }
        }

        public override ObjectAccessItem InitializeAccessItem(ObjectAccessItem accessItem, int userID, Type objectType)
        {
            var user = _systemUnitOfWork.GetRepository<User>().All().FirstOrDefault(x => !x.Hidden && x.ID == userID);

            if (user != null && user.UserCategory != null)
            {
                var nearestCompanies = new List<UserCategory>();

                if (user.UserCategory.Company)
                    nearestCompanies.Add(user.UserCategory);

                if (user.UserCategory.sys_all_parents != null)
                {
                    var parentIDs = user.UserCategory.sys_all_parents.Split(HCategory.Seperator)
                    .Select(HCategory.IdToInt).Reverse();

                    nearestCompanies.AddRange(_systemUnitOfWork.GetRepository<UserCategory>().All()
                            .Where(x => !x.Hidden && parentIDs.Contains(x.ID) && x.Company));
                }

                if (nearestCompanies.Any())
                    accessItem.ReadAll = accessItem.UpdateAll = accessItem.DeleteAll = accessItem.ChangeAccessAll = false;

                accessItem.UserCategories = nearestCompanies.Select(x => new UserCategoryAccess
                {
                    UserCategoryID = x.ID,
                    ObjectAccessItem = accessItem,
                    ChangeAccess = false
                }).ToList();

                accessItem.Users.Add(new UserAccess
                {
                    UserID = user.ID
                });
            }

            return accessItem;
        }
    }
}