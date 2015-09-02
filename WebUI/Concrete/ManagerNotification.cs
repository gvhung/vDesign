using Base;
using Base.DAL;
using Base.Entities.Complex;
using Base.Notification.Entities;
using Base.Notification.Service;
using Base.Security;
using Base.Security.Service;
using Base.Task.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Ambient;
using Framework.Extensions;
using Base.BusinessProcesses.Services.Abstract;
using Base.Task.Services;
using Base.Events;
using Base.Notification.Service.Abstract;
using Base.Settings;
using Data.Entities;
using Data.Service.Abstract;
using Framework;
using Framework.Maybe;
using Base.Service;
using Castle.Core.Internal;

namespace WebUI.Concrete
{
    public class ManagerNotification : IManagerNotification
    {
        public static string Hostname { get; set; }

        private readonly INotificationService _notificationService;
        private readonly ITaskService _taskService;
        private readonly IEmailService _emailService;
        private readonly IEmailSettingsService _emailSettingsService;
        private readonly IBroadcaster _broadcaster;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public ManagerNotification(INotificationService notificationService, ITaskService taskService,
                                   IEmailService emailService, IEmailSettingsService emailSettingsService, IBroadcaster broadcaster, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _notificationService = notificationService;
            _taskService = taskService;
            _emailService = emailService;
            _emailSettingsService = emailSettingsService;
            _broadcaster = broadcaster;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public void CreateNotice(IUnitOfWork unitOfWork, BaseObject obj, BaseEntityState state)
        {
            var notifications = new List<Notification>();

            CreateNoticeEx(unitOfWork, obj, state, notifications);

            if (!notifications.Any()) return;

            _notificationService.UpdateCollection(unitOfWork, notifications);

            if (_emailSettingsService.EnableEmail)
            {
                //TODO: [scheduler]
                //BackgroundJob.Schedule<ManagerNotification>(
                //    x => x.SendNoticesToMail(notifications.Select(z => z.ID), Hostname),
                //    TimeSpan.FromMinutes(_emailSettingsService.SendDelay));
            }
        }

        public void CreateNotices(IUnitOfWork unitOfWork, Dictionary<BaseObject, BaseEntityState> objects)
        {
            var notifications = new List<Notification>();

            foreach (var key in objects.Keys)
            {
                CreateNoticeEx(unitOfWork, key, objects[key], notifications);
            }

            if (!notifications.Any()) return;

            _notificationService.UpdateCollection(unitOfWork, notifications);

            if (_emailSettingsService.EnableEmail)
            {
                //TODO: [scheduler]
                //BackgroundJob.Schedule<ManagerNotification>(
                //    x => x.SendNoticesToMail(notifications.Select(z => z.ID), Hostname),
                //    TimeSpan.FromMinutes(_emailSettingsService.SendDelay));
            }
        }

        private void CreateNoticeEx(IUnitOfWork unitOfWork, BaseObject obj, BaseEntityState state, ICollection<Notification> updateCollection)
        {
            if (obj is Task)
            {
                CreateTaskNotice(unitOfWork, obj, state, updateCollection);
            }
            else if (obj is UserPromotion)
            {
                CreatePromotionNotice(obj, state, updateCollection);
            }
            else if (obj is SupportQA)
            {
                CreateSupportQANotice(obj, state, updateCollection);
            }
        }



        private void CreatePromotionNotice(BaseObject obj, BaseEntityState state, ICollection<Notification> updateCollection)
        {
            var promotion = obj as UserPromotion;

            if (promotion != null && String.IsNullOrEmpty(promotion.User.Email)) return;

            var linkBaseObject = new LinkBaseObject()
            {
                ID = promotion.ID,
                FullName = typeof(UserPromotion).FullName,
                Assembly = typeof(UserPromotion).Assembly.FullName,
                Mnemonic = "UserPromotion"
            };

            var notification = new Notification()
            {
                sys_name = "Updated",
                Date = DateTime.Now,
                UserID = promotion.UserID,
                UserEmail = promotion.User.Email,
                Entity = linkBaseObject,
                Title = String.Format("Заявка {0}", promotion.State.GetDescription().ToLower()),
                Description = String.Format("Ваша заявка на изменение типа учетной записи пользователя на \"{0}\" была {1}.",
                                            promotion.RequestedType.GetDescription(), promotion.State.GetDescription().ToLower())
            };

            updateCollection.Add(notification);
        }

        private void CreateSupportQANotice(BaseObject obj, BaseEntityState state, ICollection<Notification> updateCollection)
        {
            var supportQa = obj as SupportQA;

            if (supportQa != null && String.IsNullOrEmpty(supportQa.SenderEmail)) return ;

            var linkBaseObject = new LinkBaseObject()
            {
                ID = supportQa.ID,
                FullName = typeof(SupportQA).FullName,
                Assembly = typeof(SupportQA).Assembly.FullName,
                Mnemonic = "SupportQA"
            };

            var notification = new Notification()
            {
                sys_name = "Updated",
                Date = DateTime.Now,
                UserID = supportQa.SenderID,
                UserEmail = supportQa.Sender.Email,
                Entity = linkBaseObject,
                Title = String.Format("Обратная связь"),
                Description = supportQa.AnswerMessage
            };

            updateCollection.Add(notification);
        }

        private void CreateTaskNotice(IUnitOfWork unitOfWork, BaseObject obj, BaseEntityState state, ICollection<Notification> updateCollection)
        {
            var task = unitOfWork.GetRepository<Task>().Find(obj.ID);

            var assignedFrom = unitOfWork.GetRepository<User>().Find(task.AssignedFromID);

            var linkBaseObject = new LinkBaseObject()
            {
                ID = task.ID,
                FullName = typeof(Task).FullName,
                Assembly = typeof(Task).Assembly.FullName,
                Mnemonic = "Task"
            };

            string typeName = linkBaseObject.FullName;

            Notification notification = null;

            switch (state)
            {
                case BaseEntityState.Added:
                    if (task.AssignedToID != null && task.AssignedFromID != null)
                    {
                        linkBaseObject.Mnemonic = "InTask";

                        notification = new Notification()
                        {
                            sys_name = "Added",
                            Date = DateTime.Now,
                            UserID = task.AssignedToID,
                            UserEmail = task.AssignedTo.Email,
                            Entity = linkBaseObject,
                            Title = String.Format("Новое напоминание от {0}", assignedFrom.FullName),
                            Description = task.Title.TruncateAtWord(100)
                        };

                        updateCollection.Add(notification);
                    }

                    break;

                case BaseEntityState.Modified:
                    if (task.AssignedToID != null && task.AssignedFromID != null)
                    {
                        foreach (var notice in _notificationService.GetAll(unitOfWork, null).Where(x => x.Entity.ID == task.ID && x.Entity.FullName == typeName))
                        {
                            notice.Status = NotificationStatus.Viewed;
                            updateCollection.Add(notice);
                        }

                        string sys_name = null;
                        string title = null;
                        string description = null;

                        int? userID = null;
                        string userEmail = null;

                        switch (task.Status)
                        {
                            case TaskStatus.InProcess:
                                if (task.TaskChangeHistory != null)
                                {
                                    var itemHistory = task.TaskChangeHistory.LastOrDefault(x => x.Status != TaskStatus.InProcess);

                                    if (itemHistory != null)
                                    {
                                        userID = task.AssignedToID;
                                        if (userID == AppContext.SecurityUser.ID) break;

                                        userEmail = task.AssignedTo.Email;
                                        linkBaseObject.Mnemonic = "InTask";
                                        description = task.Title.TruncateAtWord(100);

                                        if (itemHistory.Status == TaskStatus.Refinement)
                                        {
                                            sys_name = "Refinement->InProcess";
                                            title = String.Format("{0} ответил(а)", assignedFrom.FullName);
                                        }
                                        else
                                        {
                                            sys_name = "NotRelevant->InProcess";
                                            title = String.Format("Новое напоминание от {0}", assignedFrom.FullName);
                                        }
                                    }
                                }

                                break;

                            case TaskStatus.New:
                                if (task.TaskChangeHistory != null)
                                {
                                    var itemHistory = task.TaskChangeHistory.LastOrDefault(x => x.Status != TaskStatus.InProcess);

                                    if (itemHistory != null)
                                    {
                                        userID = task.AssignedToID;

                                        if (userID == AppContext.SecurityUser.ID) break;

                                        userEmail = task.AssignedTo.Email;
                                        sys_name = "->New";
                                        linkBaseObject.Mnemonic = "InTask";
                                        title = String.Format("Новое напоминание от {0}", assignedFrom.FullName);
                                        description = task.Title.TruncateAtWord(100);
                                    }
                                }

                                break;

                            case TaskStatus.Refinement:
                                userID = task.AssignedFromID;
                                if (userID == AppContext.SecurityUser.ID) break;

                                userEmail = assignedFrom.Email;
                                sys_name = "Refinement";
                                linkBaseObject.Mnemonic = "OutTask";
                                title = String.Format("{0} задал(а) вопрос", task.AssignedTo.FullName);
                                description = task.LastComment.TruncateAtWord(100);

                                break;

                            case TaskStatus.Revise:
                                userID = task.AssignedFromID;
                                if (userID == AppContext.SecurityUser.ID) break;

                                userEmail = assignedFrom.Email;
                                sys_name = "Revise";
                                linkBaseObject.Mnemonic = "OutTask";
                                title = String.Format("{0} просит проверить", task.AssignedTo.FullName);
                                description = task.Title.TruncateAtWord(100);

                                break;

                            case TaskStatus.Rework:
                                userID = task.AssignedToID;
                                if (userID == AppContext.SecurityUser.ID) break;

                                userEmail = task.AssignedTo.Email;
                                sys_name = "Rework";
                                linkBaseObject.Mnemonic = "InTask";
                                title = String.Format("{0} просит уточнить", assignedFrom.FullName);
                                description = task.LastComment.TruncateAtWord(100);

                                break;

                            case TaskStatus.NotRelevant:

                                break;
                        }

                        if (sys_name != null)
                        {
                            if (!_notificationService.GetAll(unitOfWork)
                                .Any(x => x.UserID == userID && x.Entity.ID == task.ID && x.Entity.FullName == typeName &&
                                          x.sys_name == sys_name && x.Status == NotificationStatus.New))
                            {
                                linkBaseObject.Mnemonic = "InTask";

                                notification = new Notification()
                                {
                                    sys_name = sys_name,
                                    Date = DateTime.Now,
                                    UserID = userID,
                                    UserEmail = userEmail,
                                    Entity = linkBaseObject,
                                    Title = title,
                                    Description = description
                                };

                                updateCollection.Add(notification);
                            }
                        }
                    }

                    break;
            }
        }

        public async void SendNoticesToMail(IEnumerable<int> notificationIDs, string hostname)
        {
            if (notificationIDs == null) return;

            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                var notifications = _notificationService.GetAll(uofw)
                    .Where(
                        x =>
                            notificationIDs.Contains(x.ID) && x.Status == NotificationStatus.New &&
                            !String.IsNullOrEmpty(x.UserEmail)).ToList();

                foreach (var notice in notifications)
                {
                    string absUrl = String.Format(
                        "{0}/Standart/GetViewModel?mnemonic=Notification&typeDialog=0&id={1}", hostname, notice.ID);
                    string body = String.Format(@"{0}:<br>{1}", notice.Title, notice.Description);

                    if (!String.IsNullOrEmpty(hostname))
                    {
                        body += String.Format(@"<br><br><a href='{0}'>Подробнее...</a>", absUrl);
                    }

                    _emailService.SendMail(notice.UserEmail, notice.Title, body, true);
                }
            }
        }

