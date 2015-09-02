using Base;
using Base.UI;
using Base.Wizard;
using Base.Wizard.Services.Abstract;
using Framework;
using System;
using System.Linq;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class WizardController : BaseController
    {
        public WizardController(IBaseControllerServiceFacade baseServiceFacade) : base(baseServiceFacade) { }


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


        public ActionResult Start(string mnemonic)
        {
            try
            {
                var config = GetViewModelConfig(mnemonic);
                var srv = GetService<IWizardCRUDService>(mnemonic);

                if (config.TypeService != null)
                {
                    using (var uofw = CreateUnitOfWork())
                    {
                        var obj = srv.Start(uofw, config);

                        return new JsonNetResult(new { model = obj });
                    }
                }
                else
                {
                    return new JsonNetResult(new { model = Activator.CreateInstance(config.TypeEntity) });
                }

            }
            catch (Exception e)
            {
                return new JsonNetResult(new { error = 1, message = e.Message });
            }
        }

        [HttpPost]
        public ActionResult Next(string mnemonic, BaseObject model)
        {   
            try
            {
                var config = GetViewModelConfig(mnemonic);
                var srv = GetService<IWizardCRUDService>(mnemonic);


                using (var uofw = CreateUnitOfWork())
                {
                    var obj = srv.NextStep(uofw, model as IWizardObject, config);

                    return new JsonNetResult(new { model = obj, status = "Success" });
                }
            }
            catch (Exception ex)
            {
                return new JsonNetResult(new { model, status = "Error", message = ex.Message });
            }

        }

        [HttpPost]
        public ActionResult Prev(string mnemonic, BaseObject model)
        {
            try
            {
                var config = GetViewModelConfig(mnemonic);
                var srv = GetService<IWizardCRUDService>(mnemonic);

                using (var uofw = CreateUnitOfWork())
                {
                    var obj = srv.PrevStep(uofw, model as IWizardObject, config);

                    return new JsonNetResult(new { model = obj, status = "Success" });
                }
            }
            catch (Exception ex)
            {
                return new JsonNetResult(new { model, status = "Error", message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Save(string mnemonic, BaseObject model)
        {
            try
            {
                var baseConfig = GetViewModelConfigByWizard(mnemonic);
                var srv = GetService<IWizardCRUDService>(mnemonic);

                using (var uofw = CreateUnitOfWork())
                {
                    var obj = srv.Save(uofw, model as IWizardObject);

                    return new JsonNetResult(new { model, @base = obj, error = 0, basemnemonic = baseConfig != null ? baseConfig.Mnemonic : string.Empty }); ;
                }
            }
            catch (Exception ex)
            {
                return new JsonNetResult(new { error = 1, message = ex.Message });
            }
        }

        #region Helper

        internal ViewModelConfig GetViewModelConfigByWizard(string wizardMnemonic)
        {
            if (String.IsNullOrEmpty(wizardMnemonic))
            {
                throw new ArgumentNullException("IsNullOrEmpty", "wizardMnemonic");
            }

            return ViewModelConfigs.FirstOrDefault(m => m.DetailView != null && !string.IsNullOrEmpty(m.DetailView.WizardName) && String.Equals(m.DetailView.WizardName, wizardMnemonic, StringComparison.CurrentCultureIgnoreCase));
        }

        #endregion
    }
}
