using Base.DAL;
using Base.Security;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Security
{
    public class WorkflowUserService : IWorkflowUserService
    {
        private static ISecurityUser _manager;

        public WorkflowUserService(
            IUnitOfWork unitOfWork)
        {

            if (_manager == null)
            {
                var repository = unitOfWork.GetRepository<User>();

                var user = repository.Find(x => x.Login == "WorkflowManager");
                if (user == null)
                {
                    user = new User
                    {
                        Login = "WorkflowManager",
                        CategoryID = 1,
                        Roles = new List<Role> {
                            new Role {
                                ChildRoles = new List<ChildRole>(),
                                Permissions = new List<Permission>(),
                                Name = "WorkflowManagerRole",
                                SystemRole = SystemRole.Admin
                            }
                        },
                        FirstName = "Менеджер бизнес-процессов"
                    };

                    repository.Create(user);
                    unitOfWork.SaveChanges();
                }

                _manager = new SecurityUser(user);
            }
        }

        public ISecurityUser WorkflowManager
        {
            get { return _manager; }
        }
    }
}

