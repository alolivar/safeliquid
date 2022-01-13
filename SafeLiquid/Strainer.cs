
// Type: SafeLiquid.Strainer




using SafeLiquid.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace SafeLiquid
{

    public class PreStrainer
    {
        public readonly Dictionary<string, Type> Filters = new Dictionary<string, Type>();
        public readonly Dictionary<string, Tuple<object, MethodInfo>> FilterFuncs = new Dictionary<string, Tuple<object, MethodInfo>>();


        public void GlobalFilter(Type filter) => Filters[filter.AssemblyQualifiedName] = filter;

        public void GlobalFilter(string rawName, object target, MethodInfo methodInfo)
        {
            string memberName = Template.NamingConvention.GetMemberName(rawName);
            FilterFuncs[memberName] = Tuple.Create<object, MethodInfo>(target, methodInfo);
        }
    }


    public class Strainer
    {
        private readonly Context _context;
        private readonly Dictionary<string, IList<Tuple<object, MethodInfo>>> _methods = new Dictionary<string, IList<Tuple<object, MethodInfo>>>();


        public static Strainer Create(Context context, PreStrainer preStrainer)
        {
            Strainer strainer = new Strainer(context);
            foreach (KeyValuePair<string, Type> filter in preStrainer.Filters)
                strainer.Extend(filter.Value);
            foreach (KeyValuePair<string, Tuple<object, MethodInfo>> filterFunc in preStrainer.FilterFuncs)
                strainer.AddMethodInfo(filterFunc.Key, filterFunc.Value.Item1, filterFunc.Value.Item2);
            return strainer;
        }

        public IEnumerable<MethodInfo> Methods => this._methods.Values.SelectMany<IList<Tuple<object, MethodInfo>>, MethodInfo>((Func<IList<Tuple<object, MethodInfo>>, IEnumerable<MethodInfo>>)(m => m.Select<Tuple<object, MethodInfo>, MethodInfo>((Func<Tuple<object, MethodInfo>, MethodInfo>)(x => x.Item2))));

        private Strainer(Context context) => this._context = context;

        public void Extend(Type type)
        {
            IEnumerable<MethodInfo> source = type.GetRuntimeMethods().Where<MethodInfo>((Func<MethodInfo, bool>)(m => m.IsPublic && m.IsStatic));
            foreach (string key in source.Select<MethodInfo, string>((Func<MethodInfo, string>)(m => Template.NamingConvention.GetMemberName(m.Name))))
                this._methods.Remove(key);
            foreach (MethodInfo method in source)
                this.AddMethodInfo(method.Name, (object)null, method);
        }

        public void AddFunction<TIn, TOut>(string rawName, Func<TIn, TOut> func) => this.AddMethodInfo(rawName, func.Target, func.GetMethodInfo());

        public void AddFunction<TIn, TIn2, TOut>(string rawName, Func<TIn, TIn2, TOut> func) => this.AddMethodInfo(rawName, func.Target, func.GetMethodInfo());

        public void AddMethodInfo(string rawName, object target, MethodInfo method) => this._methods.TryAdd<string, IList<Tuple<object, MethodInfo>>>(Template.NamingConvention.GetMemberName(rawName), (Func<IList<Tuple<object, MethodInfo>>>)(() => (IList<Tuple<object, MethodInfo>>)new List<Tuple<object, MethodInfo>>())).Add(Tuple.Create<object, MethodInfo>(target, method));

        public bool RespondTo(string method) => this._methods.ContainsKey(method);

        public object Invoke(string method, List<object> args)
        {
            Tuple<object, MethodInfo> tuple = this._methods[method].FirstOrDefault<Tuple<object, MethodInfo>>((Func<Tuple<object, MethodInfo>, bool>)(m => ((IEnumerable<ParameterInfo>)m.Item2.GetParameters()).Count<ParameterInfo>((Func<ParameterInfo, bool>)(p => p.ParameterType != typeof(Context))) == args.Count)) ?? this._methods[method].OrderByDescending<Tuple<object, MethodInfo>, int>((Func<Tuple<object, MethodInfo>, int>)(m => m.Item2.GetParameters().Length)).First<Tuple<object, MethodInfo>>();
            ParameterInfo[] parameters = tuple.Item2.GetParameters();
            if (parameters.Length != 0 && parameters[0].ParameterType == typeof(Context))
                args.Insert(0, (object)this._context);
            if (parameters.Length > args.Count)
            {
                for (int count = args.Count; count < parameters.Length; ++count)
                {
                    if ((parameters[count].Attributes & ParameterAttributes.HasDefault) != ParameterAttributes.HasDefault)
                    {
                        throw new SyntaxException(Liquid.ResourceManager.GetString("StrainerFilterHasNoValueException"), new string[2]
            {
              method,
              parameters[count].Name
            });
                    }

                    args.Add(parameters[count].DefaultValue);
                }
            }
            for (int index = 0; index < parameters.Length; ++index)
            {
                if (args[index] is IConvertible convertible1)
                {
                    Type parameterType = parameters[index].ParameterType;
                    if (convertible1.GetType() != parameterType && !parameterType.IsAssignableFrom(convertible1.GetType()))
                        args[index] = Convert.ChangeType((object)convertible1, parameterType);
                }
            }
            try
            {
                return tuple.Item2.Invoke(tuple.Item1, args.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
