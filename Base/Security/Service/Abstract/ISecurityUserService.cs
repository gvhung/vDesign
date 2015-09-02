using Base.Security;
using Base.Service;
using Framework.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.DAL;

namespace Base.Security.Service
{
    public interface ISecurityUserService : IService
    {
        //bool ValidateUser(IUnitOfWork unitOfWork, string userName, string password, bool allowEmptyPassword = false);
        bool ValidateUser(string userName, string password, bool allowEmptyPassword = false);
        User GetUser(IUnitOfWork unitOfWork, int id);
        User GetUser(IUnitOfWork unitOfWork, string login, bool? hidden = false, bool includeAwaitConfirm = false);
        ISecurityUser GetSecurityUser(string login);
        void ChangePassword(int id, string newPass);
        void ValidateLogin(IUnitOfWork unitOfWork, User objsrc, User objdest);
        void ClearAll();
        void Clear(string login);
        void Clear(ISecurityUser securityUser);
        bool IsValidPassword(string password, out string message, int minLen = 6, int maxRepeats = 2, bool checkKeyboard = true);
        User RegisterUser(ISystemUnitOfWork unitOfWork, User user);

    }
}
