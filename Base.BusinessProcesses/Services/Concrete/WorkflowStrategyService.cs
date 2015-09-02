using Base.BusinessProcesses.Services.Abstract;
using Base.BusinessProcesses.Strategies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class WorkflowStrategyService : IWorkflowStrategyService
    {
        private readonly IWorkflowServiceResolver _resolver;
        private readonly List<WorkflowStrategyDescriptor> _strategies = new List<WorkflowStrategyDescriptor>();
        private List<WorkflowCommonStrategyDescriptor> _commonStrategies;

        public WorkflowStrategyService(IWorkflowServiceResolver resolver)
        {
            _resolver = resolver;
            var strategyType = typeof (IWorkflowStrategy);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    _strategies.AddRange(
                        assembly.GetTypes()
                            .Where(x => strategyType.IsAssignableFrom(x))
                            .Select(x => new WorkflowStrategyDescriptor(x)));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        public void RegisterCommonStrategies(WorkflowCommonStrategyModule strategyModule)
        {
            _commonStrategies = GetCommonStrategies(strategyModule);
        }

        private static List<WorkflowCommonStrategyDescriptor> GetCommonStrategies(WorkflowCommonStrategyModule strategyModule)
        {
            strategyModule.Register();

            return strategyModule.GetCommonStrategies();
        }

        public void RegisterCommonStrategies<TWorkflowCommonStrategyModule>() where TWorkflowCommonStrategyModule : WorkflowCommonStrategyModule
        {
            _commonStrategies = GetCommonStrategies(_resolver.GetStrategyModule(typeof (TWorkflowCommonStrategyModule)));
        }

        public TStrategy GetStrategyInstance<TStrategy>(string name) where TStrategy : class, IWorkflowStrategy
        {
            var strategy = _strategies.FirstOrDefault(x => x.Identifier == name);
            if (strategy != null)
                return _resolver.GetStrategy(strategy.Type) as TStrategy;

            return default(TStrategy);
        }

        public TStrategy GetCommonStrategyInstance<TStrategy>(Func<WorkflowCommonStrategyDescriptor, bool> selector) where TStrategy : class, IWorkflowStrategy
        {
            var strategy =
                _commonStrategies.Where(x => typeof(TStrategy).IsAssignableFrom(x.Type)).FirstOrDefault(selector);

            if (strategy != null)
                return _resolver.GetStrategy(strategy.Type) as TStrategy;

            return default(TStrategy);
        }

        public IEnumerable<WorkflowStrategyDescriptor> GetStrategies<TStrategy>() where TStrategy : class, IWorkflowStrategy
        {
            var type = typeof(TStrategy);

            return _strategies.Where(x => type.IsAssignableFrom(x.Type) && type != x.Type);
        }
    }

    public abstract class WorkflowCommonStrategyModule
    {
        private readonly List<WorkflowStrategyBuilder> _binders = new List<WorkflowStrategyBuilder>();

        internal List<WorkflowCommonStrategyDescriptor> GetCommonStrategies()
        {
            return _binders.Select(x => x.ToStrategyDescriptor()).ToList();
        }

        protected WorkflowStrategyBuilder Bind<TStrategy>() where TStrategy : class, IWorkflowStrategy
        {
            var binder = new WorkflowStrategyBuilder(typeof(TStrategy));

            _binders.Add(binder);

            return binder;
        }

        public abstract void Register();
    }

    public class WorkflowStrategyBuilder
    {
        private readonly WorkflowCommonStrategyDescriptor _strategyDescriptor;

        internal WorkflowStrategyBuilder(Type type)
        {
            _strategyDescriptor = new WorkflowCommonStrategyDescriptor(type);
        }

        public WorkflowStrategyBuilder ToEntity<TEntity>() where TEntity : BaseObject
        {
            _strategyDescriptor.EntityType = typeof (TEntity);

            return this;
        }

        internal WorkflowCommonStrategyDescriptor ToStrategyDescriptor()
        {
            return _strategyDescriptor;
        }
    }
}