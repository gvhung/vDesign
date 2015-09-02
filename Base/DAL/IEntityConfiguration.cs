using System;
using System.Collections.Generic;

namespace Base.DAL
{
    public interface IEntityConfiguration
    {
        EntityConfigurationItem Get(Type type);
        EntityConfigurationItem Get<T>() where T : BaseObject;
        IEnumerable<EntityConfigurationItem> GetContextConfig(Type contextType);
        IEnumerable<EntityConfigurationItem> GetContextConfig(IBaseContext context);
        ConfigContext Config();
    }
}
