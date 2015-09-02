using Base.Document.Entities;
using Base.Service;

namespace Base.Document.Service
{
    public class LinkedDocumentService : BaseObjectService<LinkedDocument>, ILinkedDocumentService
    {
        public LinkedDocumentService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
