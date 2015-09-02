using Base.Document.Entities;
using Base.Service;

namespace Base.Document.Service
{
    public class DocumentTemplateCategoryService : BaseCategoryService<DocumentTemplateCategory>, IDocumentTemplateCategoryService
    {
        public DocumentTemplateCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
