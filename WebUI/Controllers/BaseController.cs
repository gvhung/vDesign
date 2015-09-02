using Base;
using Base.Ambient;
using Base.DAL;
using Base.Security;
using Base.Security.Service;
using Base.Security.Service.Abstract;
using Base.Service;
using Base.Settings;
using Base.UI;
using Base.UI.Service;
using Framework;
using Framework.Attributes;
using Framework.Wrappers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WebUI.Authorize;
using WebUI.Helpers;

namespace WebUI.Controllers
{
    public interface IBaseController
    {
        IUnitOfWork CreateUnitOfWork();
        ISystemUnitOfWork CreateSystemUnitOfWork();
        ISystemUnitOfWork CreateSystemUnitOfWork(IUnitOfWork unitOfWork);
        ITransactionUnitOfWork CreateTransactionUnitOfWork();
        ISystemTransactionUnitOfWork CreateSystemTransactionUnitOfWork();
        ISecurityService SecurityService { get; }
        ISecurityUser SecurityUser { get; }
        IFileSystemService FileSystemService { get; }
        ISecurityUserService SecurityUserService { get; }
        IReadOnlyList<ViewModelConfig> ViewModelConfigs { get; }
        IPresetService PresetService { get; }
        ISettingItemService SettingService { get; }
        ViewModelConfig GetViewModelConfig(string mnemonic);
        ViewModelConfig DefaultViewModelConfig(Type type);
        ICacheWrapper CacheWrapper { get; }
        ISessionWrapper SessionWrapper { get; }
        IUiFasade UiFasade { get; }

        CommonEditorViewModel GetCommonEditor(string mnemonic);
        CommonEditorViewModel GetCommonEditor(IUnitOfWork uofw, string mnemonic, BaseObject obj);

        List<EditorViewModel> GetEditors(string mnemonic);
        List<EditorViewModel> GetEditors(Type type);

        List<ColumnViewModel> GetColumns(string mnemonic);
        List<ColumnViewModel> GetColumns(Type type);

        Menu GetMenu();
        HttpContextBase HttpContext { get; }

        BoContractResolver GetBoContractResolver(string mnemonic);
        ListBoContractResolver GetListBoContractResolver(string mnemonic);


    }

    public interface IBaseControllerServiceFacade
    {
        IUnitOfWorkFactory UnitOfWorkFactory { get; }
        ISecurityService SecurityService { get; }
        ISecurityUserService SecurityUserService { get; }
        IFileSystemService FileSystemService { get; }
        IPresetService PresetService { get; }
        ISettingItemService SettingService { get; }
        ICacheWrapper CacheWrapper { get; }
        ISessionWrapper SessionWrapper { get; }
        IViewModelConfigService ViewModelConfigService { get; }
        IMenuService MenuService { get; }
        IDetailViewService DetailViewService { get; }
        IListViewService ListViewService { get; }
        IContractResolverFactory ContractResolverFactory { get; }
        IUiFasade UiFasade { get; }
    }

    public class BaseControllerServiceFacade : IBaseControllerServiceFacade
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ISecurityService _securityService;
        private readonly ISecurityUserService _securityUserService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IPresetService _presetService;
        private readonly ISettingItemService _settingService;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly ISessionWrapper _sessionWrapper;
        private readonly IViewModelConfigService _viewModelConfigService;
        private readonly IDetailViewService _detailViewService;
        private readonly IListViewService _listViewService;
        private readonly IMenuService _menuService;
        private readonly IContractResolverFactory _contractResolverFactory;
        private readonly IUiFasade _uiFasade;

