
// Type: SafeLiquid.Drop




using System;
using System.Reflection;

namespace SafeLiquid
{
  public abstract class Drop : DropBase
  {
    internal override object GetObject() => (object) this;

    internal override TypeResolution CreateTypeResolution(Type type) => new TypeResolution(type, (Func<MemberInfo, bool>) (mi => mi.DeclaringType.GetTypeInfo().BaseType != (Type) null && typeof (Drop).GetTypeInfo().IsAssignableFrom(mi.DeclaringType.GetTypeInfo().BaseType.GetTypeInfo())));
  }
}
