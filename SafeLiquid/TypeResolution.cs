
// Type: SafeLiquid.TypeResolution




using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SafeLiquid
{
  internal class TypeResolution
  {
    public Dictionary<string, MethodInfo> CachedMethods { get; private set; }

    public Dictionary<string, PropertyInfo> CachedProperties { get; private set; }

    public TypeResolution(Type type, Func<MemberInfo, bool> filterMemberCallback)
    {
      this.CachedMethods = this.GetMemberDictionary<MethodInfo>(TypeResolution.GetMethodsWithoutDuplicateNames(type, (Func<MethodInfo, bool>) (mi => mi.GetParameters().Length == 0)), (Func<MethodInfo, bool>) (mi => filterMemberCallback((MemberInfo) mi)));
      this.CachedProperties = this.GetMemberDictionary<PropertyInfo>(TypeResolution.GetPropertiesWithoutDuplicateNames(type), (Func<PropertyInfo, bool>) (mi => filterMemberCallback((MemberInfo) mi)));
    }

    private Dictionary<string, T> GetMemberDictionary<T>(
      IEnumerable<T> members,
      Func<T, bool> filterMemberCallback)
      where T : MemberInfo
    {
      return members.Where<T>(filterMemberCallback).ToDictionary<T, string>((Func<T, string>) (mi => Template.NamingConvention.GetMemberName(mi.Name)), (IEqualityComparer<string>) Template.NamingConvention.StringComparer);
    }

    private static IEnumerable<PropertyInfo> GetPropertiesWithoutDuplicateNames(
      Type type,
      Func<PropertyInfo, bool> predicate = null)
    {
      return TypeResolution.GetMembersWithoutDuplicateNames(predicate != null ? (ICollection<MemberInfo>) type.GetRuntimeProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.CanRead && p.GetMethod.IsPublic && !p.GetMethod.IsStatic)).Where<PropertyInfo>(predicate).Cast<MemberInfo>().ToList<MemberInfo>() : (ICollection<MemberInfo>) type.GetRuntimeProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.CanRead && p.GetMethod.IsPublic && !p.GetMethod.IsStatic)).Cast<MemberInfo>().ToList<MemberInfo>()).Cast<PropertyInfo>();
    }

    private static IEnumerable<MethodInfo> GetMethodsWithoutDuplicateNames(
      Type type,
      Func<MethodInfo, bool> predicate = null)
    {
      return TypeResolution.GetMembersWithoutDuplicateNames(predicate != null ? (ICollection<MemberInfo>) type.GetRuntimeMethods().Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.IsPublic && !m.IsStatic)).Where<MethodInfo>(predicate).Cast<MemberInfo>().ToList<MemberInfo>() : (ICollection<MemberInfo>) type.GetRuntimeMethods().Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.IsPublic && !m.IsStatic)).Cast<MemberInfo>().ToList<MemberInfo>()).Cast<MethodInfo>();
    }

    private static IEnumerable<MemberInfo> GetMembersWithoutDuplicateNames(
      ICollection<MemberInfo> members)
    {
      foreach (IGrouping<string, MemberInfo> source in members.GroupBy<MemberInfo, string>((Func<MemberInfo, string>) (x => x.Name)).Where<IGrouping<string, MemberInfo>>((Func<IGrouping<string, MemberInfo>, bool>) (g => g.Count<MemberInfo>() > 1)))
      {
        List<MemberInfo> list = source.Select<MemberInfo, MemberInfo>((Func<MemberInfo, MemberInfo>) (g => g)).ToList<MemberInfo>();
        List<Type> declaringTypes = list.Select<MemberInfo, Type>((Func<MemberInfo, Type>) (d => d.DeclaringType)).ToList<Type>();
        Type type = declaringTypes.Single<Type>((Func<Type, bool>) (t => !declaringTypes.Any<Type>((Func<Type, bool>) (o => t.GetTypeInfo().IsAssignableFrom(o.GetTypeInfo()) && o != t))));
        foreach (MemberInfo memberInfo in list)
        {
          if (memberInfo.DeclaringType != type)
            members.Remove(memberInfo);
        }
      }
      return (IEnumerable<MemberInfo>) members;
    }
  }
}
