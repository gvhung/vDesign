using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework
{
    public static class AsyncExtensions
    {
        public static async Task<TState> AggregateAsync<T, TState>(this IEnumerable<T> items, TState initial, Func<TState, T, Task<TState>> makeTask)
        {
            var state = initial;

            foreach (var item in items)
                state = await makeTask(state, item);

            return state;
        }
    }
}