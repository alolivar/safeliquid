
// Type: SafeLiquid.Util.ListExtensionMethods




using System.Collections.Generic;

namespace SafeLiquid.Util
{
  public static class ListExtensionMethods
  {
    public static T TryGetAtIndex<T>(this List<T> list, int index) where T : class => list != null && list.Count > index && index >= 0 ? list[index] : default (T);

    public static T TryGetAtIndexReverse<T>(this List<T> list, int rindex) where T : class => list != null && list.Count > rindex && rindex >= 0 ? list[list.Count - 1 - rindex] : default (T);

    public static T Shift<T>(this List<T> list) where T : class
    {
      if (list == null || list.Count == 0)
        return default (T);
      T obj = list[0];
      list.RemoveAt(0);
      return obj;
    }

    public static T Pop<T>(this List<T> list) where T : class
    {
      if (list == null || list.Count == 0)
        return default (T);
      T obj = list[list.Count - 1];
      list.RemoveAt(list.Count - 1);
      return obj;
    }
  }
}
