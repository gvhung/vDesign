using Base.DAL;
using Base.Nomenclature.Entities;
using Base.Service;
using System.Collections.Generic;

namespace Base.Nomenclature.Service
{
    public interface IOkpdHierarchyService : IBaseCategoryService<OkpdHierarchy>
    {
        void PopulateOkpdDb(IUnitOfWork unitOfWork, string path);
        ICollection<OkpdHierarchy> OkpdFromXml(string path);
    }
}
