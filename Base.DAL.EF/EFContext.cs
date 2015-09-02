using Base.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Base.DAL.EF
{
    public abstract class EFContext : DbContext, IBaseContext
    {
        private readonly IEntityConfiguration _entityConfiguration;

        private DbContextTransaction _transaction = null;

        protected EFContext()
        {

        }

        protected EFContext(IEntityConfiguration entityConfiguration)
        {
            _entityConfiguration = entityConfiguration;
        }

        int IBaseContext.SaveChanges()
        {
            try
            {
                return this.SaveChanges();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                throw new UpdateConcurrencyException("Ошибка обновления записи, были внеcены изменения до вас, обновите модель");
            }
            catch (DbEntityValidationException e)
            {
                string error = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    error = String.Format("У сущности \"{0}\" в состоянии \"{1}\" найдены следующие ошибки:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        error += String.Format("- Свойство: \"{0}\", Ошибка: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new DbEntityValidationException(error);
            }

        }

        async Task<int> IBaseContext.SaveChangesAsync()
        {
            try
            {
                return await this.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                throw new UpdateConcurrencyException("Ошибка обновления записи, были внеcены изменения до вас, обновите модель");
            }

        }

        public bool Transaction
        {
            get { return _transaction != null; }
        }

        public IDisposable BeginTransaction()
        {
            return (_transaction =
                this.Database.BeginTransaction());
        }

        public void Commit()
        {
            if (!Transaction) return;

            _transaction.Commit();
            _transaction = null;
        }

        public void Rollback()
        {
            if (!Transaction) return;

            _transaction.Rollback();
            _transaction = null;
        }

        void IBaseContext.ChangeState<TEntity>(TEntity entity, BaseEntityState entityState)
        {
            switch (entityState)
            {
                case BaseEntityState.Added:
                    this.Entry<TEntity>(entity).State = EntityState.Added;
                    break;
                case BaseEntityState.Deleted:
                    this.Entry<TEntity>(entity).State = EntityState.Deleted;
                    break;
                case BaseEntityState.Detached:
                    this.Entry<TEntity>(entity).State = EntityState.Detached;
                    break;
                case BaseEntityState.Modified:
                    this.Entry<TEntity>(entity).State = EntityState.Modified;
                    break;
                case BaseEntityState.Unchanged:
                    this.Entry<TEntity>(entity).State = EntityState.Unchanged;
                    break;
                default: break;
            }
        }

        public Dictionary<TEntity, BaseEntityState> GetModifiedEntities<TEntity>(bool recursive = true) where TEntity : class
        {
            IEnumerable<DbEntityEntry> q = this.ChangeTracker.Entries();

            if (recursive)
            {
                q = q.Where(e => typeof(TEntity).IsAssignableFrom(e.Entity.GetType()));
            }
            else
            {
                q = q.Where(e => e.Entity is TEntity);
            }

            return q.Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .ToDictionary(e => (TEntity)e.Entity, e =>
                {
                    switch (e.State)
                    {
                        case EntityState.Added: return BaseEntityState.Added;
                        case EntityState.Deleted: return BaseEntityState.Deleted;
                        default: return BaseEntityState.Modified;
                    }
                });
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            foreach (var config in _entityConfiguration.GetContextConfig(this))
            {
                var method = modelBuilder.GetType().GetMethod("Entity");
                method = method.MakeGenericMethod(new Type[] { config.EntityType });
                method.Invoke(modelBuilder, null);
            }

            modelBuilder.Conventions.Add(new NonPublicColumnAttributeConvention());
        }

        public sealed class NonPublicColumnAttributeConvention : Convention
        {
            public NonPublicColumnAttributeConvention()
            {
                Types().Having(NonPublicProperties)
                       .Configure((config, properties) =>
                       {
                           foreach (var prop in properties)
                           {
                               config.Property(prop);
                           }
                       });
            }

            private IEnumerable<PropertyInfo> NonPublicProperties(Type type)
            {
                var matchingProperties = type.GetProperties(BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance)
                                             .Where(propInfo => propInfo.GetCustomAttributes(typeof(ColumnAttribute), true).Length > 0)
                                             .ToArray();
                return matchingProperties.Length == 0 ? null : matchingProperties;
            }
        }

        protected void InitEntityConfig(IEnumerable<EntityConfigurationItem> configs, DbModelBuilder modelBuilder)
        {
            foreach (var config in configs)
            {
                var method = modelBuilder.GetType().GetMethod("Entity");
                method = method.MakeGenericMethod(new Type[] { config.EntityType });
                method.Invoke(modelBuilder, null);
            }
        }


        public bool IsModifiedEntity<TEntity>(TEntity entity, BaseEntityState modif) where TEntity : BaseObject
        {
            switch (modif)
            {
                case BaseEntityState.Added:
                    return this.ChangeTracker.Entries<TEntity>().Any(x => x.State == EntityState.Added && (x.Entity == entity || (entity.ID != 0 && x.Entity.ID == entity.ID)));
                case BaseEntityState.Modified:
                    return this.ChangeTracker.Entries<TEntity>().Any(x => x.State == EntityState.Modified && (x.Entity == entity || (entity.ID != 0 && x.Entity.ID == entity.ID)));
                case BaseEntityState.Deleted:
                    return this.ChangeTracker.Entries<TEntity>().Any(x => x.State == EntityState.Deleted && (x.Entity == entity || (entity.ID != 0 && x.Entity.ID == entity.ID)));
            }
            return false;
        }
    }
}
