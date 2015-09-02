using Base.Content.Service.Abstract;
using Base.DAL;
using Base.Security;
using Base.Security.Service.Abstract;
using Base.Service;
using Base.Content.Entities;
using System;

namespace Base.Content.Service.Concrete
{
    public class RequestService : BaseObjectService<Request>, IRequestService
    {
        public RequestService(IDefaultUnitOfWork context, ISecurityService securityService)
            : base(context, securityService) { }

        public virtual void Create(ISecurityUser securityUser, string name, string email, string subject, string text)
        {
            var request = new Request()
            {
                Name = name,
                EMail = email,
                Subject = subject,
                Text = text
            };

            this.TCreate(securityUser, request);
        }

        protected override IObjectSaver<Request> GetForSave(ISecurityUser securityUser, IObjectSaver<Request> objectSaver)
        {
            if (objectSaver.IsNew)
                objectSaver.Dest.Date = DateTime.Now;

            return base.GetForSave(securityUser, objectSaver);
        }
    }
}
