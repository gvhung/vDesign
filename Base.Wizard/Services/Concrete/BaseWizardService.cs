using Base.DAL;
using Base.Security.Service.Abstract;
using Base.Service;
using Base.UI;
using Base.Wizard.Services.Abstract;
using Base.Wizard.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Wizard.Services.Concrete
{
    public abstract class BaseWizardService<T> : IWizardService<T> where T : BaseObject, IWizardObject, new()
    {
        private readonly IBaseObjectCRUDService _baseService;
        private readonly ISecurityService _securityService;

        protected BaseWizardService(IBaseObjectCRUDService baseService, ISecurityService securityService)
        {
            _baseService = baseService;
            _securityService = securityService;
        }

        public IBaseObjectCRUDService BaseService
        {
            get { return _baseService; }
        }

        protected ISecurityService SecurityService
        {
            get
            {
                return _securityService;
            }
        }

        protected T GetNextState(T obj, string step)
        {
            if (obj.PreviousSteps == null) obj.PreviousSteps = new List<string>();

            obj.PreviousSteps.Add(obj.Step);
            obj.Step = step;

            return obj;
        }

        protected T GetPrevState(T obj)
        {
            if (obj.PreviousSteps == null || !obj.PreviousSteps.Any()) return obj;

            obj.Step = obj.PreviousSteps.Last();
            obj.PreviousSteps.Remove(obj.PreviousSteps.Last());

            return obj;
        }

        protected int GetStepIndexByName(string stepname, ViewModelConfig config)
        {
            var wDetailView = (WizardDetailView)config.DetailView;

            //NOTE: Возможно есть смысл вернуть Exception
            if (wDetailView == null || wDetailView.Steps == null || wDetailView.Steps.Count == 0) return -1;

            int index;
            for (index = 0; index < wDetailView.Steps.Count; index++)
            {
                if (String.Equals(wDetailView.Steps[index].Name, stepname, StringComparison.CurrentCultureIgnoreCase))
                    break;
            }

            return index;
        }

        protected string GetStepNameByIndex(int index, ViewModelConfig config)
        {
            var wDetailView = (WizardDetailView)config.DetailView;

            //NOTE: Возможно есть смысл вернуть Exception
            if (wDetailView == null || wDetailView.Steps == null || wDetailView.Steps.Count == 0 || index >= wDetailView.Steps.Count) return "";

            return wDetailView.Steps[index].Name;
        }

        protected bool StepIsLast(string stepname, ViewModelConfig config)
        {
            var wDetailView = (WizardDetailView)config.DetailView;

            //NOTE: Возможно есть смысл вернуть Exception
            if (wDetailView == null || wDetailView.Steps == null || wDetailView.Steps.Count == 0) return true;

            return String.Equals(wDetailView.Steps[wDetailView.Steps.Count - 1].Name, stepname,
                StringComparison.CurrentCultureIgnoreCase);
        }

        protected bool StepIsFirst(string stepname, ViewModelConfig config)
        {
            var wDetailView = (WizardDetailView)config.DetailView;

            //NOTE: Возможно есть смысл вернуть Exception
            if (wDetailView == null || wDetailView.Steps == null || wDetailView.Steps.Count == 0) return true;

            return String.Equals(wDetailView.Steps[0].Name, stepname,
                StringComparison.CurrentCultureIgnoreCase);
        }

        public virtual T NextStep(IUnitOfWork unitOfWork, T obj, ViewModelConfig config)
        {
            var wDetailView = config.DetailView as WizardDetailView;
            if (wDetailView == null) return obj;

            var newStep = WizardConfig.WIZARD_COMPLETE_KEY;

            var currentIndex = GetStepIndexByName(obj.Step, config);

            if (currentIndex < wDetailView.Steps.Count - 1)
                newStep = GetStepNameByIndex(currentIndex + 1, config);

            obj = GetNextState(obj, newStep);

            return obj;
        }

        public virtual T PrevStep(IUnitOfWork unitOfWork, T obj, ViewModelConfig config)
        {
            if (!StepIsFirst(obj.Step, config))
                obj = GetPrevState(obj);

            return obj;
        }

        public virtual T Start(IUnitOfWork unitOfWork, ViewModelConfig config)
        {
            var obj = Activator.CreateInstance(config.TypeEntity) as T;

            obj.PreviousSteps = new List<string>();

            var wDetailView = config.DetailView as WizardDetailView;
            if (wDetailView == null || wDetailView.Steps == null || wDetailView.Steps.Count == 0) return obj;

            obj.Step = wDetailView.FirstStep;
            obj.StepCount = wDetailView.Steps.Count;

            return obj;
        }

        public virtual T Save(IUnitOfWork unitOfWork, T obj)
        {
            return BaseService.Create(unitOfWork, obj) as T;
        }

        #region CRUD
        IWizardObject IWizardCRUDService.NextStep(IUnitOfWork unitOfWork, IWizardObject obj, ViewModelConfig config)
        {
            return NextStep(unitOfWork, obj as T, config);
        }

        IWizardObject IWizardCRUDService.PrevStep(IUnitOfWork unitOfWork, IWizardObject obj, ViewModelConfig config)
        {
            return PrevStep(unitOfWork, obj as T, config);
        }

        IWizardObject IWizardCRUDService.Start(IUnitOfWork unitOfWork, ViewModelConfig config)
        {
            return Start(unitOfWork, config);
        }

        BaseObject IWizardCRUDService.Save(IUnitOfWork unitOfWork, IWizardObject obj)
        {
            return Save(unitOfWork, obj as T);
        }
        #endregion

    }
}
