using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.Security.Service.Abstract;
using System;
using System.Linq;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class NewController : BaseController
    {
        private readonly IWorkflowService _workflowService;
        private readonly ISecurityService _securityService;
        private readonly IStageUserService _stageUserService;
        private readonly IBPTaskService _taskService;

        public NewController(
            IBaseControllerServiceFacade baseServiceFacade,
            IWorkflowService workflowService,
            ISecurityService securityService,
            IStageUserService stageUserService,
            IBPTaskService taskService): base(baseServiceFacade)
        {
            _workflowService = workflowService;
            _securityService = securityService;
            _stageUserService = stageUserService;
            _taskService = taskService;
        }

        public ActionResult Test()
        {
            using (var uow = CreateUnitOfWork())
            {
                return Content(_securityService.GetAccessType(uow,
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .FirstOrDefault(x => x.FullName == "Data.Entities.TestObject"),
                    15).ToString());
            }
        }

        public ActionResult ReadOnly()
        {
            _securityService.MakeReadOnly(AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(x => x.GetTypes())
                     .FirstOrDefault(x => x.FullName == "Data.Entities.TestObject"),
                 15);

            return this.Content("ok");
        }

        public ActionResult Restore()
        {
            _securityService.RestoreAccess(AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(x => x.GetTypes())
                     .FirstOrDefault(x => x.FullName == "Data.Entities.TestObject"),
                 15);

            return this.Content("ok");
        }

        public ActionResult MakeBpTasksAuto()
        {
            using (var uow = CreateSystemUnitOfWork())
            {
                var repo = uow.GetRepository<BPTask>();

                foreach (var task in repo.All())
                    task.Auto = true;

                uow.SaveChanges();

                return Content("ok");
            }
        }
    }
}
