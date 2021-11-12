
// Type: SafeLiquid.Util.TypeUtility




using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SafeLiquid.Util
{
  internal static class TypeUtility
  {
    private const TypeAttributes AnonymousTypeAttributes = TypeAttributes.NotPublic;

    public static bool IsAnonymousType(Type t) => t.GetTypeInfo().GetCustomAttribute<CompilerGeneratedAttribute>() != null && t.GetTypeInfo().IsGenericType && (t.Name.Contains("AnonymousType") || t.Name.Contains("AnonType")) && (t.Name.StartsWith("<>") || t.Name.StartsWith("VB$")) && (t.GetTypeInfo().Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
  }
}
