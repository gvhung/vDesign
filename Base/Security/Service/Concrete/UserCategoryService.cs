using Base.DAL;
using Base.Service;

using System;
using System.Linq;

namespace Base.Security.Service
{
    public class UserCategoryService : BaseCategoryService<UserCategory>, IUserCategoryService
    {
        public UserCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }

        protected override IObjectSaver<UserCategory> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<UserCategory> objectSaver)
        {
            if (objectSaver.Src.Company && objectSaver.Src.CompanyGuid == null)
            {
                objectSaver.Dest.CompanyGuid = Guid.NewGuid();
            }

            return base.GetForSave(unitOfWork, objectSaver).SaveOneObject(x => x.Image);
        }

        public UserCategory GetCompany(IUnitOfWork unitOfWork, UserCategory category)
        {
            UserCategory ctg;
            
            if (category.Company)
            {
                ctg = category;
            }
            else
            {
                ctg = unitOfWork.GetRepository<UserCategory>().All().Where(cat => cat.Company)
                          .Join((category.sys_all_parents ?? "-1").Split(HCategory.Seperator).Select(x => HCategory.IdToInt(x)), outer => outer.ID, inner => inner, (outer, inner) => outer)
                          .OrderByDescending(x => x.sys_all_parents).FirstOrDefault();
            }

            return ctg;
        }
    }
}
