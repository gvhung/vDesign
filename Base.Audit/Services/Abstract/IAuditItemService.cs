using Base.Audit.Entities;
using Base.DAL;
using Base.Service;
using System;
using System.Threading.Tasks;

namespace Base.Audit.Services
{
    public interface IAuditItemService : IBaseObjectService<AuditItem>, IReadOnly
    {
        bool IsAudit(Type type);
        bool IsAudit<T>() where T : BaseObject;
        Task ToJornalAsync(IUnitOfWork unitOfWork, TypeAuditItem type, BaseObject obj, string desc = null);
        void ResetCache();
    }
}
