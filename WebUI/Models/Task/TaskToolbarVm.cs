using Base.Task.Entities;
using System.Collections.Generic;
using WebUI.Controllers;

namespace WebUI.Models.Task
{
    public class TaskToolbarVm : BaseViewModel
    {
        public TaskToolbarVm(IBaseController controller, Base.Task.Entities.Task task)
            : base(controller)
        {
            this.Actions = new List<TaskToolbarActionVm>();

            if (task != null)
            {
                this.TaskID = task.ID;
                this.CurrentStatus = task.Status;

                if (this.SecurityUser.ID == task.AssignedToID && (task.Status == TaskStatus.New || task.Status == TaskStatus.Viewed || task.Status == TaskStatus.Rework || task.Status == TaskStatus.InProcess))
                {
                    switch (task.Status)
                    {
                        case TaskStatus.New:
                        case TaskStatus.Viewed:
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.InProcess.ToString(), Title = "В работу", Text = "Установить статус напоминания 'В работе'", СommentIsRequired = false });
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.Refinement.ToString(), Title = "Задать вопрос", Text = "Установить статус напоминания 'Уточнение'", СommentIsRequired = true });

                            break;

                        case TaskStatus.Rework:
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.Revise.ToString(), Title = "Выполнено", Text = "Установить статус напоминания 'Проверка'", СommentIsRequired = true });
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.Refinement.ToString(), Title = "Задать вопрос", Text = "Установить статус напоминания 'Уточнение'", СommentIsRequired = true });

                            break;

                        case TaskStatus.InProcess:
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.Revise.ToString(), Title = "Выполнено", Text = "Установить статус напоминания 'Проверка'", СommentIsRequired = true });
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.Refinement.ToString(), Title = "Задать вопрос", Text = "Установить статус напоминания 'Уточнение'", СommentIsRequired = true });

                            break;
                    }

                }
                else if (this.SecurityUser.ID == task.AssignedFromID)
                {
                    switch (task.Status)
                    {
                        case TaskStatus.New:
                        case TaskStatus.Viewed:
                        case TaskStatus.Rework:
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.Redirection.ToString(), Title = "Переадресовать", Text = "Установить статус напоминания 'Переадресация'", СommentIsRequired = true });
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.NotRelevant.ToString(), Title = "В архив", Text = "Установить статус напоминания 'Неактуальна'", СommentIsRequired = true });
                            break;

                        case TaskStatus.Redirection:
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.New.ToString(), Title = "В работу", Text = "Установить статус напоминания 'Новая'", СommentIsRequired = true });
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.NotRelevant.ToString(), Title = "В архив", Text = "Установить статус напоминания 'Неактуальна'", СommentIsRequired = true });
                            break;

                        case TaskStatus.Refinement:
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.InProcess.ToString(), Title = "Ответить", Text = "Установить статус напоминания 'В работе'", СommentIsRequired = true });
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.Redirection.ToString(), Title = "Переадресовать", Text = "Установить статус напоминания 'Переадресация'", СommentIsRequired = true });
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.NotRelevant.ToString(), Title = "В архив", Text = "Установить статус напоминания 'Неактуальна'", СommentIsRequired = true });

                            break;

                        case TaskStatus.Revise:
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.Rework.ToString(), Title = "На доработку", Text = "Установить статус напоминания 'Доработка'", СommentIsRequired = true });
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.Complete.ToString(), Title = "Завершить", Text = "Установить статус напоминания 'Завершена'", СommentIsRequired = false });
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.Redirection.ToString(), Title = "Переадресовать", Text = "Установить статус напоминания 'Переадресация'", СommentIsRequired = true });
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.NotRelevant.ToString(), Title = "В архив", Text = "Установить статус напоминания 'Неактуальна'", СommentIsRequired = true });

                            break;

                        case TaskStatus.Complete:
                        case TaskStatus.NotRelevant:
                            this.Actions.Add(new TaskToolbarActionVm() { Value = TaskStatus.Rework.ToString(), Title = "На доработку", Text = "Установить статус напоминания 'Доработка'", СommentIsRequired = true });

                            break;
                    }
                }
            }

        }
        public int TaskID { get; protected set; }
        public TaskStatus CurrentStatus { get; protected set; }
        public List<TaskToolbarActionVm> Actions { get; protected set; }
    }

    public class TaskToolbarActionVm
    {
        public string Value { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool СommentIsRequired { get; set; }
    }
}