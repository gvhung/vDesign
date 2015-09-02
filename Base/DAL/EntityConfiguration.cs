using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.DAL
{
    public class EntityConfiguration : IEntityConfiguration
    {
        private Dictionary<Type, EntityConfigurationItem> _configs;

        public EntityConfigurationItem Get(Type type)
        {
            if (!_configs.ContainsKey(type))
                throw new Exception(String.Format("The entity configuration is not configured for type \"{0}\"", type.FullName));

            return _configs[type];
        }

        public EntityConfigurationItem Get<T>() where T : BaseObject
        {
            return this.Get(typeof(T));
        }

        public IEnumerable<EntityConfigurationItem> GetContextConfig(Type contextType)
        {
            return _configs.Select(x => x.Value).Where(x => x.ContextType == contextType).ToList();
        }

        public IEnumerable<EntityConfigurationItem> GetContextConfig(IBaseContext context)
        {
            return this.GetContextConfig(context.GetType());
        }

        public ConfigContext Config()
        {
            if (_configs == null)
                _configs = new Dictionary<Type, EntityConfigurationItem>();

            return new ConfigContext(_configs);
        }
    }

    public class ConfigContext
    {
        private Dictionary<Type, EntityConfigurationItem> _configs;

        public ConfigContext(Dictionary<Type, EntityConfigurationItem> configs)
        {
            _configs = configs;
        }

        public ConfigRepository Context<T>() where T : IBaseContext
        {
            return new ConfigRepository(typeof(T), _configs);
        }
    }

    public class ConfigRepository
    {
        private Dictionary<Type, EntityConfigurationItem> _configs;
        private Type _typeContext;

        public ConfigRepository(Type typeContext, Dictionary<Type, EntityConfigurationItem> configs)
        {
            _typeContext = typeContext;
            _configs = configs;
        }

        public ConfigEntity Repository<T>() where T : IRepository
        {
            return new ConfigEntity(_typeContext, typeof(T), _configs);
        }
    }

    public class ConfigEntity
    {
        private Dictionary<Type, EntityConfigurationItem> _configs;
        private Type _typeContext;
        private Type _typeRepository;

        public ConfigEntity(Type typeContext, Type typeRepository, Dictionary<Type, EntityConfigurationItem> configs)
        {
            _typeContext = typeContext;
            _typeRepository = typeRepository;
            _configs = configs;
        }

        public ConfigEntity Entity<T>() where T : IBaseObject
        {
            _configs.Add(typeof(T), new EntityConfigurationItem(_typeContext, _typeRepository, typeof(T)));

            return this;
        }
    }
}
