using Base.DAL;
using Base.Employee.Entities;
using Base.Employee.Service.Abstract;
using Base.Security;
using Base.Security.Service;
using Base.Security.Service.Abstract;
using Base.Settings;

namespace Base.Employee.Service.Concrete
{
    public class EmployeeService : UserService, IEmployeeService
    {
        public EmployeeService(IDefaultUnitOfWork context, ISettingItemService settingsService,
            ISecurityUserService securityUserService, ISecurityService securityService) : base(context, settingsService, securityUserService, securityService) { }

        //protected override User GetForSave(ISecurityUser securityUser, User obj)
        //{
        //    Entities.Employee objDest = base.GetForSave(securityUser, obj) as Entities.Employee;

        //    Entities.Employee objSrc = obj as Entities.Employee;

        //    if (objSrc.Post != null)
        //    {
        //        objDest.Post = null;

        //        objDest.PostID = objSrc.Post.ID;
        //    }
        //    else
        //    {
        //        objDest.PostID = null;
        //    }

        //    this.UnitOfWork.SaveOneToMany(objSrc, objDest, x => x.Jobs, x => x.EmployeeID);

        //    this.UnitOfWork.SaveOneToMany(objSrc, objDest, x => x.Phones, x => x.EmployeeID);

        //    this.UnitOfWork.SaveOneToMany(objSrc, objDest, x => x.Files, x => x.EmployeeID);

        //    this.UnitOfWork.SaveOneToMany(objSrc, objDest, x => x.Qualifications, x => x.EmployeeID);

        //    this.UnitOfWork.SaveOneToMany(objSrc, objDest, x => x.FamilyMembers, x => x.EmployeeID,
        //        beforCollectionEntrySave: x => this.UnitOfWork.SaveOneObject(x, s => s.FamilyMembersType, s => x.FamilyMembersTypeID),
        //        whenCollectionEntryUpdate: (src, dest) =>
        //            {
        //                this.UnitOfWork.SaveOneToMany(src, dest, x => x.Jobs, x => x.FamilyMemberID);

        //                this.UnitOfWork.SaveOneToMany(src, dest, x => x.Phones, x => x.FamilyMemberID);
        //            });

        //    return objDest;
        //}
    }
}