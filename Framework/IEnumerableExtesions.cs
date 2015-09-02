using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Framework.EnumerableExtesions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<IRecursion<T>> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            return SelectRecursive(source, selector, null);
        }

        public static IEnumerable<IRecursion<T>> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector, Func<IRecursion<T>, bool> predicate)
        {
            return SelectRecursive(source, selector, predicate, 0);
        }
        private static IEnumerable<IRecursion<T>> SelectRecursive<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> selector, Func<IRecursion<T>, bool> predicate, int depth)
        {
            var q = source
                .Select(item => new Recursion<T>(depth, item))
                .Cast<IRecursion<T>>();
            if (predicate != null)
                q = q.Where(predicate);
            foreach (var item in q)
            {
                yield return item;
                foreach (var item2 in SelectRecursive(selector(item.Item), selector, predicate, depth + 1))
                    yield return item2;
            }
        }

        private class Recursion<T> : IRecursion<T>
        {
            private int _depth;
            private T _item;
            public int Depth
            {
                get { return _depth; }
            }
            public T Item
            {
                get { return _item; }
            }
            public Recursion(int depth, T item)
            {
                _depth = depth;
                _item = item;
            }
        }

        public static SmartEnumerable<T> AsSmartEnumerable<T>(this IEnumerable<T> source)
        {
            return new SmartEnumerable<T>(source);
        }
    }

    /// <summary>
    /// Represents an item in a recursive projection.
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    public interface IRecursion<T>
    {
        /// <summary>
        /// Gets the recursive depth.
        /// </summary>
        /// <value>The depth.</value>
        int Depth { get; }
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        T Item { get; }
    }

    public static class SmartEnumerable
    {
        /// <summary>
        /// Extension method to make life easier.
        /// </summary>
        /// <typeparam name="T">Type of enumerable</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <returns>A new SmartEnumerable of the appropriate type</returns>
        public static SmartEnumerable<T> Create<T>(IEnumerable<T> source)
        {
            return new SmartEnumerable<T>(source);
        }
    }

    /// <summary>
    /// Type chaining an IEnumerable&lt;T&gt; to allow the iterating code
    /// to detect the first and last entries simply.
    /// </summary>
    /// <typeparam name="T">Type to iterate over</typeparam>
    public class SmartEnumerable<T> : IEnumerable<SmartEnumerable<T>.Entry>
    {
        /// <summary>
        /// Enumerable we proxy to
        /// </summary>
        readonly IEnumerable<T> enumerable;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="enumerable">Collection to enumerate. Must not be null.</param>
        public SmartEnumerable(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }
            this.enumerable = enumerable;
        }

        /// <summary>
        /// Returns an enumeration of Entry objects, each of which knows
        /// whether it is the first/last of the enumeration, as well as the
        /// current value.
        /// </summary>
        public IEnumerator<Entry> GetEnumerator()
        {
            using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    yield break;
                }
                bool isFirst = true;
                bool isLast = false;
                int index = 0;
                while (!isLast)
                {
                    T current = enumerator.Current;
                    isLast = !enumerator.MoveNext();
                    yield return new Entry(isFirst, isLast, current, index++);
                    isFirst = false;
                }
            }
        }

        /// <summary>
        /// Non-generic form of GetEnumerator.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Represents each entry returned within a collection,
        /// containing the value and whether it is the first and/or
        /// the last entry in the collection's. enumeration
        /// </summary>
        public class Entry
        {
            readonly bool isFirst;
            readonly bool isLast;
            readonly T value;
            readonly int index;

            internal Entry(bool isFirst, bool isLast, T value, int index)
            {
                this.isFirst = isFirst;
                this.isLast = isLast;
                this.value = value;
                this.index = index;
            }

            /// <summary>
            /// The value of the entry.
            /// </summary>
            public T Value { get { return value; } }

            /// <summary>
            /// Whether or not this entry is first in the collection's enumeration.
            /// </summary>
            public bool IsFirst { get { return isFirst; } }

            /// <summary>
            /// Whether or not this entry is last in the collection's enumeration.
            /// </summary>
            public bool IsLast { get { return isLast; } }

            /// <summary>
            /// The 0-based index of this entry (i.e. how many entries have been returned before this one)
            /// </summary>
            public int Index { get { return index; } }
        }
    }
}
