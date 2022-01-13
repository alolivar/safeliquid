
// Type: SafeLiquid.StandardFilters




using SafeLiquid.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

namespace SafeLiquid
{
    public static class StandardFilters
    {

        public static readonly TimeSpan RegexTimeOut = TimeSpan.FromSeconds(10.0);


        public static int Size(object input)
        {
            switch (input)
            {
                case string str:
                    return str.Length;
                case IEnumerable source:
                    return source.Cast<object>().Count<object>();
                default:
                    return 0;
            }
        }

        public static string Slice(string input, long start, long len = 1)
        {
            if (input == null || start > (long)input.Length)
                return (string)null;
            if (start < 0L)
            {
                start += (long)input.Length;
                if (start < 0L)
                {
                    len = Math.Max(0L, len + start);
                    start = 0L;
                }
            }
            if (start + len > (long)input.Length)
                len = (long)input.Length - start;
            return input.Substring(Convert.ToInt32(start), Convert.ToInt32(len));
        }

        public static string Downcase(string input) => input != null ? input.ToLower() : input;

        public static string Upcase(string input) => input != null ? input.ToUpper() : input;

#pragma warning disable CA1055 // Uri return values should not be strings
        public static string UrlEncode(string input) => input != null ? WebUtility.UrlEncode(input) : input;
#pragma warning restore CA1055 // Uri return values should not be strings

#pragma warning disable CA1055 // Uri return values should not be strings
        public static string UrlDecode(string input) => input != null ? WebUtility.UrlDecode(input) : input;
#pragma warning restore CA1055 // Uri return values should not be strings

        public static string Capitalize(Context context, string input)
        {
            if (input.IsNullOrWhiteSpace())
                return input;
            if (context.SyntaxCompatibilityLevel >= SyntaxCompatibility.DotLiquid21)
            {
                string str = input.TrimStart();
                return input.Substring(0, input.Length - str.Length) + char.ToUpper(str[0]).ToString() + str.Substring(1);
            }
            return !string.IsNullOrEmpty(input) ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input) : input;
        }

        public static string Escape(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            try
            {
                return WebUtility.HtmlEncode(input);
            }
            catch
            {
                return input;
            }
        }

        public static string EscapeOnce(string input) => !string.IsNullOrEmpty(input) ? WebUtility.HtmlEncode(WebUtility.HtmlDecode(input)) : input;

        public static string H(string input) => StandardFilters.Escape(input);

        public static string Truncate(string input, int length = 50, string truncateString = "...")
        {
            if (string.IsNullOrEmpty(input))
                return input;
            if (length < 0)
                return truncateString;
            int num = length - truncateString.Length;
            return input.Length <= length ? input : input.Substring(0, num < 0 ? 0 : num) + truncateString;
        }

        public static string TruncateWords(string input, int words = 15, string truncateString = "...")
        {
            if (string.IsNullOrEmpty(input))
                return input;
            if (words <= 0)
                return truncateString;
            string[] strArray = input.Split(' ');
            return strArray.Length <= words ? input : string.Join(" ", ((IEnumerable<string>)strArray).Take<string>(words)) + truncateString;
        }

