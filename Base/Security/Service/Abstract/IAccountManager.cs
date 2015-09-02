using Base.DAL;
using Base.Service;
using System;
using System.Threading.Tasks;

namespace Base.Security.Service.Abstract
{
    public interface IAccountManager : IService
    {
        Task ResetPassword(IUnitOfWork unitOfWork, Uri systemUrl, string login);
        void Register(ISystemUnitOfWork unitOfWork, Uri systemUrl, string email, string password, string firstName, string lastName);
        User ConfirmationVerify(IUnitOfWork unitOfWork, ConfirmType type, int userId, string code);
    }
}
