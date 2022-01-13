
// Type: SafeLiquid.DropBase




using SafeLiquid.NamingConventions;
using System;
using System.Reflection;

namespace SafeLiquid
{
  public abstract class DropBase : ILiquidizable, IIndexable, IContextAware
  {
    internal TypeResolution TypeResolution
    {
      get
      {
        Type type = this.GetObject().GetType();
        TypeResolution typeResolution;
        if (!TypeResolutionCache.Instance.TryGetValue(type, out typeResolution))
          TypeResolutionCache.Instance[type] = typeResolution = this.CreateTypeResolution(type);
        return typeResolution;
      }
    }

    public Context Context { get; set; }

    public virtual object this[object method] => this.InvokeDrop(method);

    public virtual bool ContainsKey(object name) => true;

    public virtual object ToLiquid() => (object) this;

    internal abstract object GetObject();

    internal abstract TypeResolution CreateTypeResolution(Type type);

    public virtual object BeforeMethod(string method)
    {
      if (Template.NamingConvention is RubyNamingConvention)
      {
        string memberName = Template.NamingConvention.GetMemberName(method);
        if (this.TypeResolution.CachedMethods.TryGetValue(memberName, out MethodInfo _) || this.TypeResolution.CachedProperties.TryGetValue(memberName, out PropertyInfo _))
          return (object) string.Format(Liquid.ResourceManager.GetString("DropWrongNamingConventionMessage"), (object) memberName);
      }
      return (object) null;
    }

    public object InvokeDrop(object name)
    {
      string str = (string) name;
      MethodInfo methodInfo;
      if (this.TypeResolution.CachedMethods.TryGetValue(str, out methodInfo))
        return methodInfo.Invoke(this.GetObject(), (object[]) null);
      PropertyInfo propertyInfo;
      return this.TypeResolution.CachedProperties.TryGetValue(str, out propertyInfo) ? propertyInfo.GetValue(this.GetObject(), (object[]) null) : this.BeforeMethod(str);
    }
  }
}
