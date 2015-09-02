using Base.Ambient;
using Base.Security;
using Framework;
using Framework.EnumerableExtesions;
using Framework.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.UI
{
    public class MenuService : IMenuService
    {
        private readonly IMenuLoader _loder;
        private readonly IViewModelConfigService _viewModelConfigService;
        private readonly ICacheWrapper _cacheWrapper;

        private readonly static object _cacheLocker = new object();

        private const string _keyCache = "71B115C2-F1A8-4D02-9FF0-74B358C4F339";

        public MenuService(IMenuLoader loder, IViewModelConfigService viewModelConfigService, ICacheWrapper cacheWrapper)
        {
            _loder = loder;
            _viewModelConfigService = viewModelConfigService;
            _cacheWrapper = cacheWrapper;
        }

        private string GetUserKeyCache(ISecurityUser user)
        {
            return String.Format("{1}:{0}", _keyCache, user.GetKey());
        }

        public Menu Get()
        {
            string userKeyCache = GetUserKeyCache(AppContext.SecurityUser);

            if (_cacheWrapper[userKeyCache] != null)
                return _cacheWrapper[userKeyCache] as Menu;

            lock (_cacheLocker)
            {
                if (_cacheWrapper[_keyCache] == null)
                {
                    _cacheWrapper[_keyCache] = _loder.Load();
                }

                var srcMenu = _cacheWrapper[_keyCache] as Menu;

                var resMenu = ObjectHelper.CreateAndCopyObject<Menu>(srcMenu, new Type[] { typeof(MenuItem) });

                InitAndFilterPermission(srcMenu.Items, resMenu.Items);

                resMenu = this.FilterEmpty(resMenu);

                _cacheWrapper[userKeyCache] = resMenu;
            }

            return (Menu) _cacheWrapper[userKeyCache];
        }

        private void InitAndFilterPermission(List<MenuItem> srcItems, List<MenuItem> destItems, MenuItem parent = null)
        {
            var securityUser = AppContext.SecurityUser;

            foreach (MenuItem sitem in srcItems.ToList())
            {
                MenuItem ditem = null;
                
                if (sitem.ForSystemRole == null || securityUser.IsSysRole(sitem.ForSystemRole.Value))
                {
                    if (!String.IsNullOrEmpty(sitem.Mnemonic))
                    {
                        var config = _viewModelConfigService.Get(sitem.Mnemonic);

                        if (securityUser.IsPermission(config.TypeEntity, TypePermission.Navigate))
                        {
                            ditem = ObjectHelper.CreateAndCopyObject<MenuItem>(sitem, new Type[] { typeof(MenuItem) });

                            if (String.IsNullOrEmpty(ditem.Title))
                            {
                                ditem.Title = config.ListView.Title;
                            }

                            if (String.IsNullOrEmpty(ditem.Icon))
                            {
                                ditem.Icon = config.Icon;
                            }
                        }
                    }
                    else
                    {
                        ditem = ObjectHelper.CreateAndCopyObject<MenuItem>(sitem, new Type[] { typeof(MenuItem) });
                    }

                    if (ditem != null)
                    {
                        destItems.Add(ditem);

                        InitAndFilterPermission(sitem.Items, ditem.Items, sitem);
                    }

                    if (ditem != null)
                    {
                        ditem.Parent = parent;
                    }
                }
            }
        }

        private Menu FilterEmpty(Menu menu)
        {
            Action<MenuItem> func = null;
            func = menuItem =>
            {
                List<MenuItem> items = new List<MenuItem>();

                for (int i = 0; i < menuItem.Items.Count; i++)
                {
                    MenuItem item = menuItem.Items[i];

                    if (item.Items.SelectRecursive(x => x.Items, x => x.Item.Title != "-").Any())
                    {
                        items.Add(item);

                        func(item);
                    }
                    else if (item.Items.Count == 0 && item.Title != "-" && (!String.IsNullOrEmpty(item.URL) || !String.IsNullOrEmpty(item.Mnemonic)))
                    {
                        items.Add(item);
                    }
                    else if (item.Title == "-" && i < menuItem.Items.Count - 1 && menuItem.Items[i + 1].Title != "-")
                    {
                        items.Add(item);
                    }
                }

                menuItem.Items = items;
            };

            foreach (var item in menu.Items)
            {
                func(item);
            }

            return menu;
        }

        public void Clear(ISecurityUser user)
        {
            _cacheWrapper.Remove(GetUserKeyCache(user));
        }
    }
}
