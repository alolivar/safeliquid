
// Type: SafeLiquid.TypeResolutionCache




using SafeLiquid.Util;
using System;

namespace SafeLiquid
{
  internal static class TypeResolutionCache
  {
    [ThreadStatic]
    private static WeakTable<Type, TypeResolution> _cache;

    public static WeakTable<Type, TypeResolution> Instance => TypeResolutionCache._cache ?? (TypeResolutionCache._cache = new WeakTable<Type, TypeResolution>(32));
  }
}
