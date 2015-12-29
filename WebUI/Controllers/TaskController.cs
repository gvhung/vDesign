using Base.Ambient;
using Base.Task.Entities;
using Base.Task.Services;
using Framework;
using System;
using System.Linq;
using System.Web.Mvc;
using Framework.Wrappers;
using WebUI.Models.Task;
using SystemTask = System.Threading.Tasks;

namespace WebUI.Controllers
{
    public class TaskController : BaseController
    {
        private readonly ITaskService _taskService;
        private readonly ICacheWrapper _cacheWrapper;

        public TaskController(IBaseControllerServiceFacade baseServiceFacade, ITaskService taskService, ICacheWrapper cacheWrapper)
            : base(baseServiceFacade)
        {
            _taskService = taskService;
            _cacheWrapper = cacheWrapper;
        }

        public ActionResult Toolbar(int? taskID)
        {
            Task task = null;

            using (var uofw = CreateUnitOfWork())
            {
                if (taskID.HasValue && taskID != 0)
                    task = _taskService.Get(uofw, (int)taskID);
            }

            return PartialView("_Toolbar", new TaskToolbarVm(this, task));
        }

        [HttpPost]
        public JsonNetResult ExecuteAction(int taskID, string actionID, string comment)
        {
            if (taskID == 0)
                return new JsonNetResult(new { error = 1, message = "Некорректный параметр" });

            var intError = 0;
            var strMsg = "";

            using (var uofw = CreateUnitOfWork())
            {
                var task = _taskService.Get(uofw, taskID);

                if (task == null)
                {
                    intError = 2;
                    strMsg = "Напоминание не найдена";
                }
                else
                {
                    var toolbar = new TaskToolbarVm(this, task);

                    var action = toolbar.Actions.FirstOrDefault(x => x.Value == actionID);

                    if (action == null)
                    {
                        intError = 3;
                        strMsg = "Попытка выполнить недопустимое действие";
                    }
                    else if (action.СommentIsRequired && String.IsNullOrEmpty(comment))
                    {
                        intError = 4;
                        strMsg = "Введите комментарий";
                    }
                    else
                    {
                        try
                        {
                            var newStatus = (TaskStatus)Enum.Parse(typeof(TaskStatus), actionID);

                            task.Status = newStatus;

                            _taskService.AddItemToChangeHistory(task, comment);

                            _taskService.Update(uofw, task);
                        }
                        catch (Exception e)
                        {
                            intError = 1;
                            strMsg = e.ToStringWithInner();
                        }
                    }

                }
            }


            return new JsonNetResult(new { error = intError, message = strMsg });
        }

        public ActionResult GetMyTasks(int maxcount = -1)
        {
            const string key = "{4099E458-FEA8-49DD-BD2B-233F3E38D891}";
            object result;


            if (_taskService.HasUserCache(AppContext.SecurityUser.ID, key))
            {
                result = _taskService.GetUserCache(AppContext.SecurityUser.ID, key);
            }
            else
            {
                using (var uofw = CreateUnitOfWork())
                {
                    var res = _taskService.GetAll(uofw)
                        .Where(x => x.AssignedToID == AppContext.SecurityUser.ID)
                        .Where(x => x.Status == TaskStatus.New || x.Status == TaskStatus.Viewed || x.Status == TaskStatus.InProcess || x.Status == TaskStatus.Rework)
                        .OrderByDescending(x => x.Period.Start)
                        .AsQueryable();

                    if (maxcount > 0)
                        res = res.Take(maxcount);

                    result = new JsonNetResult(res.ToList(), GetListBoContractResolver("Task"));
                }

                _taskService.GetUserCache(AppContext.SecurityUser.ID, key, result);
            }


            return (JsonNetResult)result;
        }
    }
}
