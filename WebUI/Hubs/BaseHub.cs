using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Service;
using Base.UI;
using Microsoft.AspNet.SignalR;

namespace WebUI.Hubs
{
    public class BaseHub : Hub
    {
        private readonly IViewModelConfigService _viewModelConfigService;

        public BaseHub(IViewModelConfigService viewModelConfigService)
        {
            _viewModelConfigService = viewModelConfigService;
        }

        internal T GetService<T>(string mnemonic) where T : IService
        {
            var config = _viewModelConfigService.Get(mnemonic);

            var type = config.TypeService;

            if (type == typeof(IBaseObjectService<>) ||
                type == typeof(IBaseCategoryService<>) ||
                type == typeof(IBaseCategorizedItemService<>))
            {
                var arrservice = config.Service.Split(',');

                type = Type.GetType(String.Format("{0}[[{1}]],{2}", arrservice[0], config.Entity, arrservice[1]));
            }


            return (T)DependencyResolver.Current.GetService(type);
        }
    }
}