        public static string[] Split(string input, string pattern)
        {
            if (input.IsNullOrWhiteSpace())
                return new[] { input };
            return string.IsNullOrEmpty(pattern)
                ? input.ToCharArray().Select(character => character.ToString()).ToArray()
                : input.Split(new[] { pattern }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string StripHtml(string input) => !input.IsNullOrWhiteSpace() ? Regex.Replace(input, "<.*?>", string.Empty, RegexOptions.None, RegexTimeOut) : input;

        public static string Strip(string input) => input?.Trim();

        public static string Lstrip(string input) => input?.TrimStart();

        public static string Rstrip(string input) => input?.TrimEnd();

        public static string Currency(object input, string cultureInfo = null)
        {
            Decimal result;
            if (!Decimal.TryParse(input.ToString(), out result))
                return input.ToString();
            if (cultureInfo.IsNullOrWhiteSpace())
                cultureInfo = CultureInfo.CurrentCulture.Name;
            CultureInfo cultureInfo1 = new CultureInfo(cultureInfo);
            return result.ToString("C", (IFormatProvider)cultureInfo1);
        }

        public static string StripNewlines(string input) => !input.IsNullOrWhiteSpace() ? Regex.Replace(input, "(\\r?\\n)", string.Empty, RegexOptions.None, RegexTimeOut) : input;

        public static string Join(IEnumerable input, string glue = " ")
        {
            if (input == null)
                return (string)null;
            IEnumerable<object> values = input.Cast<object>();
            return string.Join<object>(glue, values);
        }

        public static IEnumerable Sort(object input, string property = null)
        {
            List<object> source1;
            switch (input)
            {
                case null:
                    return (IEnumerable)null;
                case IEnumerable<Hash> source2 when !string.IsNullOrEmpty(property):
                    source1 = source2.Cast<object>().ToList<object>();
                    break;
                case IEnumerable array:
                    source1 = array.Flatten().Cast<object>().ToList<object>();
                    break;
                default:
                    source1 = new List<object>((IEnumerable<object>)new object[1]
                    {
            input
                    });
                    break;
            }
            if (!source1.Any<object>())
                return (IEnumerable)source1;
            if (string.IsNullOrEmpty(property))
                source1.Sort();
            else if (source1.All<object>((Func<object, bool>)(o => o is IDictionary)) && source1.Any<object>((Func<object, bool>)(o => ((IDictionary)o).Contains((object)property))))
                source1.Sort((Comparison<object>)((a, b) => System.Collections.Generic.Comparer<object>.Default.Compare(((IDictionary)a)[(object)property], ((IDictionary)b)[(object)property])));
            else if (source1.All<object>((Func<object, bool>)(o => o.RespondTo(property))))
                source1.Sort((Comparison<object>)((a, b) => System.Collections.Generic.Comparer<object>.Default.Compare(a.Send(property), b.Send(property))));
            return (IEnumerable)source1;
        }

        public static IEnumerable Map(IEnumerable enumerableInput, string property)
        {
            if (enumerableInput == null)
                return null;

            // Enumerate to a list so we can repeatedly parse through the collection.
            List<object> listedInput = enumerableInput.Cast<object>().ToList();

            // If the list happens to be empty we are done already.
            if (!listedInput.Any())
                return listedInput;

            // Note that liquid assumes that contained complex elements are all following the same schema.
            // Hence here we only check if the first element has the property requested for the map.
            if (listedInput.All(element => element is IDictionary)
                && ((IDictionary)listedInput.First()).Contains(key: property))
                return listedInput.Select(element => ((IDictionary)element)[property]);

            return listedInput
                .Select(selector: element =>
                {
                    if (element == null)
                        return null;

                    var indexable = element as IIndexable;
                    if (indexable == null)
                    {
                        var type = element.GetType();
                        /*var safeTypeTransformer = Template.GetSafeTypeTransformer(type);
                        if (safeTypeTransformer != null)
                        {
                            indexable = safeTypeTransformer(element) as DropBase;
                        }
                        else
                        {*/
                        var liquidTypeAttribute = type
                            .GetTypeInfo()
                            .GetCustomAttributes(attributeType: typeof(LiquidTypeAttribute), inherit: false)
                            .FirstOrDefault() as LiquidTypeAttribute;
                        if (liquidTypeAttribute != null)
                        {
                            indexable = new DropProxy(element, liquidTypeAttribute.AllowedMembers);
                        }
                        else if (TypeUtility.IsAnonymousType(type))
                        {
                            return element.RespondTo(property) ? element.Send(property) : element;
                        }
                        //}
                    }

                    return (indexable?.ContainsKey(property) ?? false) ? indexable[property] : null;
                });
        }


        public static string Replace(
          Context context,
          string input,
          string theString,
          string replacement = "")
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(theString))
                return input;
            return context.SyntaxCompatibilityLevel >= SyntaxCompatibility.DotLiquid21 ? input.Replace(theString, replacement) : Regex.Replace(input, theString, replacement, RegexOptions.None, RegexTimeOut);
        }

