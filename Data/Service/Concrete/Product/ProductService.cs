using Base;
using Base.BusinessProcesses.Entities;
using Base.DAL;
using Base.Service;
using Data.Service.Abstract.Product;

namespace Data.Service.Concrete.Product
{
    public class ProductService : BaseObjectService<Entities.Product.Product>, IProductService
    {
        public ProductService(IBaseObjectServiceFacade facade)
            : base(facade)
        {

        }

        protected override IObjectSaver<Entities.Product.Product> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Entities.Product.Product> objectSaver)
        {
            var temp = base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.FillerClass)
                .SaveOneObject(x => x.FillerType)
                .SaveOneObject(x => x.MatrixSubType)
                .SaveOneObject(x => x.MatrixType)
                .SaveOneObject(x => x.SoftwareType)
                .SaveOneObject(x => x.Tech)
                .SaveManyToMany(x => x.ApplicationAreas)
                .SaveOneToMany(x => x.ProductConformityDocuments)
                .SaveManyToMany(x => x.Additives)
                .SaveManyToMany(x => x.Equipments)
                .SaveOneObject(x => x.ProductionStage)
                .SaveOneObject(x => x.Manufacturing)
                .SaveOneToMany(x => x.Files);
            return temp;
        }

        public void BeforeInvoke(BaseObject obj)
        {

        }

        public void OnActionExecuting(ActionExecuteArgs args)
        {

        }

        public int GetWorkflowID(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return Workflow.Default;
        }
    }
}
