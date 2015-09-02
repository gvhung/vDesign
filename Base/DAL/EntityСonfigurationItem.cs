using System;

namespace Base.DAL
{
    public struct EntityConfigurationItem
    {
        public EntityConfigurationItem(Type context, Type repository, Type entity)
        {
            ContextType = context;
            RepositoryType = repository;
            EntityType = entity;
        }

        public readonly Type EntityType;
        public readonly Type ContextType;
        public readonly Type RepositoryType;
    }
}
