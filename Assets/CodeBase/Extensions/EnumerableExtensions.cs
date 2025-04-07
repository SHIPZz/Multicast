using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;

            if (enumerable is ICollection<T> collection)
                return collection.Count == 0;

            return !enumerable.Any();
        }

        public static bool IsNullOrEmpty(this string str) =>
            !string.IsNullOrWhiteSpace(str);

        public static T PickRandom<T>(this IEnumerable<T> collection)
        {
            T[] enumerable = collection as T[] ?? collection.ToArray();
            return enumerable[Random.Range(0, enumerable.Length)];
        }

        public static HashSet<T> AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                hashSet.Add(item);
            }

            return hashSet;
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T toExcept) =>
            enumerable.Except(new[] { toExcept });

        public static T ElementAtOrFirst<T>(this T[] array, int index) =>
            index < array.Length ? array[index] : array[0];

        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> self) => self ?? Enumerable.Empty<T>();

        public static IEnumerable<T> NoNulls<T>(this IEnumerable<T> self) => self.Where(element => element != null);

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var array = source.ToArray();
            
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }

            return array;
        }
    }
}