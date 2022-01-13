
// Type: SafeLiquid.Util.EnumerableExtensionMethods




using System;
using System.Collections;
using System.Collections.Generic;

namespace SafeLiquid.Util
{
  public static class EnumerableExtensionMethods
  {
    public static IEnumerable Flatten(this IEnumerable array)
    {
      foreach (object obj1 in array)
      {
        switch (obj1)
        {
          case IEnumerable _:
            foreach (object obj2 in ((IEnumerable) obj1).Flatten())
              yield return obj2;
            continue;
          default:
            yield return obj1;
            continue;
        }
      }
    }

    public static void EachWithIndex(this IEnumerable<object> array, Action<object, int> callback)
    {
      int num = 0;
      foreach (object obj in array)
      {
        callback(obj, num);
        ++num;
      }
    }
  }
}
