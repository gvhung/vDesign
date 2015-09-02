using Base;
using Base.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebUI.Concrete
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IEntityConfiguration _entityConfiguration;

        private readonly Dictionary<Type, IRepository> _ropositories;
        private readonly Dictionary<Type, IBaseContext> _contexts;

        public RepositoryFactory(IEntityConfiguration entityConfiguration)
        {
            _entityConfiguration = entityConfiguration;

            _ropositories = new Dictionary<Type, IRepository>();
            _contexts = new Dictionary<Type, IBaseContext>();
        }

        public IRepository<T> RepositoryOf<T>() where T : BaseObject
        {
            var entityConfig = _entityConfiguration.Get<T>();

            IBaseContext context = null;

            if (!_contexts.ContainsKey(entityConfig.ContextType))
            {
                if (entityConfig.ContextType.IsInterface)
                    context = DependencyResolver.Current.GetService(entityConfig.ContextType) as IBaseContext;
                else
                    context = Activator.CreateInstance(entityConfig.ContextType, _entityConfiguration) as IBaseContext;

                _contexts.Add(entityConfig.ContextType, context);
            }
            else
            {
                context = _contexts[entityConfig.ContextType];
            }

            IRepository<T> rep = null;

            if (!_ropositories.ContainsKey(entityConfig.EntityType))
            {
                if (entityConfig.ContextType.IsInterface)
                {
                    rep = DependencyResolver.Current.GetService(entityConfig.RepositoryType) as IRepository<T>;
                }
                else
                {
                    var repType = entityConfig.RepositoryType.GetGenericTypeDefinition().MakeGenericType(new Type[] { typeof(T) });

                    rep = Activator.CreateInstance(repType, context) as IRepository<T>;
                }

                _ropositories.Add(entityConfig.EntityType, rep);
            }
            else
            {
                rep = _ropositories[entityConfig.EntityType] as IRepository<T>;
            }

            return rep;
        }

        public IBaseContext ContextOf<T>() where T : BaseObject
        {
            var entityConfig = _entityConfiguration.Get<T>();

            return _contexts[entityConfig.ContextType];
        }


        public IReadOnlyList<IBaseContext> GetContexts()
        {
            return _contexts.Select(x => x.Value).ToList().AsReadOnly();
        }

        public void Dispose()
        {
            _ropositories.Clear();
            _contexts.Clear();
        }
    }
}