        public BaseControllerServiceFacade(IUnitOfWorkFactory unitOfWorkFactory, ISecurityService securityService, ISecurityUserService securityUserService,
            IFileSystemService fileSystemService, IPresetService presetService, ISettingItemService settingService, ICacheWrapper cacheWrapper,
            ISessionWrapper sessionWrapper, IDetailViewService detailViewService, IViewModelConfigService viewModelConfigService, IMenuService menuService,
            IListViewService listViewService, IContractResolverFactory contractResolverFactory, IUiFasade uiFasade)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _securityService = securityService;
            _securityUserService = securityUserService;
            _fileSystemService = fileSystemService;
            _presetService = presetService;
            _settingService = settingService;
            _cacheWrapper = cacheWrapper;
            _sessionWrapper = sessionWrapper;
            _detailViewService = detailViewService;
            _viewModelConfigService = viewModelConfigService;
            _menuService = menuService;
            _listViewService = listViewService;
            _contractResolverFactory = contractResolverFactory;
            _uiFasade = uiFasade;
        }

        public IUnitOfWorkFactory UnitOfWorkFactory { get { return _unitOfWorkFactory; } }
        public ISecurityService SecurityService { get { return _securityService; } }
        public ISecurityUserService SecurityUserService { get { return _securityUserService; } }
        public IFileSystemService FileSystemService { get { return _fileSystemService; } }
        public IPresetService PresetService { get { return _presetService; } }
        public ISettingItemService SettingService { get { return _settingService; } }
        public ICacheWrapper CacheWrapper { get { return _cacheWrapper; } }
        public ISessionWrapper SessionWrapper { get { return _sessionWrapper; } }
        public IViewModelConfigService ViewModelConfigService { get { return _viewModelConfigService; } }
        public IMenuService MenuService { get { return _menuService; } }
        public IDetailViewService DetailViewService { get { return _detailViewService; } }
        public IListViewService ListViewService { get { return _listViewService; } }
        public IUiFasade UiFasade { get { return _uiFasade; } }
        public IContractResolverFactory ContractResolverFactory { get { return _contractResolverFactory; } }
    }

    [AuthorizeCustom]
    [EnableLog]
    public abstract class BaseController : Controller, IBaseController
    {
        private readonly IBaseControllerServiceFacade _serviceFacade;
        public ISecurityService SecurityService { get { return _serviceFacade.SecurityService; } }
        public ISecurityUserService SecurityUserService { get { return _serviceFacade.SecurityUserService; } }

        private ISecurityUser _securityUser;
        public ISecurityUser SecurityUser
        {
            get
            {
                return _securityUser ?? (_securityUser = AppContext.SecurityUser);
            }
        }
        public IFileSystemService FileSystemService { get { return _serviceFacade.FileSystemService; } }
        public IPresetService PresetService { get { return _serviceFacade.PresetService; } }
        public ISettingItemService SettingService { get { return _serviceFacade.SettingService; } }
        public IReadOnlyList<ViewModelConfig> ViewModelConfigs { get { return _serviceFacade.ViewModelConfigService.GetAll(); } }
        public ICacheWrapper CacheWrapper { get { return _serviceFacade.CacheWrapper; } }
        public ISessionWrapper SessionWrapper { get { return _serviceFacade.SessionWrapper; } }

        public IUiFasade UiFasade
        {
            get { return _serviceFacade.UiFasade; }
        }

        private int _requestID;
        protected BaseController(IBaseControllerServiceFacade serviceFacade)
        {
            _serviceFacade = serviceFacade;

            var random = new Random();
            _requestID = random.Next(0, Int32.MaxValue);
        }

        protected IBaseControllerServiceFacade Facade
        {
            get { return _serviceFacade; }
        }

        public ViewModelConfig GetViewModelConfig(string mnemonic)
        {
            return _serviceFacade.ViewModelConfigService.Get(mnemonic);
        }

        public ViewModelConfig DefaultViewModelConfig(Type type)
        {
            return _serviceFacade.ViewModelConfigService.Get(type);
        }

        public ViewModelConfig DefaultViewModelConfig(string type)
        {
            return _serviceFacade.ViewModelConfigService.Get(type);
        }

        public T GetService<T>(string mnemonic) where T : IService
        {
            var config = this.GetViewModelConfig(mnemonic);

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

        public T GetService<T>() where T : IService
        {
            return (T)DependencyResolver.Current.GetService(typeof(T));
        }

        public Type GetTypeEntity(string mnemonic)
        {
            return this.GetViewModelConfig(mnemonic).TypeEntity;
        }

        protected string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }

        protected string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }

        protected string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }

        protected string RenderPartialViewToString(string viewName, object model, ViewDataDictionary vd = null)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            if (vd != null)
            {
                foreach (string key in vd.Keys)
                {
                    ViewData.Add(key, vd[key]);
                }
            }

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public JsonNetResult JsonNet(object jObject)
        {
            return new JsonNetResult(jObject);
        }

        public JsonNetResult JsonNet(object jObject, params JsonConverter[] converters)
        {
            return new JsonNetResult(jObject, converters);
        }

        public CommonEditorViewModel GetCommonEditor(string mnemonic)
        {
            return _serviceFacade.DetailViewService.GetCommonEditor(mnemonic);
        }

        public CommonEditorViewModel GetCommonEditor(IUnitOfWork unitOfWork, string mnemonic, BaseObject obj)
        {
            return _serviceFacade.DetailViewService.GetCommonEditor(unitOfWork, mnemonic, obj);
        }

        public List<EditorViewModel> GetEditors(string mnemonic)
        {
            return _serviceFacade.DetailViewService.GetEditors(mnemonic);
        }

        public List<EditorViewModel> GetEditors(Type type)
        {
            return _serviceFacade.DetailViewService.GetEditors(type);
        }

        public List<ColumnViewModel> GetColumns(string mnemonic)
        {
            return _serviceFacade.ListViewService.GetColumns(mnemonic);
        }

        public List<ColumnViewModel> GetColumns(Type type)
        {
            return _serviceFacade.ListViewService.GetColumns(type);
        }

        public Menu GetMenu()
        {
            return _serviceFacade.MenuService.Get();
        }

        HttpContextBase IBaseController.HttpContext
        {
            get { return this.HttpContext; }
        }

        public BoContractResolver GetBoContractResolver(string mnemonic)
        {
            return _serviceFacade.ContractResolverFactory.GetBoContractResolver(mnemonic);
        }

        public ListBoContractResolver GetListBoContractResolver(string mnemonic)
        {
            return _serviceFacade.ContractResolverFactory.GetListBoContractResolver(mnemonic);
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return _serviceFacade.UnitOfWorkFactory.Create();
        }

        public ISystemUnitOfWork CreateSystemUnitOfWork()
        {
            return _serviceFacade.UnitOfWorkFactory.CreateSystem();
        }

        public ISystemUnitOfWork CreateSystemUnitOfWork(IUnitOfWork unitOfWork)
        {
            return _serviceFacade.UnitOfWorkFactory.CreateSystem(unitOfWork);
        }

        public ITransactionUnitOfWork CreateTransactionUnitOfWork()
        {
            return _serviceFacade.UnitOfWorkFactory.CreateTransaction();
        }

        public ISystemTransactionUnitOfWork CreateSystemTransactionUnitOfWork()
        {
            return _serviceFacade.UnitOfWorkFactory.CreateSystemTransaction();
        }
    }
}
