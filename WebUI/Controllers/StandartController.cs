using Base;
using Base.Entities.Complex;
using Base.QueryableExtensions;
using Base.Security.ObjectAccess;
using Base.Service;
using Base.UI;
using Framework;
using Framework.FullTextSearch;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Base.DAL;
using WebUI.Extensions.Kendo;
using WebUI.Helpers;
using WebUI.Models;
using WebUI.Authorize;

namespace WebUI.Controllers
{
    public class StandartController : BaseController
    {
        public StandartController(IBaseControllerServiceFacade baseServiceFacade)
            : base(baseServiceFacade)
        {
        }

        public ActionResult Index(string mnemonic, int? parentID, int? currentID, TypeDialog typeDialog = TypeDialog.Frame, string sysFilter = null)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("Index", new StandartDialogViewModel(this, mnemonic, typeDialog) { ParentID = parentID, CurrentID = currentID, SysFilter = sysFilter });
            }

            return View("Index", new StandartDialogViewModel(this, mnemonic, typeDialog) { ParentID = parentID, CurrentID = currentID, SysFilter = sysFilter });
        }

        [AllowGuest]
        public PartialViewResult GetDialog(string mnemonic, int? parentID, int? currentID, TypeDialog typeDialog = TypeDialog.Frame, string searchStr = null, string sysFilter = null)
        {
            return PartialView("Index", new StandartDialogViewModel(this, mnemonic, typeDialog) { ParentID = parentID, CurrentID = currentID, SearchStr = searchStr, SysFilter = sysFilter });
        }

        public PartialViewResult GetEditorTemplate(string name)
        {
            return PartialView(String.Format("~/Views/Shared/EditorTemplates/{0}.cshtml", name));
        }

        public PartialViewResult GetDisplayTemplate(string name)
        {
            return PartialView(String.Format("~/Views/Shared/DisplayTemplates/{0}.cshtml", name));
        }

        public ActionResult GetProperties(string mnemonic)
        {
            return new JsonNetResult(GetEditors(mnemonic).Select(x => new
            {
                PropertyName = x.PropertyName,
                Title = x.Title,
                Description = x.Description,
                TabName = x.TabName,
                Visible = x.Visible,
                Enable = !x.IsReadOnly,
            }));
        }

        public ActionResult GetEditorViewModel(string mnemonic, string member)
        {
            var editorVm = GetEditors(mnemonic).FirstOrDefault(x => x.PropertyName == member);

            if (editorVm != null)
                return PartialView("~/Views/Shared/EditorTemplates/EditorView.cshtml", editorVm);

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        public ActionResult GetViewModel(string mnemonic, TypeDialog typeDialog, int id = 0)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("_BuilderViewModel", new StandartDialogViewModel(this, mnemonic, typeDialog));
            }

            ViewBag.ID = id;
            ViewBag.AutoBind = true;

            return View("_BuilderViewModel", new StandartDialogViewModel(this, mnemonic, typeDialog));
        }

        [AllowGuest]
        public PartialViewResult GetPartialViewModel(string mnemonic, TypeDialog typeDialog, int id = 0, bool autoBind = false)
        {
            ViewBag.ID = id;
            ViewBag.AutoBind = autoBind;

            return PartialView("_BuilderViewModel", new StandartDialogViewModel(this, mnemonic, typeDialog));
        }

        public PartialViewResult GetAjaxForm(int id, string mnemonic, bool readOnly = false)
        {
            using (var uofw = CreateUnitOfWork())
            {
                var serv = GetService<IBaseObjectCRUDService>(mnemonic);

                var model = id != 0 ? serv.Get(uofw, id) : serv.CreateOnGroundsOf(uofw, null);

                var commonEditorViewModel = GetCommonEditor(uofw, mnemonic, model);

                string partialName = String.Format(readOnly ? "~/Views/Shared/DisplayTemplates/Common/{0}.cshtml" : "~/Views/Shared/EditorTemplates/Common/{0}.cshtml", "AjaxForm");

                return PartialView(partialName, new StandartFormModel(this)
                {
                    CommonEditorViewModel = commonEditorViewModel,
                    Model = model
                });
            }
        }

        [AllowGuest]
        [OutputCache(NoStore = true, Duration = 0)]
        public JsonNetResult Get(string mnemonic, int id)
        {
            using (var uofw = this.CreateUnitOfWork())
            {
                var serv = this.GetService<IBaseObjectCRUDService>(mnemonic);

                var model = serv.Get(uofw, id);

                return new JsonNetResult(new
                {
                    model = model,
                    access = GetObjAccess(uofw, mnemonic, model),
                }, GetBoContractResolver(mnemonic));
            }
        }

        private BaseObject _Save(IUnitOfWork uofw, string mnemonic, BaseObject model)
        {
            var bserv = this.GetService<IBaseObjectCRUDService>(mnemonic);

            var res = model.ID == 0 ? bserv.Create(uofw, model) : bserv.Update(uofw, model);

            return res;
        }

        [HttpPost]
        [AllowGuest]
        public ActionResult Save(string mnemonic, BaseObject model, bool returnEntireModel = true)
        {
            int intError = 0;
            string strMsg = "";

            object res = null;
            object access = null;

            try
            {
                using (var uofw = this.CreateTransactionUnitOfWork())
                {
                    var obj = this._Save(uofw, mnemonic, model);

                    uofw.Commit();

                    if (returnEntireModel)
                    {
                        res = obj;
                        access = this.GetObjAccess(uofw, mnemonic, obj);
                    }
                    else
                    {
                        res = new { ID = obj.ID };
                    }
                }

                strMsg = "Данные успешно сохранены!";
            }
            catch (Exception e)
            {
                intError = 1;
                strMsg = String.Format("Ошибка сохранения записи: {0}", e.ToStringWithInner());
            }

            return new JsonNetResult(new
            {
                error = intError,
                message = strMsg,
                model = res,
                access = access,
            }, GetBoContractResolver(mnemonic));
        }

        private dynamic GetObjAccess(IUnitOfWork uofw, string mnemonic, BaseObject obj)
        {
            var config = this.GetViewModelConfig(mnemonic);

            if (obj == null)
                return new
                {
                    Update = false,
                    Delete = false,
                };

            bool enable = true;

            if (typeof(IEnabledState).IsAssignableFrom(config.TypeEntity))
            {
                var enabledState = obj as IEnabledState;
                if (enabledState != null) enable = enabledState.IsEnabled(this.SecurityUser);
            }

            if (typeof(IAccessibleObject).IsAssignableFrom(config.TypeEntity))
            {
                var access = this.SecurityService.GetAccessType(uofw, config.TypeEntity, obj.ID);

                return new
                {
                    Update = access.HasFlag(AccessType.Update) && enable,
                    Delete = access.HasFlag(AccessType.Delete) && enable,
                };
            }
            else
            {
                return new
                {
                    Update = enable,
                    Delete = enable,
                };
            }
        }

        [HttpPost]
        public ActionResult KendoUI_Save([DataSourceRequest] DataSourceRequest request, string mnemonic, BaseObject model)
        {
            int intError = 0;
            string strMsg = "";

            if (ModelState.IsValid)
            {
                try
                {
                    using (var uofw = this.CreateTransactionUnitOfWork())
                    {
                        model = this._Save(uofw, mnemonic, model);

                        uofw.Commit();
                    }

                    strMsg = "Данные успешно сохранены!";
                }
                catch (Exception e)
                {
                    intError = 1;
                    strMsg = String.Format("Ошибка сохранения записи: {0}", e.ToStringWithInner());
                }
            }
            else
            {
                intError = 2;
                strMsg = "Не заполнены все обязательные поля";
            }

            if (intError == 0)
            {
                return new JsonNetResult(new[] { model }.ToDataSourceResult(request));
            }
            else
            {
                var res = new DataSourceResult()
                {
                    Errors = strMsg
                };

                return new JsonNetResult(res);
            }
        }

        private void _Delete(IUnitOfWork uofw, string mnemonic, int id)
        {
            if (id == 0) return;

            var serv = this.GetService<IBaseObjectCRUDService>(mnemonic);

            var obj = serv.Get(uofw, id);

            if (obj == null) return;

            serv.Delete(uofw, obj);
        }

        [HttpPost]
        public ActionResult Destroy(string mnemonic, int id)
        {
            int error = 0;
            string message = "";

            try
            {
                using (var uofw = this.CreateTransactionUnitOfWork())
                {
                    this._Delete(uofw, mnemonic, id);

                    uofw.Commit();
                }

                message = "Данные успешно удалены!";
            }
            catch (Exception e)
            {
                error = 1;
                message = String.Format("Ошибка удаления записи: {0}", e.ToStringWithInner());
            }

            return new JsonNetResult(new
            {
                error = error,
                message = message
            });
        }


        [HttpPost]
        public ActionResult KendoUI_Destroy([DataSourceRequest] DataSourceRequest request, string mnemonic, BaseObject model)
        {
            int error = 0;
            string message = "";

            try
            {
                using (var uofw = this.CreateTransactionUnitOfWork())
                {
                    this._Delete(uofw, mnemonic, model.ID);

                    uofw.Commit();
                }

                message = "Данные успешно удалены!";
            }
            catch (Exception e)
            {
                error = 1;
                message = String.Format("Ошибка удаления записи: {0}", e.ToStringWithInner());
            }

            if (error == 0)
            {
                return new JsonNetResult(new[] { model }.ToDataSourceResult(request));
            }
            else
            {
                var res = new DataSourceResult()
                {
                    Errors = message
                };

                return new JsonNetResult(res);
            }
        }

        [AllowGuest]
        public async Task<JsonNetResult> KendoUI_CollectionRead([DataSourceRequest] DataSourceRequest request, string mnemonic, int? categoryID, bool? allItems, string searchStr, string sysFilter)
        {
            var serv = this.GetService<IBaseObjectCRUDService>(mnemonic);

            try
            {
                using (var uofw = this.CreateUnitOfWork())
                {

                    IQueryable<BaseObject> q = null;

                    if (serv.GetType().GetInterfaces().Contains(typeof(ICategorizedItemCRUDService)))
                    {
                        if (allItems ?? false)
                        {
                            q = ((ICategorizedItemCRUDService)serv).GetAllCategorizedItems(uofw,
                                categoryID ?? 0);
                        }
                        else
                        {
                            q = ((ICategorizedItemCRUDService)serv).GetCategorizedItems(uofw,
                                categoryID ?? 0);
                        }
                    }
                    else
                    {
                        q = serv.GetAll(uofw);
                    }

                    var config = this.GetViewModelConfig(mnemonic);

                    q = q.SystemFilter(config, SecurityUser, sysFilter);

                    q = q.FullTextSearch(searchStr, this.CacheWrapper);

                    return new JsonNetResult(await q.ToDataSourceResultAsync(request, serv, config), GetListBoContractResolver(mnemonic));
                }
            }
            catch (Exception e)
            {
                var res = new DataSourceResult()
                {
                    Errors = e.Message
                };

                return new JsonNetResult(res);
            }
        }

        [AllowGuest]
        public JsonNetResult PropertyKendoUI_CollectionRead(string parentMnemonic, string mnemonic, string property, int objectID, bool full = false)
        {
            int error = 0;
            string message = "";

            var serv = this.GetService<IBaseObjectCRUDService>(parentMnemonic);

            IEnumerable resCollection = null;

            try
            {
                using (var uofw = this.CreateUnitOfWork())
                {
                    var model = serv.Get(uofw, objectID);

                    var collection = model.GetType().GetProperty(property).GetValue(model) as IEnumerable<BaseObject>;

                    if (!full)
                    {
                        var config = this.GetViewModelConfig(mnemonic);

                        string lookupProperty = config.LookupProperty;

                        var prop = config.TypeEntity.GetProperty(lookupProperty);

                        if (typeof(MultilanguageText).IsAssignableFrom(prop.PropertyType))
                        {
                            if (collection != null)
                                resCollection = collection.Select(m =>
                                {
                                    var record = new Dictionary<string, object>
                                    {
                                        {"ID", m.ID},
                                        {lookupProperty, (MultilanguageText) prop.GetValue(m)}
                                    };

                                    return record;
                                });
                        }
                        else
                        {
                            resCollection = collection.Select(collection != null && collection.GetType().GenericTypeArguments[0].IsAssignableToGenericType(
                                typeof(EasyCollectionEntry<>)) ? String.Format("new (ObjectID as ID, {0})", lookupProperty) : String.Format("new (ID, {0})", lookupProperty));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                error = 1;
                message = e.ToStringWithInner();
            }

            return new JsonNetResult(new
            {
                collection = resCollection,
                error = error,
                message = message
            });
        }

        [AllowGuest]
        public async Task<JsonNetResult> Filter_Read(string startswith, string mnemonic, string property, bool propertyIsBaseObject = true, string ids = null, string sysFilter = null)
        {
            using (var uofw = this.CreateSystemUnitOfWork())
            {
                var bserv = this.GetService<IBaseObjectCRUDService>(mnemonic);

                var q = bserv.GetAll(uofw).SystemFilter(GetViewModelConfig(mnemonic), SecurityUser, sysFilter);

                if (q == null) return null;

                startswith = startswith != null ? startswith.Trim() : null;

                IList qIDs = new List<BaseObject>();

                if (!String.IsNullOrEmpty(ids))
                {
                    var arrIDs = ids.Split(';').Select(Int32.Parse).ToArray();

                    qIDs = await bserv.GetAll(uofw, hidden: null).Where(x => arrIDs.Contains(x.ID)).ToListAsync();
                }

                if (!String.IsNullOrEmpty(startswith))
                {
                    if (propertyIsBaseObject && property == null)
                    {
                        var config = GetViewModelConfig(mnemonic);

                        property = config.LookupPropertyForFilter;
                    }

                    if (propertyIsBaseObject)
                    {
                        if (property == null)
                        {
                            var config = GetViewModelConfig(mnemonic);

                            property = config.LookupPropertyForFilter;
                        }

                        //bottleneck
                        q = q.Where(String.Format("it.{0}.Contains(\"{1}\")", property, startswith)).Take(50);

                        var list = await q.ToListAsync();

                        foreach (var obj in qIDs)
                        {
                            list.Add(obj);
                        }

                        return new JsonNetResult(list.Distinct(), GetListBoContractResolver(mnemonic));
                    }
                    else
                    {
                        //bottleneck 
                        var res =
                            await
                                q.Select("it." + property)
                                    .Where(String.Format("it.Contains(\"{0}\")", startswith))
                                    .Cast<string>()
                                    .Distinct()
                                    .Take(50)
                                    .ToGenericListAsync();

                        string pattern = String.Format(startswith.Length == 1 ? @"\b{0}\S*" : @"\S*{0}\S*", startswith);

                        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

                        var res2 = (from str in res from Match match in regex.Matches(str) select match.Value).ToList();

                        return new JsonNetResult(res2.Distinct().Take(100));
                    }
                }
                else
                {
                    if (propertyIsBaseObject)
                    {
                        var list = await q.Distinct().Take(50).ToListAsync();

                        foreach (var obj in qIDs)
                        {
                            list.Add(obj);
                        }

                        return new JsonNetResult(list.Distinct(), GetListBoContractResolver(mnemonic));
                    }
                    else
                    {
                        return
                            new JsonNetResult(
                                await q.Select("it." + property).Cast<string>().Distinct().Take(50).ToEnumerableAsync());
                    }
                }
            }
        }

        [AllowGuest]
        public async Task<JsonNetResult> FilterBaseObject_Read(string startswith, string mnemonicCollection, string property)
        {
            using (var uofw = this.CreateSystemUnitOfWork())
            {
                property = property
                    .Replace(JsonNetResult.BASE_COLLECTION_SUFFIX, "")
                    .Replace(JsonNetResult.EASY_COLLECTION_SUFFIX, "")
                    .Split('.')[0];

                var bserv = this.GetService<IBaseObjectCRUDService>(mnemonicCollection);

                startswith = startswith != null ? startswith.Trim() : null;

                var config = GetViewModelConfig(mnemonicCollection);

                var col =
                    GetColumns(mnemonicCollection)
                        .FirstOrDefault(x => x.PropertyName == property);

                string lookupProperty = col.ViewModelConfig.LookupProperty;

                if (col.PropertyType.IsBaseCollection())
                {
                    var q =
                        bserv.GetAll(uofw)
                            .SystemFilter(config, SecurityUser)
                            .Where("it." + property + ".Any()")
                            .SelectMany("it." + property)
                            .Distinct()
                            .OrderBy("it." + lookupProperty);

                    if (!String.IsNullOrEmpty(startswith))
                    {
                        q = q.Where(String.Format("it.{0}.Contains(\"{1}\")", lookupProperty, startswith));
                    }

                    return new JsonNetResult(await q.ToEnumerableAsync(),
                        GetListBoContractResolver(col.ViewModelConfig.Mnemonic));
                }
                else
                {
                    var q =
                        bserv.GetAll(uofw)
                            .SystemFilter(config, SecurityUser)
                            .Where("it." + property + " != null")
                            .Select("it." + property)
                            .Distinct()
                            .OrderBy("it." + lookupProperty);

                    if (!String.IsNullOrEmpty(startswith))
                    {
                        q = q.Where(String.Format("it.{0}.Contains(\"{1}\")", lookupProperty, startswith));
                    }

                    return new JsonNetResult(await q.ToEnumerableAsync(),
                        GetListBoContractResolver(col.ViewModelConfig.Mnemonic));
                }
            }
        }

        [AllowGuest]
        public async Task<JsonNetResult> GetPreset(string mnemonic, string ownerMnemonic)
        {
            try
            {
                using (var uofw = this.CreateSystemUnitOfWork())
                {
                    var preset =
                        await
                            this.PresetService.GetPresetAsync(uofw, mnemonic, ownerMnemonic);

                    return new JsonNetResult(preset);
                }
            }
            catch (Exception ex)
            {
                return new JsonNetResult(new { error = ex.Message });
            }
        }

        [HttpPost]
        [AllowGuest]
        public async Task<JsonNetResult> SavePreset(BaseObject model, string mnemonic)
        {
            try
            {
                using (var uofw = this.CreateSystemUnitOfWork())
                {
                    var preset = model as Preset;

                    if (preset == null) return new JsonNetResult(null);

                    await
                        this.PresetService.SavePresetAsync(uofw, preset);

                    return new JsonNetResult(preset);
                }
            }
            catch (Exception ex)
            {
                return new JsonNetResult(new { error = ex.Message });
            }
        }

        [AllowGuest]
        public PartialViewResult GetToolbarPreset(string mnemonic)
        {
            return PartialView("_ToolbarPreset");
        }

        [HttpPost]
        [AllowGuest]
        public async Task<JsonNetResult> DeletePreset(BaseObject model, string mnemonic)
        {
            try
            {
                using (var uofw = this.CreateSystemUnitOfWork())
                {
                    var preset = model as Preset;

                    await this.PresetService.DeletePresetAsync(uofw, preset);

                    return new JsonNetResult(preset);
                }
            }
            catch (Exception ex)
            {
                return new JsonNetResult(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonNetResult ChangeSortOrder(string mnemonic, int id, int newOrder)
        {
            try
            {
                var serv = this.GetService<IBaseObjectCRUDService>(mnemonic);

                using (var unitOfWork = this.CreateTransactionUnitOfWork())
                {
                    var obj = serv.Get(unitOfWork, id);

                    serv.ChangeSortOrder(unitOfWork, obj, newOrder);

                    unitOfWork.Commit();

                    return new JsonNetResult(new
                    {
                        error = 0,
                        model = obj
                    });
                }
            }
            catch (Exception e)
            {
                return new JsonNetResult(new
                {
                    error = 1,
                    message = e.Message
                });
            }
        }

        [HttpPost]
        public JsonNetResult ChangeCategory(string mnemonic, int id, int categoryID)
        {
            try
            {
                var serv = this.GetService<ICategorizedItemCRUDService>(mnemonic);

                using (var unitOfWork = this.CreateTransactionUnitOfWork())
                {
                    serv.ChangeCategory(unitOfWork, id, categoryID);

                    unitOfWork.Commit();

                    return new JsonNetResult(new
                    {
                        error = 0,
                    });
                }
            }
            catch (Exception e)
            {
                return new JsonNetResult(new
                {
                    error = 1,
                    message = e.Message
                });
            }
        }

        [AllowGuest]
        public ActionResult CreateOnGroundsOf(string baseMnemonic, string destMnemonic, int? id = null)
        {
            try
            {
                var destServ = this.GetService<IBaseObjectCRUDService>(destMnemonic);

                if (destServ != null)
                {
                    using (var unitOfWork = this.CreateUnitOfWork())
                    {
                        BaseObject source = null;

                        if (id.HasValue)
                        {
                            var srcServ = this.GetService<IBaseObjectCRUDService>(baseMnemonic);

                            source = srcServ.Get(unitOfWork, (int)id);
                        }

                        return new JsonNetResult(new { model = (object)destServ.CreateOnGroundsOf(unitOfWork, source) });
                    }
                }
                else
                {
                    var config = this.GetViewModelConfig(destMnemonic);

                    return new JsonNetResult(new { model = Activator.CreateInstance(config.TypeEntity) });
                }
            }
            catch (Exception e)
            {
                return new JsonNetResult(new
                {
                    error = 1,
                    message = e.Message
                });
            }
        }

        [AllowGuest]
        public ActionResult GetAccessObject(string mnemonic, int id)
        {
            try
            {
                using (var unitOfWork = this.CreateUnitOfWork())
                {
                    var config = this.GetViewModelConfig(mnemonic);

                    var item = this.SecurityService.GetObjectAccessItem(unitOfWork, config.TypeEntity, id);

                    if (item == null)
                    {
                        var bServ = this.GetService<IBaseObjectCRUDService>(mnemonic);

                        var model = bServ.Get(unitOfWork, id);

                        if (model != null)
                            item = this.SecurityService.CreateAndSaveAccessItem(unitOfWork, model);
                    }

                    return JsonNet(new { model = item });
                }
            }
            catch (Exception e)
            {
                return JsonNet(new { error = e.ToStringWithInner() });
            }
        }

        [AllowGuest]
        public async Task<ActionResult> GetObjectCount(string mnemonic, bool allItems = true, int? categoryID = null)
        {
            var serv = this.GetService<IBaseObjectCRUDService>(mnemonic);

            try
            {
                using (var unitOfWork = this.CreateUnitOfWork())
                {
                    IQueryable<IBaseObject> q;

                    if (categoryID.HasValue &&
                        serv.GetType().GetInterfaces().Contains(typeof(ICategorizedItemCRUDService)))
                    {
                        q = allItems
                            ? ((ICategorizedItemCRUDService)serv).GetAllCategorizedItems(unitOfWork,
                                categoryID.Value)
                            : ((ICategorizedItemCRUDService)serv).GetCategorizedItems(unitOfWork,
                                categoryID.Value);
                    }
                    else
                    {
                        q = serv.GetAll(unitOfWork);
                    }

                    var config = this.GetViewModelConfig(mnemonic);

                    q = q.SystemFilter(config, SecurityUser);

                    return new JsonNetResult(new { count = await q.CountAsync() });
                }
            }
            catch (Exception e)
            {
                return new JsonNetResult(new { message = e.ToStringWithInner() });
            }
        }

        public ActionResult GetAllTypes(string search)
        {
            if (!String.IsNullOrEmpty(search))
                return JsonNet(Facade.ViewModelConfigService.GetAllObjects(x => x.Title.StartsWith(search)).Select(x => new { ID = x.Key.FullName, Text = x.Value }));
            else
                return JsonNet(Facade.ViewModelConfigService.GetAllObjects().Select(x => new { ID = x.Key.FullName, Text = x.Value }));
        }

        private static readonly object GetViewModelConfigsLocker = new object { };
        [HttpGet]
        [AllowGuest]
        public ActionResult GetViewModelConfigs()
        {
            const string key = "{827E09CD-C503-49E7-8586-886AD3BC4F01}";

            if (CacheWrapper[key] != null)
                return Content(CacheWrapper[key].ToString(), "text/json");

            lock (GetViewModelConfigsLocker)
            {
                if (CacheWrapper[key] == null)
                {
                    CacheWrapper[key] = JsonNet(this.ViewModelConfigs
                        .Select(x => new
                        {
                            Mnemonic = x.Mnemonic,
                            TypeEntity = x.TypeEntity.FullName,
                            Icon = x.Icon,
                            Title = x.Title,
                            ListView = new
                            {
                                Title = x.ListView.Title,
                                Columns = x.ListView.Columns.Select(c => new
                                {
                                    PropertyName = c.PropertyName,
                                    Hidden = c.Hidden
                                })
                            },
                            DetailView = new
                            {
                                Title = x.DetailView.Title,
                                Width = x.DetailView.Width,
                                Height = x.DetailView.Height,
                                IsMaximaze = x.DetailView.isMaximaze,
                            }
                        })).ToString();
                }
            }

            return Content(CacheWrapper[key].ToString(), "text/json");
        }

        [HttpPost]
        public ActionResult KendoUI_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

    }
}
