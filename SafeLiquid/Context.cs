
// Type: SafeLiquid.Context




using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace SafeLiquid
{
    public class Context
    {
        private static readonly Regex SingleQuotedRegex = R.C(R.Q("^'(.*)'$"));
        private static readonly Regex DoubleQuotedRegex = R.C(R.Q("^\"(.*)\"$"));
        private static readonly Regex IntegerRegex = R.C(R.Q("^([+-]?\\d+)$"));
        private static readonly Regex RangeRegex = R.C(R.Q("^\\((\\S+)\\.\\.(\\S+)\\)$"));
        private static readonly Regex NumericRegex = R.C(R.Q("^([+-]?\\d[\\d\\.|\\,]+)$"));
        private static readonly Regex SquareBracketedRegex = R.C(R.Q("^\\[(.*)\\]$"));
        private static readonly Regex VariableParserRegex = R.C(Liquid.VariableParser);
        private readonly ErrorsOutputMode _errorsOutputMode;
        private readonly int _maxIterations;
        private readonly int _timeout;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public SyntaxCompatibility SyntaxCompatibilityLevel { get; set; }

        public int MaxIterations => this._maxIterations;

        public List<Hash> Environments { get; private set; }

        public List<Hash> Scopes { get; private set; }

        public Hash Registers { get; private set; }

        public List<Exception> Errors { get; private set; }

        public Template Template { get; set; }

        public Context(
          Template template,
          List<Hash> environments,
          Hash outerScope,
          Hash registers,
          ErrorsOutputMode errorsOutputMode,
          int maxIterations,
          int timeout,
          IFormatProvider formatProvider)
          : this(template, environments, outerScope, registers, errorsOutputMode, maxIterations, formatProvider, CancellationToken.None)
        {
            this._timeout = timeout;
            this.RestartTimeout();
        }

        public Context(
          Template template,
          List<Hash> environments,
          Hash outerScope,
          Hash registers,
          ErrorsOutputMode errorsOutputMode,
          int maxIterations,
          IFormatProvider formatProvider,
          CancellationToken cancellationToken)
        {
            this.Template = template;
            this.Environments = environments;
            this.Scopes = new List<Hash>();
            if (outerScope != null)
                this.Scopes.Add(outerScope);
            this.Registers = registers;
            this.Errors = new List<Exception>();
            this._errorsOutputMode = errorsOutputMode;
            this._maxIterations = maxIterations;
            this._cancellationToken = cancellationToken;
            this.FormatProvider = formatProvider;
            this.SyntaxCompatibilityLevel = Template.DefaultSyntaxCompatibilityLevel;
            this.SquashInstanceAssignsWithEnvironments();
        }

        public Context(Template template, IFormatProvider formatProvider)
          : this(
                template,
                new List<Hash>(),
                new Hash(),
                new Hash(),
                ErrorsOutputMode.Display,
                0,
                0,
                formatProvider)
        {
        }

        public Strainer Strainer { get; set; }

        public void AddFilter<TIn, TOut>(string filterName, Func<TIn, TOut> func) => this.Strainer.AddFunction<TIn, TOut>(filterName, func);

        public void AddFilter<TIn, TIn2, TOut>(string filterName, Func<TIn, TIn2, TOut> func) => this.Strainer.AddFunction<TIn, TIn2, TOut>(filterName, func);

        public void AddFilters(IEnumerable<Type> filters)
        {
            foreach (Type filter in filters)
                this.Strainer.Extend(filter);
        }

        public void AddFilters(params Type[] filters)
        {
            if (filters == null)
                return;
            this.AddFilters(((IEnumerable<Type>)filters).AsEnumerable<Type>());
        }

        public string HandleError(Exception ex)
        {
            switch (ex)
            {
                case InterruptException _:
                case TimeoutException _:
                case RenderException _:
                case OperationCanceledException _:
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    break;
            }
            this.Errors.Add(ex);
            if (this._errorsOutputMode == ErrorsOutputMode.Suppress)
                return string.Empty;
            if (this._errorsOutputMode == ErrorsOutputMode.Rethrow)
                ExceptionDispatchInfo.Capture(ex).Throw();
            return ex is SyntaxException ? string.Format(Liquid.ResourceManager.GetString("ContextLiquidSyntaxError"), (object)ex.Message) : string.Format(Liquid.ResourceManager.GetString("ContextLiquidError"), (object)ex.Message);
        }

        public object Invoke(string method, List<object> args) => this.Strainer.RespondTo(method) ? this.Strainer.Invoke(method, args) : args.First<object>();

        public void Push(Hash newScope)
        {
            if (this.Scopes.Count > 80)
                throw new StackLevelException(Liquid.ResourceManager.GetString("ContextStackException"));
            this.Scopes.Insert(0, newScope);
        }

        public void Merge(Hash newScopes) => this.Scopes[0].Merge((IDictionary<string, object>)newScopes);

        public Hash Pop()
        {
            Hash hash = this.Scopes.Count != 1 ? this.Scopes[0] : throw new ContextException();
            this.Scopes.RemoveAt(0);
            return hash;
        }

        public void Stack(Hash newScope, Action callback)
        {
            this.Push(newScope);
            try
            {
                callback();
            }
            finally
            {
                this.Pop();
            }
        }

        public void Stack(Action callback) => this.Stack(new Hash(), callback);

        public void ClearInstanceAssigns() => this.Scopes[0].Clear();

        public object this[string key, bool notifyNotFound = true]
        {
            get => this.Resolve(key, notifyNotFound);
            set => this.Scopes[0][key] = value;
        }

        public bool HasKey(string key) => this.Resolve(key, false) != null;

        private object Resolve(string key, bool notifyNotFound = true)
        {
            switch (key)
            {
                case null:
                case "":
                case "nil":
                case "null":
                    return (object)null;
                case "blank":
                case "empty":
                    return (object)new Symbol((Func<object, bool>)(o => o is IEnumerable && !((IEnumerable)o).Cast<object>().Any<object>()));
                case "false":
                    return (object)false;
                case "true":
                    return (object)true;
                default:
                    Match match1 = SingleQuotedRegex.Match(key);
                    if (match1.Success)
                        return (object)match1.Groups[1].Value;
                    Match match2 = DoubleQuotedRegex.Match(key);
                    if (match2.Success)
                        return (object)match2.Groups[1].Value;
                    Match match3 = IntegerRegex.Match(key);
                    if (match3.Success)
                    {
                        try
                        {
                            return (object)Convert.ToInt32(match3.Groups[1].Value);
                        }
                        catch (OverflowException ex)
                        {
                            return (object)Convert.ToInt64(match3.Groups[1].Value);
                        }
                    }
                    else
                    {
                        Match match4 = RangeRegex.Match(key);
                        if (match4.Success)
                            return (object)SafeLiquid.Util.Range.Inclusive(Convert.ToInt32(this.Resolve(match4.Groups[1].Value)), Convert.ToInt32(this.Resolve(match4.Groups[2].Value)));
                        Match match5 = NumericRegex.Match(key);
                        if (!match5.Success)
                            return this.Variable(key, notifyNotFound);
                        Decimal result1;
                        if (Decimal.TryParse(match5.Groups[1].Value, NumberStyles.Number | NumberStyles.AllowExponent, this.FormatProvider, out result1))
                            return (object)result1;
                        Decimal result2;
                        if (Decimal.TryParse(match5.Groups[1].Value, NumberStyles.Number | NumberStyles.AllowExponent, (IFormatProvider)CultureInfo.InvariantCulture, out result2))
                            return (object)result2;
                        double result3;
                        return double.TryParse(match5.Groups[1].Value, NumberStyles.Number | NumberStyles.AllowExponent, this.FormatProvider, out result3) ? (object)result3 : (object)double.Parse(match5.Groups[1].Value, NumberStyles.Number | NumberStyles.AllowExponent, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
            }
        }

        public IFormatProvider FormatProvider { get; }

        private object FindVariable(string key)
        {
            Hash hash1 = this.Scopes.FirstOrDefault<Hash>((Func<Hash, bool>)(s => s.ContainsKey(key)));
            object obj1 = (object)null;
            if (hash1 == null)
            {
                foreach (Hash environment in this.Environments)
                {
                    if ((obj1 = this.LookupAndEvaluate((object)environment, (object)key)) != null)
                    {
                        hash1 = environment;
                        break;
                    }
                }
            }
            Hash hash2 = hash1 ?? this.Environments.LastOrDefault<Hash>() ?? this.Scopes.Last<Hash>();
            object obj2 = Liquidize(obj1 ?? this.LookupAndEvaluate((object)hash2, (object)key));
            if (obj2 is IContextAware contextAware)
                contextAware.Context = this;
            return obj2;
        }

        private object Variable(string markup, bool notifyNotFound)
        {
            List<string> list = R.Scan(markup, VariableParserRegex);
            string atIndex = list.TryGetAtIndex<string>(0);
            Match match1 = SquareBracketedRegex.Match(atIndex);
            if (match1.Success)
                atIndex = this.Resolve(match1.Groups[1].Value).ToString();
            object obj1;
            if ((obj1 = this.FindVariable(atIndex)) == null)
            {
                if (notifyNotFound)
                    this.Errors.Add((Exception)new VariableNotFoundException(string.Format(Liquid.ResourceManager.GetString("VariableNotFoundException"), (object)markup)));
                return (object)null;
            }
            for (int index = 1; index < list.Count; ++index)
            {
                string input = list[index];
                Match match2 = SquareBracketedRegex.Match(input);
                bool success = match2.Success;
                object obj2 = (object)input;
                if (success)
                    obj2 = this.Resolve(match2.Groups[1].Value);
                if (IsKeyValuePair(obj1) && (obj2.SafeTypeInsensitiveEqual((object)0L) || obj2.Equals((object)"Key")))
                {
                    obj1 = Liquidize(obj1.GetType().GetRuntimeProperty("Key").GetValue(obj1));
                }
                else if (IsKeyValuePair(obj1) && (obj2.SafeTypeInsensitiveEqual((object)1L) || obj2.Equals((object)"Value")))
                {
                    obj1 = Liquidize(obj1.GetType().GetRuntimeProperty("Value").GetValue(obj1));
                }
                else if (obj1 is KeyValuePair<string, object> keyValuePair4 && keyValuePair4.Key == (string)obj2)
                {
                    keyValuePair4 = (KeyValuePair<string, object>)obj1;
                    obj1 = Liquidize(keyValuePair4.Value);
                }
                else if (IsHashOrArrayLikeObject(obj1, obj2))
                {
                    obj1 = Liquidize(this.LookupAndEvaluate(obj1, obj2));
                }
                else if (!success && obj1 is IEnumerable && (Template.NamingConvention.OperatorEquals(obj2 as string, "size") || Template.NamingConvention.OperatorEquals(obj2 as string, "first") || Template.NamingConvention.OperatorEquals(obj2 as string, "last")))
                {
                    IEnumerable<object> source = ((IEnumerable)obj1).Cast<object>();
                    if (Template.NamingConvention.OperatorEquals(obj2 as string, "size"))
                        obj1 = (object)source.Count<object>();
                    else if (Template.NamingConvention.OperatorEquals(obj2 as string, "first"))
                        obj1 = Liquidize(source.FirstOrDefault<object>());
                    else if (Template.NamingConvention.OperatorEquals(obj2 as string, "last"))
                        obj1 = Liquidize(source.LastOrDefault<object>());
                }
                else
                {
                    this.Errors.Add((Exception)new VariableNotFoundException(string.Format(Liquid.ResourceManager.GetString("VariableNotFoundException"), (object)markup)));
                    return (object)null;
                }
                if (obj1 is IContextAware contextAware2)
                    contextAware2.Context = this;
            }
            return obj1;
        }

        private static bool IsHashOrArrayLikeObject(object obj, object part) => obj != null && (obj is IDictionary && ((IDictionary)obj).Contains(part) || obj is IDictionary<string, object> dictionary && dictionary.ContainsKey(part.ToString()) || obj is IList && (part is int || part is long) || TypeUtility.IsAnonymousType(obj.GetType()) && obj.GetType().GetRuntimeProperty((string)part) != (PropertyInfo)null || obj is IIndexable && ((IIndexable)obj).ContainsKey(part));

        private object LookupAndEvaluate(object obj, object key)
        {
            object obj1;
            switch (obj)
            {
                case IDictionary dictionary1:
                    obj1 = dictionary1[key];
                    break;
                case IDictionary<string, object> dictionary2:
                    obj1 = dictionary2[key.ToString()];
                    break;
                case IList list1:
                    obj1 = list1[Convert.ToInt32(key)];
                    break;
                default:
                    if (TypeUtility.IsAnonymousType(obj.GetType()))
                    {
                        obj1 = obj.GetType().GetRuntimeProperty((string)key).GetValue(obj, (object[])null);
                        break;
                    }
                    obj1 = obj is IIndexable indexable2 ? indexable2[key] : throw new NotSupportedException();
                    break;
            }
            if (!(obj1 is Proc proc))
                return obj1;
            object obj2 = proc(this);
            if (obj is IDictionary dictionary3)
            {
                dictionary3[key] = obj2;
            }
            else if (obj is IList list3)
            {
                list3[Convert.ToInt32(key)] = obj2;
            }
            else
            {
                if (!TypeUtility.IsAnonymousType(obj.GetType()))
                    throw new NotSupportedException();
                obj.GetType().GetRuntimeProperty((string)key).SetValue(obj, obj2, (object[])null);
            }
            return obj2;
        }

        private object Liquidize(object obj)
        {
            switch (obj)
            {
                case null:
                    return obj;
                case ILiquidizable liquidizable:
                    return liquidizable.ToLiquid();
                case string _:
                    return obj;
                case IEnumerable _:
                    return obj;
                default:
                    if (obj.GetType().GetTypeInfo().IsPrimitive)
                        return obj;
                    switch (obj)
                    {
                        case Decimal _:
                            return obj;
                        case DateTime _:
                            return obj;
                        case DateTimeOffset _:
                            return obj;
                        case TimeSpan _:
                            return obj;
                        case Guid _:
                            return obj;
                        default:
                            if (TypeUtility.IsAnonymousType(obj.GetType()))
                                return obj;
                            Func<object, object> safeTypeTransformer = Template.GetSafeTypeTransformer(obj.GetType());
                            if (safeTypeTransformer != null)
                                return safeTypeTransformer(obj);
                            if (((IEnumerable<object>)obj.GetType().GetTypeInfo().GetCustomAttributes(typeof(LiquidTypeAttribute), false)).Any<object>())
                            {
                                LiquidTypeAttribute liquidTypeAttribute = (LiquidTypeAttribute)((IEnumerable<object>)obj.GetType().GetTypeInfo().GetCustomAttributes(typeof(LiquidTypeAttribute), false)).First<object>();
                                return (object)new DropProxy(obj, liquidTypeAttribute.AllowedMembers);
                            }
                            return IsKeyValuePair(obj) ? obj : throw new SyntaxException(Liquid.ResourceManager.GetString("ContextObjectInvalidException"), new string[1]
                            {
                obj.ToString()
                            });
                    }
            }
        }

        private static bool IsKeyValuePair(object obj)
        {
            if (obj != null)
            {
                Type type = obj.GetType();
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                    return true;
            }
            return false;
        }

        private void SquashInstanceAssignsWithEnvironments()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>)Template.NamingConvention.StringComparer);
            Hash hash = this.Scopes.Last<Hash>();
            foreach (string key in (IEnumerable<string>)hash.Keys)
            {
                foreach (Hash environment in this.Environments)
                {
                    if (environment.ContainsKey(key))
                    {
                        dictionary[key] = this.LookupAndEvaluate((object)environment, (object)key);
                        break;
                    }
                }
            }
            foreach (string key in dictionary.Keys)
                hash[key] = dictionary[key];
        }

        public void RestartTimeout()
        {
            this._stopwatch.Restart();
            this._cancellationToken.ThrowIfCancellationRequested();
        }

        public void CheckTimeout()
        {
            if (this._timeout > 0 && this._stopwatch.ElapsedMilliseconds > (long)this._timeout)
                throw new TimeoutException();
            this._cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
