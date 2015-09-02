using Base;
using Base.QueryableExtensions;
using Base.Service;
using Base.UI;
using Framework;
using Framework.FullTextSearch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class HCategoryController : BaseController
    {
        public HCategoryController(IBaseControllerServiceFacade baseServiceFacade) : base(baseServiceFacade) { }

        public JsonNetResult Get(string mnemonic, int id)
        {
            try
            {
                using (var uofw = this.CreateUnitOfWork())
                {
                    var serv = this.GetService<ICategoryCRUDService>(mnemonic);

                    var config = this.GetViewModelConfig(mnemonic);

                    var pifo = config.TypeEntity.GetProperty(config.LookupProperty);

                    var category = serv.Get(uofw, id) as HCategory;

                    if (category == null) return new JsonNetResult(null);

                    var configTreeView = config.ListView as TreeView;

                    return new JsonNetResult(
                        new
                        {
                            id = category.ID,
                            Title = pifo.GetValue(category),
                            CategoryItemMnemonic =
                                configTreeView != null && configTreeView.ExtendedCategory
                                    ? (category as IExtendedCategory).CategoryItemMnemonic
                                    : null,
                            Image = category is ITreeNodeImage ? (category as ITreeNodeImage).Image : null,
                            Icon = category is ITreeNodeIcon ? (category as ITreeNodeIcon).Icon : null,
                            hasChildren = category.Children.Any(m => m.Hidden == false),
                            isRoot = category.IsRoot
                        });
                }
            }
            catch (Exception e)
            {
                return new JsonNetResult(
                    new
                    {
                        error = String.Format("Ошибка: {0}", e.Message)
                    }
                );
            }
        }

        public async Task<JsonNetResult> TreeView_Read(int? id, string mnemonic, string searchStr = null, string sysFilter = null)
        {
            using (var uofw = this.CreateUnitOfWork())
            {
                var config = this.GetViewModelConfig(mnemonic);

                var configTreeView = config.ListView as TreeView;

                var pifo = config.TypeEntity.GetProperty(config.LookupProperty);

                var serv = this.GetService<ICategoryCRUDService>(mnemonic);

                //TODO: id == null -> временный костыль
                if (id == null && !String.IsNullOrEmpty(searchStr))
                {
                    var res =
                        (await
                            serv.GetAll(uofw)
                                .FullTextSearch(searchStr, this.CacheWrapper)
                                .Take(100)
                                .ToListAsync()).Cast<HCategory>().OrderBy(x => x.sys_all_parents).Select(a => new
                                {
                                    id = a.ID,
                                    Title = pifo.GetValue(a),
                                    CategoryItemMnemonic =
                                        configTreeView.ExtendedCategory
                                            ? (a as IExtendedCategory).CategoryItemMnemonic
                                            : null,
                                    Image = a is ITreeNodeImage ? (a as ITreeNodeImage).Image : null,
                                    Icon = a is ITreeNodeIcon ? (a as ITreeNodeIcon).Icon : null,
                                });

                    return new JsonNetResult(res);
                }
                else
                {
                    IEnumerable<HCategory> list;

                    if (id == null)
                        list = await serv.GetRootsAsync(uofw);
                    else
                        list = await serv.GetChildrenAsync(uofw, (int)id);

                    return new JsonNetResult(
                        list.Select(a => new
                        {
                            id = a.ID,
                            Title = pifo.GetValue(a),
                            CategoryItemMnemonic =
                                configTreeView.ExtendedCategory ? (a as IExtendedCategory).CategoryItemMnemonic : null,
                            Image = a is ITreeNodeImage ? (a as ITreeNodeImage).Image : null,
                            Icon = a is ITreeNodeIcon ? (a as ITreeNodeIcon).Icon : null,
                            hasChildren = a.Children.Any(m => m.Hidden == false),
                            isRoot = a.IsRoot
                        })
                        );
                }
            }
        }

        public async Task<JsonNetResult> GetChildren(int id, string mnemonic)
        {
            using (var uofw = this.CreateUnitOfWork())
            {
                var serv = this.GetService<ICategoryCRUDService>(mnemonic);

                var list = await serv.GetChildrenAsync(uofw, id);

                return new JsonNetResult(list);
            }
        }

        [HttpPost]
        public ActionResult ChangePosition(string mnemonic, int id, int? parentID, int? posChangeID, string typePosChange)
        {
            var serv = this.GetService<ICategoryCRUDService>(mnemonic);

            try
            {
                using (var uofw = this.CreateTransactionUnitOfWork())
                {
                    var obj = serv.Get(uofw, id) as HCategory;

                    if (obj != null)
                    {
                        serv.ChangePosition(uofw, obj, posChangeID, typePosChange);

                        uofw.Commit();

                        return new JsonNetResult(new
                        {
                            message = "Раздел успешно перенесен!"
                        });
                    }
                    
                    return new JsonNetResult(new
                    {
                        error = "Ошибка переноса раздела: раздел не найден"
                    });
                }

            }
            catch (Exception e)
            {
                return new JsonNetResult(new
                {
                    error = String.Format("Ошибка переноса раздела: {0}", e.Message)
                });
            }
        }
    }
}
