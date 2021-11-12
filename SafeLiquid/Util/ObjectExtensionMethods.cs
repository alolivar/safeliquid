
// Type: SafeLiquid.Util.ObjectExtensionMethods




using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SafeLiquid.Util
{
    public static class ObjectExtensionMethods
    {
        private static HashSet<HashSet<Type>> _BackCompatComparableTypeBoundaries = new HashSet<HashSet<Type>>()
    {
      new HashSet<Type>()
      {
        typeof (Decimal),
        typeof (double),
        typeof (float),
        typeof (int),
        typeof (uint),
        typeof (long),
        typeof (ulong),
        typeof (short),
        typeof (ushort)
      },
      new HashSet<Type>() { typeof (string), typeof (char) }
    };

        public static bool RespondTo(this object value, string member, bool ensureNoParameters = true)
        {
            Type type = value != null ? value.GetType() : throw new ArgumentNullException(nameof(value));
            MethodInfo runtimeMethod = type.GetRuntimeMethod(member, Type.EmptyTypes);
            if (runtimeMethod != (MethodInfo)null && (!ensureNoParameters || !((IEnumerable<ParameterInfo>)runtimeMethod.GetParameters()).Any<ParameterInfo>()))
                return true;
            PropertyInfo runtimeProperty = type.GetRuntimeProperty(member);
            return runtimeProperty != (PropertyInfo)null && runtimeProperty.CanRead;
        }

        public static object Send(this object value, string member, object[] parameters = null)
        {
            Type type = value != null ? value.GetType() : throw new ArgumentNullException(nameof(value));
            MethodInfo runtimeMethod = type.GetRuntimeMethod(member, Type.EmptyTypes);
            if (runtimeMethod != (MethodInfo)null)
                return runtimeMethod.Invoke(value, parameters);
            PropertyInfo runtimeProperty = type.GetRuntimeProperty(member);
            return runtimeProperty != (PropertyInfo)null ? runtimeProperty.GetValue(value, (object[])null) : (object)null;
        }

        public static bool BackCompatSafeTypeInsensitiveEqual(this object value, object otherValue)
        {
            if (value != null && otherValue != null)
            {
                HashSet<Type> comparedTypes = new HashSet<Type>()
        {
          value.GetType(),
          otherValue.GetType()
        };
                if (comparedTypes.Count > 1 && ObjectExtensionMethods._BackCompatComparableTypeBoundaries.All<HashSet<Type>>((Func<HashSet<Type>, bool>)(boundary => !comparedTypes.IsSubsetOf((IEnumerable<Type>)boundary))))
                    return false;
            }
            return value.SafeTypeInsensitiveEqual(otherValue);
        }


        public static bool SafeTypeInsensitiveEqual(this object value, object otherValue)
        {
            // NOTE(David Burg): null values cannot be tested for type, but they can be used for direct comparison.
            if (value == null)
            {
                return value == otherValue;
            }

            // NOTE(David Burg): a is not null so if b is null the values are not equal.
            if (otherValue == null)
            {
                return false;
            }

            // NOTE(David Burg): If both types are the same we can just do a regular comparison
            var aType = value.GetType();
            var bType = otherValue.GetType();
            if (aType == bType)
            {
                // NOTE(David Burg): Use Equals method to allow unboxing. Comparing boxed values with == operator would lead to reference comparison and unexpected results.
                return value.Equals(otherValue);
            }

            // NOTE(David Burg): When types are different we need to try if one can be converted to the other without loss or vice-versa
            // NOTE(David Burg): Order in which conversion is attempted changes the outcome for comparison between string and bool.
            // It's out of Shopify spec compliance but so for backward compatibility.
            try
            {
                return Convert.ChangeType(otherValue, aType).Equals(value);
            }
            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
            {
                try
                {
                    return Convert.ChangeType(value, bType).Equals(otherValue);
                }
                catch (Exception ex2) when (ex2 is InvalidCastException || ex2 is FormatException || ex2 is OverflowException)
                {
                    // NOTE(David Burg): Types are not the same and we can't convert so the values cannot be the same.
                    return false;
                }
            }
        }


        public static bool IsTruthy(this object any) => !any.IsFalsy();

        public static bool IsFalsy(this object any)
        {
            object obj;
            if (any == null || (obj = any) is bool && !(bool)obj)
                return true;
            return any is string str && "false".Equals(str, StringComparison.OrdinalIgnoreCase);
        }
    }
}
