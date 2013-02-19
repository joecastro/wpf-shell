
namespace Standard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Further LINQ extensions
    /// </summary>
    internal static class Enumerable2
    {
        // Unnecessary in .Net 4.
        //public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> func)
        //{
        //    Verify.IsNotNull(first, "first");
        //    Verify.IsNotNull(second, "second");

        //    return _Zip(first, second, func);
        //}

        //private static IEnumerable<TResult> _Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> func)
        //{
        //    IEnumerator<TFirst> ie1 = first.GetEnumerator();
        //    IEnumerator<TSecond> ie2 = second.GetEnumerator();
        //    while (ie1.MoveNext() && ie2.MoveNext())
        //    {
        //        yield return func(ie1.Current, ie2.Current);
        //    }
        //}

        /// <summary>Partition a collection into two, based on whether the items match a predicate.</summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="collection">The original collection to split.</param>
        /// <param name="condition">The condition to use for the split.</param>
        /// <param name="rest">A collection of all items in the original collection that do not satisfy the condition.</param>
        /// <returns>A collection of all items in the original collection that satisfy the condition.</returns>
        /// <remarks>Unlike most extension methods of this nature, this does not perform the operation lazily.</remarks>
        public static IEnumerable<T> SplitWhere<T>(this IEnumerable<T> collection, Predicate<T> condition, out IEnumerable<T> rest)
        {
            Verify.IsNotNull(collection, "collection");
            Verify.IsNotNull(condition, "condition");

            var passList = new List<T>();
            var failList = new List<T>();

            foreach (T t in collection)
            {
                if (condition(t))
                {
                    passList.Add(t);
                }
                else
                {
                    failList.Add(t);
                }
            }

            rest = failList;
            return passList;
        }

        /// <summary>
        /// Limit an enumeration to be constrained to a subset after a given index.
        /// </summary>
        /// <typeparam name="T">The type of items being enumerated.</typeparam>
        /// <param name="enumerable">The collection to be enumerated.</param>
        /// <param name="startIndex">The index (inclusive) of the first item to be returned.</param>
        /// <returns></returns>
        public static IEnumerable<T> Sublist<T>(this IEnumerable<T> enumerable, int startIndex)
        {
            return Sublist(enumerable, startIndex, null);
        }

        /// <summary>
        /// Limit an enumeration to be within a set of indices.
        /// </summary>
        /// <typeparam name="T">The type of items being enumerated.</typeparam>
        /// <param name="enumerable">The collection to be enumerated.</param>
        /// <param name="startIndex">The index (inclusive) of the first item to be returned.</param>
        /// <param name="endIndex">
        /// The index (exclusive) of the last item to be returned.
        /// If this is null then the full collection after startIndex is returned.
        /// If this is greater than the count of the collection after startIndex, then the full collection after startIndex is returned.
        /// </param>
        /// <returns></returns>
        public static IEnumerable<T> Sublist<T>(this IEnumerable<T> enumerable, int startIndex, int? endIndex)
        {
            Verify.IsNotNull(enumerable, "enumerable");
            Verify.BoundedInteger(0, startIndex, int.MaxValue, "startIndex");
            if (endIndex != null)
            {
                Verify.BoundedInteger(startIndex, endIndex.Value, int.MaxValue, "endIndex");
            }

            // If this supports indexing then just use that.
            var list = enumerable as IList<T>;
            if (list != null)
            {
                return _SublistList(list, startIndex, endIndex);
            }

            return _SublistEnum(enumerable, startIndex, endIndex);
        }

        private static IEnumerable<T> _SublistEnum<T>(this IEnumerable<T> enumerable, int startIndex, int? endIndex)
        {
            int currentIndex = 0;
            IEnumerator<T> enumerator = enumerable.GetEnumerator();
            while (currentIndex < startIndex && enumerator.MoveNext())
            {
                ++currentIndex;
            }

            int trueEndIndex = endIndex ?? int.MaxValue;

            while (currentIndex < trueEndIndex && enumerator.MoveNext())
            {
                yield return enumerator.Current;
                ++currentIndex;
            }
        }

        private static IEnumerable<T> _SublistList<T>(this IList<T> list, int startIndex, int? endIndex)
        {
            int trueEndIndex = Math.Min(list.Count, endIndex ?? int.MaxValue);
            for (int i = startIndex; i < trueEndIndex; ++i)
            {
                yield return list[i];
            }
        }

        public static bool AreSorted<T>(this IEnumerable<T> enumerable)
        {
            return _AreSorted(enumerable);
        }

        public static bool AreSorted<T>(this IEnumerable<T> enumerable, Comparison<T> comparison)
        {
            Verify.IsNotNull(enumerable, "enumerable");
            if (comparison == null)
            {
                if (typeof(T).GetInterface(typeof(IComparable<T>).Name) == null)
                {
                    // Not comparable for a sort.
                    return true;
                }

                return _AreSorted(enumerable);
            }

            return _AreSorted(enumerable, comparison);
        }

        private static bool _AreSorted<T>(IEnumerable<T> enumerable, Comparison<T> comparison)
        {
            T last = default(T);
            bool isFirst = true;
            foreach (var item in enumerable)
            {
                if (isFirst)
                {
                    last = item;
                    isFirst = false;
                }
                else
                {
                    if (comparison(last, item) > 0)
                    {
                        return false;
                    }
                    last = item;
                } 
            }

            return true;
        }

        private static bool _AreSorted<T>(IEnumerable<T> enumerable)
        {
            IComparable<T> last = null;
            bool isFirstNonNull = true;
            foreach (var item in enumerable)
            {
                if (isFirstNonNull)
                {
                    last = (IComparable<T>)item;
                    if (last != null)
                    {
                        isFirstNonNull = false;
                    }
                }
                else
                {
                    if (last.CompareTo(item) > 0)
                    {
                        return false;
                    }
                    last = (IComparable<T>)item;
                    if (last == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void AddRange<T>(this ICollection<T> collection, params T[] items)
        {
            Verify.IsNotNull(collection, "collection");
            _AddRange(collection, items);
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            Verify.IsNotNull(collection, "collection");
            _AddRange(collection, items);
        }

        private static void _AddRange<T>(ICollection<T> collection, IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static IEnumerable<T> Reverse<T>(this IList<T> list)
        {
            Verify.IsNotNull(list, "list");
            return _Reverse(list);
        }

        private static IEnumerable<T> _Reverse<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i >= 0; --i)
            {
                yield return list[i];
            }
        }

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            var r = new Random();
            return Shuffle(list, () => r.Next(list.Count));
        }

        public static IList<T> Shuffle<T>(this IList<T> list, Func<int> numberGenerator)
        {
            Verify.IsNotNull(list, "list");
            Verify.IsNotNull(numberGenerator, "numberGenerator");

            var swapIndices = new int[list.Count];
            for (int i = 0; i < list.Count; ++i)
            {
                int j = numberGenerator();
                if (j < 0 || j >= list.Count)
                {
                    throw new ArgumentException("The number generator function generated a number outside the valid range.");
                }
                swapIndices[i] = j;
            }
            return _Shuffle(list, swapIndices);
        }

        private static IList<T> _Shuffle<T>(IList<T> list, int[] swapIndices)
        {
            Assert.AreEqual(list.Count, swapIndices.Length);
            for (int i = swapIndices.Length; i > 1; --i)
            {
                int k = swapIndices[i-1];
                T temp = list[k];
                list[k] = list[i-1];
                list[i-1] = temp;
            }

            return list;
        }
    }
}
