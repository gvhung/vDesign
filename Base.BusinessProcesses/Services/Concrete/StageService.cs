using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Service;
using Base.Task.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class StageService : BaseObjectService<Stage>, IStageService
    {
        public StageService(IBaseObjectServiceFacade facade) : base(facade) { }

        protected override void BeforeCreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject srcObj, BaseObject destObj)
        {
            if (srcObj.GetType().BaseType == typeof(StageTemplate) && destObj.GetType() == typeof(Stage))
            {
                StageTemplate template = srcObj as StageTemplate;

                if(template == null)
                    throw new Exception("Не удалось создать на основании шаблона");

                Stage stage = destObj as Stage;

                if(stage == null)
                    throw new Exception("Объект не является этапом");
                

                stage.TitleTemplate = template.TitleTemplate;

                stage.Outputs = template.Actions != null
                    ? template.Actions.Select(x => new StageAction(x)).ToList()
                    : new List<StageAction>();

                stage.CategoryID = template.CategoryID;

                stage.DescriptionTemplate = template.DescriptionTemplate;

                stage.Color = template.Color;

                stage.CreateTask = template.CreateTask;

                stage.Description = template.Description;

                stage.IsCustomPerformer = template.IsCustomPerformer;

                stage.PerformancePeriod = template.PerformancePeriod;

                stage.Title = template.Title;

                stage.ObjectType = template.ObjectType;

                stage.TaskCategory =
                    unitOfWork.GetRepository<TaskCategory>().All().FirstOrDefault(x => x.ID == template.CategoryID);
            }

            base.BeforeCreateOnGroundsOf(unitOfWork, srcObj, destObj);
        }
    }
}
