using Base.BusinessProcesses.Services.Concrete;
using Base.BusinessProcesses.Strategies;
using Base.DAL;
using Base.Service;
using Base.UI;
using Framework.Maybe;
using Ninject;
using System;
using System.Linq;

namespace WebUI.Concrete
{
    public class NinjectBaseObjectServiceResolver : IWorkflowServiceResolver
    {
        private readonly IKernel _kernel;
        private readonly IViewModelConfigService _configService;

        public NinjectBaseObjectServiceResolver(IKernel kernel, IViewModelConfigService configService)
        {
            _kernel = kernel;
            _configService = configService;
        }

        public IBaseObjectCRUDService GetObjectService(string objectTypeStr, IUnitOfWork unitOfWork = null)
        {
            var serviceType = _configService.GetAll().FirstOrDefault(x => x.TypeEntity.FullName == objectTypeStr).With(x => x.TypeService);

            if (serviceType == null)
                throw new Exception("Type not founded");

            return this.GetServiceImpl(serviceType, unitOfWork);
        }

        public IBaseObjectCRUDService GetObjectService(Type type, IUnitOfWork unitOfWork)
        {
            if (type == null)
                throw new Exception("Type not founded");

            var serviceType = _configService.GetAll().FirstOrDefault(x => x.TypeEntity == type).With(x => x.TypeService);

            return GetServiceImpl(serviceType, unitOfWork);
        }

        public IWorkflowStrategy GetStrategy(Type type)
        {
            return _kernel.Get(type) as IWorkflowStrategy;
        }

        public WorkflowCommonStrategyModule GetStrategyModule(Type type)
        {
            return _kernel.Get(type) as WorkflowCommonStrategyModule;
        }

        private IBaseObjectCRUDService GetServiceImpl(Type serviceType, IUnitOfWork unitOfWork)
        {

            var service = _kernel.Get(serviceType) as IBaseObjectCRUDService;

            //if (unitOfWork != null && unitOfWork.GetType() == service.UnitOfWork.GetType())
            //    service.UnitOfWork = unitOfWork;

            return service;
        }
    }
}