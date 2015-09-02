using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Base.DAL.EF
{
    public class EFExtendedQueryable<T> : ExtendedQueryable<T>
    {
        public EFExtendedQueryable(IQueryable<T> query)
            : base(query)
        {
        }

        public EFExtendedQueryable(IQueryable<T> query, IQueryProvider provider) : base(query, provider) { }


        public override async Task<List<T>> ToGenericListAsync()
        {
            return await this.Query.ToListAsync();
        }
        public override async Task<IList> ToListAsync()
        {
            return await this.Query.ToListAsync();
        }
        public override async Task<IEnumerable> ToEnumerableAsync()
        {
            return await this.Query.ToListAsync();
        }

        public override Task<List<T>> ToGenericListAsync(CancellationToken token)
        {
            return Query.ToListAsync(token);
        }

        public override Task<int> CountAsync()
        {
            return this.Query.CountAsync();
        }

        protected override ExtendedQueryableProvider CreateProvider()
        {
            return new EFExtendedQueryableProvider(Query.Provider);
        }

        public override Task<T> FirstOrDefaultAsync()
        {
            return Query.FirstOrDefaultAsync();
        }
    }

    public class EFExtendedQueryableProvider : ExtendedQueryableProvider
    {

        public EFExtendedQueryableProvider(IQueryProvider queryProvider)
            : base(queryProvider)
        {
        }

        protected override IQueryable CreateQueryEx(IQueryable sourse, Type type)
        {
            return (IQueryable)Activator.CreateInstance(typeof(EFExtendedQueryable<>).MakeGenericType(type), new object[] { sourse, this });
        }
    }
}
