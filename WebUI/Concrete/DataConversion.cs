using Base;
using Base.Ambient;
using Base.DAL;
using Base.Security;
using Base.Security.Service;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebUI.Concrete
{
    public class DataConversion
    {
        public static void Run(IKernel kernel)
        {
            const string userLogin = "{954C1CDD-A1D9-4A18-B1CE-7B02073D1578}";

            //TODO: add ver
            var types = new List<Type> { typeof(IBaseInitializer) };

            //NOTE:
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    types.AddRange(assembly.GetTypes()
                        .Where(x => typeof(IBaseInitializer).IsAssignableFrom(x)));
                }
                catch
                {
                    // ignored
                }
            }

            var unitOfWorkFactory = kernel.Get<IUnitOfWorkFactory>();

            ISecurityUser sysuser;

            using (var unitOfWork = unitOfWorkFactory.CreateSystem())
            {
                var userService = kernel.Get<IUserService>();

                var user = userService.GetAll(unitOfWork).FirstOrDefault(x => x.Login == userLogin);
                
                if (user == null)
                {
                    user = userService.Create(unitOfWork, new User
                    {
                        UserCategory = new UserCategory()
                        {
                            Name = "DataConversion",
                            Hidden = true,
                        },
                        Login = userLogin,
                        LastName = "DataConversion",
                    });
                }

                sysuser = new SecurityUser(user);
            }

            using (var unitOfWork = unitOfWorkFactory.CreateSystemTransaction())
            {
                var appContextBootstrapper = kernel.Get<IAppContextBootstrapper>();

                using (appContextBootstrapper.LocalContextSecurity(sysuser))
                {
                    foreach (var initializer in types.Distinct().Select(type => kernel.Get(type)).OfType<IBaseInitializer>())
                    {
                        initializer.Init(unitOfWork);
                    }

                    unitOfWork.Commit();
                }

                //appContextBootstrapper.SetSecurityUser(new SecurityUser(sysuser));

                //foreach (var initializer in types.Distinct().Select(type => kernel.Get(type)).OfType<IBaseInitializer>())
                //{
                //    initializer.Init(unitOfWork);
                //}

                //unitOfWork.Commit();

                //appContextBootstrapper.RemoveSecurityUser();
            }
        }
    }
}