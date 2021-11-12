
// Type: SafeLiquid.DictionaryExtensions




using System;
using System.Collections.Generic;

namespace SafeLiquid
{
  internal static class DictionaryExtensions
  {
    public static V TryAdd<K, V>(this IDictionary<K, V> dic, K key, Func<V> factory)
    {
      V v;
      return !dic.TryGetValue(key, out v) ? (dic[key] = factory()) : v;
    }
  }
}
