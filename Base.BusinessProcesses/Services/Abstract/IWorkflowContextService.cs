using System.Collections.Generic;
using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Concrete;
using Base.DAL;
using Base.Security;
using Base.Service;

namespace Base.BusinessProcesses.Services.Abstract
{
    public interface IWorkflowContextService : IBaseObjectService<WorkflowContext>
    {
        /// <summary>
        /// Получить текущие этапы
        /// </summary>
        /// <param name="uow">Unit of work</param>
        /// <param name="obj">Объект</param>
        /// <returns></returns>
        List<StagePerform> GetCurrentStages(IUnitOfWork uow, IBPObject obj);
        /// <summary>
        /// Получить пользователей
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="ctxItem">Этап</param>
        /// <param name="obj">Объект</param>
        /// <returns></returns>
        List<User> GetPermittedUsers(IUnitOfWork uow, Stage ctxItem, BaseObject obj);
        /// <summary>
        /// Возвращает тип доступа
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="perform">Этап</param>
        /// <param name="obj">Объект</param>
        /// <returns></returns>
        PerformerType GetPerformerType(IUnitOfWork unitOfWork, StagePerform perform, BaseObject obj);
    }
}
