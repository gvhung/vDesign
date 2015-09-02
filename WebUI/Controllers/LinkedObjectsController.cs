using Base;
using Base.Document.Entities;
using Base.LinkedObjects.Entities;
using Base.LinkedObjects.Service;
using Base.Service;
using Framework;
using System;
using System.Linq;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class LinkedObjectsController : BaseController
    {
        private readonly ILinkedObjectsService _linkedObjectsService;

        public LinkedObjectsController(IBaseControllerServiceFacade baseServiceFacade, ILinkedObjectsService linkedObjectsService)
            : base(baseServiceFacade)
        {
            _linkedObjectsService = linkedObjectsService;
        }

        public ActionResult Get(string mnemonic, int id)
        {
            try
            {
                using (var uofw = this.CreateUnitOfWork())
                {
                    //TODO: пенести всю логику в сервис
                    var serv = this.GetService<IBaseObjectCRUDService>(mnemonic);

                    var obj = serv.Get(uofw, id);

                    var item = _linkedObjectsService.GetListLinkedObjects(uofw, obj);

                    item.Obj.Mnemonic = mnemonic;

                    var links = item.Links.GroupBy(x => x.Obj.FullName).Select(x => new
                    {
                        FullName = x.Key,
                        IDs = x.Select(i => i.Obj.ID).ToArray()
                    });

                    foreach (var link in links)
                    {
                        var servTemp = this.GetService<IBaseObjectCRUDService>(link.FullName);

                        if (servTemp != null)
                        {
                            var config = this.DefaultViewModelConfig(link.FullName);

                            if (config != null)
                            {
                                string propertyName = config.LookupProperty;

                                var q =
                                    servTemp.GetAll(uofw, hidden: null)
                                        .Where(x => link.IDs.Contains(x.ID))
                                        .ToList();

                                foreach (int objID in link.IDs)
                                {
                                    var linkObj = q.FirstOrDefault(x => x.ID == objID);

                                    var resLink = item.Links.FirstOrDefault(x => x.Obj.ID == objID);

                                    if (resLink != null)
                                    {
                                        if (linkObj != null)
                                            resLink.Description =
                                                config.TypeEntity.GetProperty(propertyName).GetValue(linkObj).ToString();
                                        else
                                            item.Links.Remove(resLink);
                                    }
                                }
                            }
                        }

                    }

                    return JsonNet(new { model = item });
                }
            }
            catch (Exception e)
            {
                return JsonNet(new { error = e.ToStringWithInner() });
            }
        }

        public PartialViewResult GetToolbar()
        {
            return PartialView("_Toolbar", new BaseViewModel(this));
        }

        [HttpPost]
        public ActionResult CreateLinkedObject(string linkMnemonic, int linkID, string mnemonic, BaseObject obj)
        {
            try
            {
                using (var uofw = this.CreateTransactionUnitOfWork())
                {
                    //TODO: пенести всю логику в сервис
                    this.SecurityService.ThrowIfAccessDenied(uofw, typeof(ListLinkedОbjects),
                        Base.Security.TypePermission.Create);

                    var config = this.GetViewModelConfig(linkMnemonic);

                    this.SecurityService.ThrowIfAccessDenied(uofw, config.TypeEntity, linkID,
                        Base.Security.ObjectAccess.AccessType.Update);

                    var serv1 = this.GetService<IBaseObjectCRUDService>(linkMnemonic);

                    var obj1 = serv1.Get(uofw, linkID);

                    var serv2 = this.GetService<IBaseObjectCRUDService>(mnemonic);

                    var obj2 = obj.ID == 0 ? serv2.Create(uofw, obj) : serv2.Get(uofw, obj.ID);

                    _linkedObjectsService.AddLink(uofw, obj1, obj2);

                    uofw.Commit();

                    return JsonNet(new {model = new {ID = obj2.ID}});
                }
            }
            catch (Exception e)
            {
                return JsonNet(new { error = e.ToStringWithInner() });
            }
        }

        [HttpPost]
        public ActionResult DeleteLink(string mnemonic1, int id1, string mnemonic2, int id2)
        {
            try
            {   
                using (var uofw = this.CreateTransactionUnitOfWork())
                {
                    //TODO: пенести всю логику в сервис
                    var config1 = this.GetViewModelConfig(mnemonic1);

                    this.SecurityService.ThrowIfAccessDenied(uofw, config1.TypeEntity, id1, Base.Security.ObjectAccess.AccessType.Update);

                    var serv1 = this.GetService<IBaseObjectCRUDService>(mnemonic1);

                    var obj1 = serv1.Get(uofw, id1);

                    var serv2 = this.GetService<IBaseObjectCRUDService>(mnemonic2);

                    var obj2 = serv2.Get(uofw, id2);

                    var config2 = this.GetViewModelConfig(mnemonic2);

                    if (typeof (LinkedDocument).IsAssignableFrom(config2.TypeEntity))
                        serv2.Delete(uofw, obj2);
                    else
                        _linkedObjectsService.DeleteLink(uofw, obj1, obj2);

                    uofw.Commit();

                    return JsonNet(new {});
                }
            }
            catch (Exception e)
            {
                return JsonNet(new { error = e.ToStringWithInner() });
            }
        }
    }
}