        public static string ReplaceFirst(
          Context context,
          string input,
          string theString,
          string replacement = "")
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(theString))
                return input;
            if (context.SyntaxCompatibilityLevel >= SyntaxCompatibility.DotLiquid21)
            {
                int startIndex = input.IndexOf(theString);
                return startIndex >= 0 ? input.Remove(startIndex, theString.Length).Insert(startIndex, replacement) : input;
            }
            bool doneReplacement = false;
            return Regex.Replace(input, theString, (MatchEvaluator)(m =>
           {
               if (doneReplacement)
                   return m.Value;
               doneReplacement = true;
               return replacement;
           }), RegexOptions.None, RegexTimeOut);
        }

        public static string Remove(string input, string theString) => !input.IsNullOrWhiteSpace() ? input.Replace(theString, string.Empty) : input;

        public static string RemoveFirst(Context context, string input, string theString) => !input.IsNullOrWhiteSpace() ? StandardFilters.ReplaceFirst(context, input, theString, string.Empty) : input;

        public static string Append(string input, string theString) => input != null ? input + theString : input;

        public static string Prepend(string input, string theString) => input != null ? theString + input : input;

        public static string NewlineToBr(string input) => !input.IsNullOrWhiteSpace() ? Regex.Replace(input, "(\\r?\\n)", "<br />$1", RegexOptions.None, RegexTimeOut) : input;

        public static string Date(Context context, object input, string format)
        {
            if (input == null)
                return (string)null;
            if (input is DateTime dateTime)
            {
                if (format.IsNullOrWhiteSpace())
                    return dateTime.ToString();
                if (!Liquid.UseRubyDateFormat)
                    return dateTime.ToString(format);
                return context.SyntaxCompatibilityLevel < SyntaxCompatibility.DotLiquid21 ? dateTime.ToStrFTime(format) : new DateTimeOffset(dateTime).ToStrFTime(format);
            }
            if (context.SyntaxCompatibilityLevel == SyntaxCompatibility.DotLiquid20)
                return StandardFilters.DateLegacyParsing(input.ToString(), format);
            if (format.IsNullOrWhiteSpace())
                return input.ToString();
            DateTimeOffset result;
            switch (input)
            {
                case DateTimeOffset dateTimeOffset:
                    result = dateTimeOffset;
                    break;
                case Decimal _:
                case double _:
                case float _:
                case int _:
                case uint _:
                case long _:
                case ulong _:
                case short _:
                case ushort _:
                    result = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddSeconds(Convert.ToDouble(input)).ToLocalTime();
                    break;
                default:
                    string str = input.ToString();
                    if (string.Equals(str, "now", StringComparison.OrdinalIgnoreCase) || string.Equals(str, "today", StringComparison.OrdinalIgnoreCase))
                    {
                        result = DateTimeOffset.Now;
                        break;
                    }
                    if (!DateTimeOffset.TryParse(str, out result))
                        return str;
                    break;
            }
            return !Liquid.UseRubyDateFormat ? result.ToString(format) : result.ToStrFTime(format);
        }

        private static string DateLegacyParsing(string value, string format)
        {
            DateTime result;
            if (string.Equals(value, "now", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "today", StringComparison.OrdinalIgnoreCase))
            {
                result = DateTime.Now;
                if (format.IsNullOrWhiteSpace())
                    return result.ToString();
            }
            else if (!DateTime.TryParse(value, out result))
            {
                return value;
            }

            if (format.IsNullOrWhiteSpace())
                return value;
            return !Liquid.UseRubyDateFormat ? result.ToString(format) : result.ToStrFTime(format);
        }

        public static object First(IEnumerable array) => array == null ? (object)null : array.Cast<object>().FirstOrDefault<object>();

        public static object Last(IEnumerable array) => array == null ? (object)null : array.Cast<object>().LastOrDefault<object>();

        public static object Plus(Context context, object input, object operand)
        {
            if (context.SyntaxCompatibilityLevel >= SyntaxCompatibility.DotLiquid21)
                return StandardFilters.DoMathsOperation(context, input, operand, new Func<Expression, Expression, BinaryExpression>(Expression.AddChecked));
            return !(input is string) ? StandardFilters.DoMathsOperation(context, input, operand, new Func<Expression, Expression, BinaryExpression>(Expression.AddChecked)) : (object)(input.ToString() + operand);
        }

        public static object Minus(Context context, object input, object operand) => StandardFilters.DoMathsOperation(context, input, operand, new Func<Expression, Expression, BinaryExpression>(Expression.SubtractChecked));

        public static object Times(Context context, object input, object operand)
        {
            if (context.SyntaxCompatibilityLevel >= SyntaxCompatibility.DotLiquid21)
                return StandardFilters.DoMathsOperation(context, input, operand, new Func<Expression, Expression, BinaryExpression>(Expression.MultiplyChecked));
            if (input is string)
            {
                switch (operand)
                {
                    case int _:
                    case long _:
                        return (object)Enumerable.Repeat<string>((string)input, Convert.ToInt32(operand));
                }
            }
            return StandardFilters.DoMathsOperation(context, input, operand, new Func<Expression, Expression, BinaryExpression>(Expression.MultiplyChecked));
        }

        public static object Round(object input, object places = null)
        {
            try
            {
                int decimals = places == null ? 0 : Convert.ToInt32(places);
                return (object)Math.Round(Convert.ToDecimal(input), decimals);
            }
            catch (Exception ex)
            {
                return (object)null;
            }
        }

        public static object Ceil(object input)
        {
            Decimal result;
            return Decimal.TryParse(input.ToString(), out result) ? (object)Math.Ceiling(result) : (object)null;
        }

        public static object Floor(object input)
        {
            Decimal result;
            return Decimal.TryParse(input.ToString(), out result) ? (object)Math.Floor(result) : (object)null;
        }

        public static object DividedBy(Context context, object input, object operand) => StandardFilters.DoMathsOperation(context, input, operand, new Func<Expression, Expression, BinaryExpression>(Expression.Divide));

        public static object Modulo(Context context, object input, object operand) => StandardFilters.DoMathsOperation(context, input, operand, new Func<Expression, Expression, BinaryExpression>(Expression.Modulo));

        public static string Default(string input, string defaultValue) => string.IsNullOrWhiteSpace(input) ? defaultValue : input;

        private static bool IsReal(object o)
        {
            switch (o)
            {
                case double _:
                case float _:
                    return true;
                default:
                    return o is Decimal;
            }
        }


        private static object DoMathsOperation(Context context, object input, object operand, Func<Expression, Expression, BinaryExpression> operation)
        {
            if (input == null || operand == null)
                return null;

            // NOTE(David Burg): Try for maximal precision if the input and operand fit the decimal's range.
            // This avoids rounding errors in financial arithmetic.
            // E.g.: 0.1 | Plus 10 | Minus 10 to remain 0.1, not 0.0999999999999996
            // Otherwise revert to maximum range (possible precision loss).
            var shouldConvertStrings = context.SyntaxCompatibilityLevel >= SyntaxCompatibility.DotLiquid21 && ((input is string) || (operand is string));
            if (IsReal(input) || IsReal(operand) || shouldConvertStrings)
            {
                try
                {
                    input = Convert.ToDecimal(input);
                    operand = Convert.ToDecimal(operand);

                    return ExpressionUtility
                        .CreateExpression(
                            body: operation,
                            leftType: input.GetType(),
                            rightType: operand.GetType())
                        .DynamicInvoke(input, operand);
                }
                catch (Exception ex) when (ex is OverflowException || ex is DivideByZeroException || (ex is TargetInvocationException && (ex?.InnerException is OverflowException || ex?.InnerException is DivideByZeroException)))
                {
                    input = Convert.ToDouble(input);
                    operand = Convert.ToDouble(operand);
                }
            }

            try
            {
                return ExpressionUtility
                    .CreateExpression(
                        body: operation,
                        leftType: input.GetType(),
                        rightType: operand.GetType())
                    .DynamicInvoke(input, operand);
            }
            catch (TargetInvocationException ex)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        public static IEnumerable Uniq(object input)
        {
            if (input == null)
                return null;
            var enumerable = input as IEnumerable;
            if (enumerable != null)
            {
                return enumerable.Cast<object>().Distinct();
            }
            else
            {
                return new List<object>(new[] { input });
            }
        }

        public static double Abs(object input)
        {
            double result;
            return !double.TryParse(input.ToString(), NumberStyles.Number, (IFormatProvider)CultureInfo.CurrentCulture, out result) ? 0.0 : Math.Abs(result);
        }

        public static object AtLeast(object input, object atLeast)
        {
            double result1;
            double result2;
            return double.TryParse(input.ToString(), NumberStyles.Number, (IFormatProvider)CultureInfo.CurrentCulture, out result1) & double.TryParse(atLeast.ToString(), NumberStyles.Number, (IFormatProvider)CultureInfo.CurrentCulture, out result2) ? (object)(result2 > result1 ? result2 : result1) : input;
        }

        public static object AtMost(object input, object atMost)
        {
            double result1;
            double result2;
            return double.TryParse(input.ToString(), NumberStyles.Number, (IFormatProvider)CultureInfo.CurrentCulture, out result1) & double.TryParse(atMost.ToString(), NumberStyles.Number, (IFormatProvider)CultureInfo.CurrentCulture, out result2) ? (object)(result2 < result1 ? result2 : result1) : input;
        }

        public static IEnumerable Compact(object input)
        {
            if (input == null)
                return (IEnumerable)null;
            List<object> source;
            if (input is IEnumerable)
            {
                source = ((IEnumerable)input).Flatten().Cast<object>().ToList<object>();
            }
            else
            {
                source = new List<object>((IEnumerable<object>)new object[1]
                {
          input
                });
            }

            if (!source.Any<object>())
                return (IEnumerable)source;
            source.RemoveAll((Predicate<object>)(item => item == null));
            return (IEnumerable)source;
        }

        public static IEnumerable Where(
          IEnumerable input,
          string propertyName,
          object targetValue = null)
        {
            if (input == null)
                return (IEnumerable)null;
            if (propertyName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(propertyName), "'propertyName' cannot be null or empty.");
            return (IEnumerable)input.Cast<object>().Where<object>((Func<object, bool>)(source => source.HasMatchingProperty(propertyName, targetValue)));
        }

        private static bool HasMatchingProperty(
          this object any,
          string propertyName,
          object targetValue)
        {
            object any1 = (object)null;
            if (any is IDictionary dictionary && dictionary.Contains((object)propertyName))
                any1 = dictionary[(object)propertyName];
            else if (any != null && any.RespondTo(propertyName))
                any1 = any.Send(propertyName);
            return targetValue != null && any1 != null ? any1.SafeTypeInsensitiveEqual(targetValue) : any1.IsTruthy();
        }

        public static IEnumerable Concat(IEnumerable left, IEnumerable right)
        {
            if (left == null)
                return right;
            return right == null ? left : (IEnumerable)left.Cast<object>().ToList<object>().Concat<object>(right.Cast<object>());
        }

        public static IEnumerable Reverse(IEnumerable input)
        {
            switch (input)
            {
                case null:
                case string _:
                    return input;
                default:
                    List<object> list = input.Cast<object>().ToList<object>();
                    list.Reverse();
                    return (IEnumerable)list;
            }
        }
    }
}
