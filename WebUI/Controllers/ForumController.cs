using Base.Forum.Entities;
using Base.Forum.Service;
using Base.Security;
using Framework;
using System;
using System.Linq;
using System.Web.Mvc;
using WebUI.Models.Forum;

namespace WebUI.Controllers
{
    public class ForumController : BaseController
    {
        private readonly IForumSectionService _sectionService;
        private readonly IForumTopicService _topicService;
        private readonly IForumPostService _postService;

        const int ITEMS_PER_PAGE = 20;

        public ForumController(IBaseControllerServiceFacade baseServiceFacade, IForumSectionService sectionService, IForumTopicService topicService, 
                               IForumPostService postService)
            : base(baseServiceFacade)
        {
            _sectionService = sectionService;
            _topicService = topicService;
            _postService = postService;
        }

        public ViewResult Index(int? sect = null, int? topic = null, int page = 1)
        {
            using(var uow = CreateUnitOfWork())
            {
                ForumViewModel model = new ForumViewModel(this);

                if (this.SecurityUser.IsPermission(typeof(ForumTopic), TypePermission.Delete))
                {
                    model.UserRole = UserRole.Moderator;
                }
                else if (this.SecurityUser.IsPermission(typeof(ForumTopic), TypePermission.Create))
                {
                    model.UserRole = UserRole.User;
                }
                else
                {
                    model.UserRole = UserRole.Guest;
                }


                if (sect == null)
                {
                    model.Type = ForumVMType.Root;
                    model.Title = "Разделы";
                    model.Items = _sectionService.GetAll(uow).ToViewModel();
                }
                else if (topic == null)
                {
                    model.Type = ForumVMType.Section;
                    model.Title = _sectionService.Get(uow, sect.Value).Title;
                    model.Items = _topicService.GetAll(uow).Where(x => x.SectionID == sect.Value).ToViewModel();
                    model.SectionID = sect;
                }
                else
                {
                    model.Type = ForumVMType.Topic;
                    model.Title = _topicService.GetForViewing(uow, topic.Value).Title;
                    model.Items = _postService.GetAll(uow).Where(x => x.TopicID == topic.Value).ToViewModel();
                    model.SectionID = sect;
                    model.TopicID = topic;
                }

                if (model.Type != ForumVMType.Root)
                {
                    model.CurrentPage = page;
                    model.PageCount = (int)Math.Ceiling((double)model.Items.Count() / ITEMS_PER_PAGE);
                    model.Items = model.Items.Skip((model.CurrentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE);
                }

                return View("Index", model);
            }
        }

        [HttpPost]
        public JsonNetResult CreateSection(string title, string description)
        {
            using (var uow = CreateUnitOfWork())
            {
                try
                {
                    ForumSection section = _sectionService.Create(uow, title, description);
                    return new JsonNetResult(new {success = Url.Action("Index", new {sect = section.ID})});
                }
                catch (Exception e)
                {
                    return new JsonNetResult(new {error = e.Message});
                }
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonNetResult CreateTopic(int sectionID, string title, string description, string message)
        {
            using (var uow = CreateUnitOfWork())
            {
                try
                {
                    ForumTopic topic = _topicService.Create(uow, sectionID, title, description, message);
                    return
                        new JsonNetResult(new {success = Url.Action("Index", new {sect = sectionID, topic = topic.ID})});
                }
                catch (Exception e)
                {
                    return new JsonNetResult(new {error = e.Message});
                }
            }
        }

        public JsonNetResult PublishTopic(int id)
        {
            using (var uow = CreateUnitOfWork())
            {
                try
                {
                    ForumTopic topic = _topicService.Publish(uow, id);
                    return new JsonNetResult(new {success = "OK"});
                }
                catch (Exception e)
                {
                    return new JsonNetResult(new {error = e.Message});
                }
            }
        }

        public JsonNetResult DeleteTopic(int id)
        {
            using (var uow = CreateUnitOfWork())
            {
                try
                {
                    ForumTopic topic = _topicService.Get(uow, id);
                    _topicService.Delete(uow, topic);

                    return new JsonNetResult(new {success = "OK"});
                }
                catch (Exception e)
                {
                    return new JsonNetResult(new {error = e.Message});
                }
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonNetResult CreatePost(int topicID, string message)
        {
            using (var uow = CreateUnitOfWork())
            {
                try
                {
                    ForumPost post = _postService.Create(uow, topicID, message);
                    return new JsonNetResult(new
                    {
                        success = Url.Action("Index", new
                        {
                            sect = _topicService.Get(uow, post.TopicID).SectionID,
                            topic = topicID,
                            page = _postService.CalcPostPage(uow, post.TopicID, ITEMS_PER_PAGE)
                        })
                    });
                }
                catch (Exception e)
                {
                    return new JsonNetResult(new {error = e.Message});
                }
            }
        }

        public JsonNetResult PublishPost(int id)
        {
            using (var uow = CreateUnitOfWork())
            {
                try
                {
                    ForumPost post = _postService.Publish(uow, id);
                    return new JsonNetResult(new {success = "OK"});
                }
                catch (Exception e)
                {
                    return new JsonNetResult(new {error = e.Message});
                }
            }
        }

        public JsonNetResult DeletePost(int id)
        {
            using (var uow = CreateUnitOfWork())
            {
                try
                {
                    ForumPost post = _postService.Get(uow, id);
                    _postService.Delete(uow, post);

                    return new JsonNetResult(new {success = "OK"});
                }
                catch (Exception e)
                {
                    return new JsonNetResult(new {error = e.Message});
                }
            }
        }

        public ActionResult Search(string searchStr, int? sect = null)
        {
            using (var uow = CreateUnitOfWork())
            {
                ForumViewModel model = new ForumViewModel(this) {Type = ForumVMType.Search, SectionID = sect};

                var sections = _sectionService.GetAll(uow);
                if (sect != null)
                {
                    sections = sections.Where(x => x.ID == sect.Value);
                }

                model.Items = sections.FullTextSearch(searchStr, this.CacheWrapper).ToList();

                return View("Index", model);
            }
        }
    }
}
