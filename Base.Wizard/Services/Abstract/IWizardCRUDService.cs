using Base.DAL;
using Base.Service;
using Base.UI;

namespace Base.Wizard.Services.Abstract
{
    public interface IWizardCRUDService: IService
    {
        IWizardObject Start(IUnitOfWork unitOfWork, ViewModelConfig config);
        IWizardObject NextStep(IUnitOfWork unitOfWork, IWizardObject obj, ViewModelConfig config);
        IWizardObject PrevStep(IUnitOfWork unitOfWork, IWizardObject obj, ViewModelConfig config);
        BaseObject Save(IUnitOfWork unitOfWork, IWizardObject obj);
    }
}