        public void CreateNotice(BaseObjectEventArgs e)
        {
            if (e.Object is Task)
            {
                switch (e.Type)
                {
                    case TypeEvent.OnCreate:
                        this.CreateNotice(e.UnitOfWork, e.Object, BaseEntityState.Added);

                        break;

                    case TypeEvent.OnGet:

                        var task = e.UnitOfWork.GetRepository<Task>().Find(e.Object.ID);

                        var linkBaseObject = new LinkBaseObject()
                        {
                            ID = task.ID,
                            FullName = typeof(Task).FullName,
                            Assembly = typeof(Task).Assembly.FullName,
                            Mnemonic = "Task"
                        };

                        string typeName = linkBaseObject.FullName;

                        var notifications = new List<Notification>();

                        foreach (var notice in _notificationService.GetAll(e.UnitOfWork, hidden: null)
                            .Where(x => x.UserID == AppContext.SecurityUser.ID && x.Entity.ID == task.ID && x.Entity.FullName == typeName && x.Status == NotificationStatus.New))
                        {
                            notice.Status = NotificationStatus.Viewed;
                            notifications.Add(notice);
                        }

                        _notificationService.UpdateCollection(e.UnitOfWork, notifications);

                        break;

                    case TypeEvent.OnUpdate:
                        this.CreateNotice(e.UnitOfWork, e.Object, BaseEntityState.Modified);

                        break;
                }
            }
        }

    }
}