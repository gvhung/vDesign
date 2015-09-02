using Base.DAL;
using Base.UI;

namespace Base.Wizard.Services.Abstract
{
    public interface IWizardService<T> : IWizardCRUDService where T : IWizardObject
    {
        T Start(IUnitOfWork unitOfWork, ViewModelConfig config);
        T NextStep(IUnitOfWork unitOfWork, T obj, ViewModelConfig config);
        T PrevStep(IUnitOfWork unitOfWork, T obj, ViewModelConfig config);
        T Save(IUnitOfWork unitOfWork, T obj);
    }
}
