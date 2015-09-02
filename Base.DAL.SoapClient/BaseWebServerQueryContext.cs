using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Base.DAL.SoapClient
{
    public abstract class BaseWebServerQueryContext : IBaseWebServerQueryContext
    {
        private Dictionary<EntityKey, WebEntityEntry> _entities = new Dictionary<EntityKey, WebEntityEntry>();

        protected abstract IQueryable GetQueryable(Expression expression, Type type);
        protected abstract object CallMethod(Expression expression, Type type);

        protected abstract int SaveChanges();

        public virtual object Execute(Expression expression, bool IsEnumerable)
        {
            ExpressionGetType getType = new ExpressionGetType();

            Type type = getType.GetType(expression);

            if (IsEnumerable)
            {
                //IQueryable queryable = this.GetQueryable(expression, type);

                //ExpressionTreeModifier treeCopier = new ExpressionTreeModifier(queryable);
                //Expression newExpressionTree = treeCopier.Visit(expression);

                //return queryable.Provider.CreateQuery(newExpressionTree);

                return this.GetQueryable(expression, type);
            }
            else
            {
                return this.CallMethod(expression, type);
            }
        }


        public TEntity Attach<TEntity>(TEntity entity) where TEntity : class
        {
            EntityKey key = new EntityKey(entity);

            if (!_entities.ContainsKey(key))
            {
                _entities.Add(key, new WebEntityEntry(entity));
            }

            return entity;
        }

        public WebEntityEntry Entry<TEntity>(TEntity entity) where TEntity : class
        {
            EntityKey key = new EntityKey(entity);

            return _entities[key];
        }


        public IEnumerable<WebEntityEntry> Entries()
        {
            return _entities.Select(m => m.Value).ToList();
        }


        int IBaseWebServerQueryContext.SaveChanges()
        {
            return this.SaveChanges();
        }

        public void Dispose()
        {
            _entities = null;
        }


        public Task<int> SaveChangesAsync()
        {
            return this.SaveChangesAsync();
        }
    }

    public class ExpressionGetType : ExpressionVisitor
    {
        private Type _type;

        public Type GetType(Expression expression)
        {
            this.Visit(expression);

            return _type;
        }


        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (typeof(IQueryableWebServerData).IsAssignableFrom(c.Type))
            {
                _type = TypeSystem.GetElementType(c.Type);
            }

            return c;
        }
    }


    public class ExpressionTreeModifier : ExpressionVisitor
    {
        private IQueryable queryable;
        
        public ExpressionTreeModifier(IQueryable q)
        {
            this.queryable = q;
        }

        
        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (typeof(IQueryableWebServerData).IsAssignableFrom(c.Type))
            {
                return Expression.Constant(this.queryable);
            }
            else
                return c;

        }
    }
}
