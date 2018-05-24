using LeanCloud.Storage.Internal;
using LeanCloud.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanCloud
{
    public static class PlayExtensions
    {
        public static string ToLog(this Hashtable hashtable)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry pair in hashtable)
            {
                sb.Append(pair.Key.ToString() + " : " + pair.Value.ToString());
            }
            return sb.ToString();
        }

        public static string ToLog<K, V>(this IDictionary<K, V> dictionary)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pair in dictionary)
            {
                var valueLog = pair.Value != null ? pair.Value.ToString() : "null";
                sb.Append(pair.Key.ToString() + " = " + valueLog + "\n");
            }
            return sb.ToString();
        }

        internal static string ToJsonLog<K, V>(this IDictionary<K, V> keyValuePairs)
        {
            return Json.Encode(keyValuePairs);
        }

        internal static IEnumerable<TSource> When<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var result = source.Where(predicate);

            return result == null ? new List<TSource>() : result;
        }

        internal static void Every<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (TSource item in source) { action(item); }
        }

        internal static void Every(this Hashtable source, Action<DictionaryEntry> action)
        {
            foreach (DictionaryEntry de in source)
            {
                action(de);
            }
        }

        internal static IEnumerable<KeyValuePair<K, V>> SmartCombine<K, V>(this IEnumerable<KeyValuePair<K, V>> source, IEnumerable<KeyValuePair<K, V>> toMerge)
        {
            var sourceDic = source.ToDictionary(x => x.Key, x => x.Value);
            var toMergeDic = toMerge.ToDictionary(x => x.Key, x => x.Value);
            return toMergeDic.Concat(sourceDic.Where(x => !sourceDic.Keys.Contains(x.Key)));
        }

        internal static IDictionary<string, object> Merge(this IDictionary<string, object> dataLeft, IDictionary<string, object> dataRight, bool clear = false)
        {
            if (dataRight == null)
                return dataLeft;
            if (dataLeft == null)
                return dataRight;
            foreach (var kv in dataRight)
            {
                if (dataLeft.ContainsKey(kv.Key))
                {
                    dataLeft[kv.Key] = kv.Value;
                }
                else
                {
                    dataLeft.Add(kv);
                }
            }
            return dataLeft;
        }

        internal static bool IsIn<T>(this T @this, params T[] possibles)
        {
            return possibles.Contains(@this);
        }

        internal static IEnumerable<T> GrapEnumerable<K, V, T>(this IDictionary<K, V> source, K key)
        {
            if (source.ContainsKey(key))
            {
                var liobjs = source[key] as List<object>;
                if (liobjs != null)
                {
                    return liobjs.Select(o => (T)(o));
                }
            }
            return null;
        }

        internal static IDictionary<K, V> Filter<K, V>(this IDictionary<K, V> source, IEnumerable<K> filterKeys)
        {
            if (filterKeys == null) return source;
            var filter = source.Where(kv => !filterKeys.Contains(kv.Key));
            return filter.ToDictionary(x => x.Key, x => x.Value);
        }

        internal static IDictionary<K, V> ToDictionary<K, V>(this Hashtable table)
        {
            return table
              .Cast<DictionaryEntry>()
              .ToDictionary(kvp => (K)kvp.Key, kvp => (V)kvp.Value);
        }

        internal static IDictionary<string, object> ToDictionary(this Hashtable table)
        {
            return table.ToDictionary<string, object>();
        }

        internal static Hashtable ToHashtable<K, V>(this IDictionary<K, V> source)
        {
            if (source == null) return null;
            var dictionary = new Dictionary<K, V>(source);
            var hashtable = new Hashtable(dictionary);
            return Hashtable.Synchronized(hashtable);
        }

        internal static IDictionary<K, V> FixKeys<K, V>(this IDictionary<K, V> source, IDictionary<K, K> fixKeyPairs)
        {
            if (fixKeyPairs == null) return source;
            return source.ToDictionary(x =>
            {
                if (fixKeyPairs.ContainsKey(x.Key))
                {
                    return fixKeyPairs[x.Key];
                }
                return x.Key;
            }, x => x.Value);
        }

        internal static T GetValue<T>(this IDictionary<string, object> source, string key, T defaultValue)
        {
            var result = defaultValue;
            if (source.ContainsKey(key))
            {
                try
                {
                    var temp = Conversion.To<T>(source[key]);
                    result = temp;
                }
                catch (InvalidCastException ex)
                {
                    //result = default(T);
                }
            }
            return result;
        }

        internal static void RemoveElement<T>(this AVObject source, string key, T elementToRemove)
        {
            List<object> oldValue = null;
            source.TryGetValue(key, out oldValue);
            if (oldValue != null)
            {
                oldValue.Remove(elementToRemove);
                source[key] = oldValue;
            }
        }
    }
}
