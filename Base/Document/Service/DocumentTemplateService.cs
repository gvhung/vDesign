using Base.DAL;
using Base.Document.Entities;
using Base.Service;
using System.Linq;

namespace Base.Document.Service
{
    public class DocumentTemplateService : BaseCategorizedItemService<DocumentTemplate>, IDocumentTemplateService
    {
        public DocumentTemplateService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override IQueryable<DocumentTemplate> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);

            return this.GetAll(unitOfWork, hidden).Where(a => (a.Category_.sys_all_parents != null && a.Category_.sys_all_parents.Contains(strID)) || a.Category_.ID == categoryID);
        }
    }
}
