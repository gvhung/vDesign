using Base.DAL;
using Base.OpenID.Entities;
using Base.OpenID.Service.Abstract;
using Base.Security;
using Base.Security.Service;
using Base.Security.Service.Abstract;
using Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.OpenID.Service.Concrete
{
    public class ExtProfileService : BaseProfileService<ExtProfile>, IExtProfileService
    {
        private readonly IProfileService _profileService;
        private readonly IExtAccountService _extAccountService;
        private readonly IOpenIdConfigService _openIdConfigService;

        public ExtProfileService(ISecurityUserService securityUserService, IBaseObjectServiceFacade facade,
                                 IProfileService profileService, IExtAccountService extAccountService, IOpenIdConfigService openIdConfigService) 
            : base(securityUserService, facade)
        {
            _profileService = profileService;
            _extAccountService = extAccountService;
            _openIdConfigService = openIdConfigService;
        }

        public override ExtProfile Get(IUnitOfWork unitOfWork, int id)
        {
            var profile = _profileService.Get(unitOfWork, id);
            var extUserInfo = _extAccountService.GetAll(unitOfWork).Where(x => x.UserID == id).ToList();

            ExtProfile extProfile = _Combine(profile, extUserInfo);

            return extProfile;
        }

        public override ExtProfile Update(IUnitOfWork unitOfWork, ExtProfile obj)
        {
            var profile = _profileService.Update(unitOfWork, obj.ToObject<Profile>());

            bool hasPassword = unitOfWork.GetRepository<User>().Find(x => x.ID == obj.ID).Password != null;
            if (!hasPassword && (obj.AccountInfos == null || !obj.AccountInfos.Any()))
            {
                throw new Exception("Требуется задать пароль для профиля перед удалением всех связанных аккаунтов.");
            }

            var linkedAccounts = _extAccountService.GetLinked(unitOfWork, obj.ID);
            var removedAccounts = linkedAccounts.Where(x => obj.AccountInfos == null || !obj.AccountInfos.Select(z => z.ID).Contains(x.ID)).ToList();

            if (removedAccounts.Any()) _extAccountService.DeleteCollection(unitOfWork, removedAccounts);

            var extprofile = _Combine(profile, obj.AccountInfos);

            unitOfWork.SaveChanges();

            return extprofile;
        }
        
        private ExtProfile _Combine(Profile profile, IEnumerable<ExtAccount> userInfos)
        {
            ExtProfile extProfile = profile.ToObject<ExtProfile>();

            if (userInfos == null) return extProfile;

            var extAccounts = userInfos as IList<ExtAccount> ?? userInfos.ToList();
            if (extAccounts.Any())
            {
                var openIdConfig = _openIdConfigService.GetConfig();
                extProfile.AccountInfos = extAccounts.Join(openIdConfig, outer => outer.Type, inner => inner.Type, (outer, inner) => new ExtAccount
                {
                    ID = outer.ID,
                    UserID = outer.UserID,
                    ExternalId = outer.ExternalId,
                    Type = outer.Type,
                    Login = outer.Login,
                    Email = outer.Email,
                    FirstName = outer.FirstName,
                    LastName = outer.LastName,
                    ProfileLink = outer.ProfileLink,
                    IconCssClass = inner.IconCssClass
                }).ToList();
            }

            return extProfile;
        }

        public override ExtProfile Create(IUnitOfWork unitOfWork, ExtProfile obj)
        {
            throw new NotImplementedException();
            return base.Create(unitOfWork, obj);
        }

        public override IQueryable<ExtProfile> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            throw new NotImplementedException();
            return base.GetAll(unitOfWork, hidden);
        }
    }
}
