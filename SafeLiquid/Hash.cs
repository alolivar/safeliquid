using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SafeLiquid
{
    public class Hash :
      IDictionary<string, object>,
      ICollection<KeyValuePair<string, object>>,
      IEnumerable<KeyValuePair<string, object>>,
      IEnumerable,
      IDictionary,
      ICollection
    {
        private static ConcurrentDictionary<string, Action<object, Hash>> mapperCache = new ConcurrentDictionary<string, Action<object, Hash>>();
        private readonly Func<Hash, string, object> _lambda;
        private readonly Dictionary<string, object> _nestedDictionary;
        private readonly object _defaultValue;

        public static Hash FromAnonymousObject(
          object anonymousObject,
          bool includeBaseClassProperties = false)
        {
            Hash hash = new Hash();
            if (anonymousObject != null)
                Hash.FromAnonymousObject(anonymousObject, hash, includeBaseClassProperties);
            return hash;
        }

        private static void FromAnonymousObject(
          object anonymousObject,
          Hash hash,
          bool includeBaseClassProperties)
        {
            Hash.GetObjToDictionaryMapper(anonymousObject.GetType(), includeBaseClassProperties)(anonymousObject, hash);
        }

        private static Action<object, Hash> GetObjToDictionaryMapper(
          Type type,
          bool includeBaseClassProperties)
        {
            string key = type.FullName + "_" + (includeBaseClassProperties ? "WithBaseProperties" : "WithoutBaseProperties");
            Action<object, Hash> mapper;
            if (!Hash.mapperCache.TryGetValue(key, out mapper))
            {
                mapper = Hash.GenerateMapper(type, includeBaseClassProperties);
                Hash.mapperCache[key] = mapper;
            }
            return mapper;
        }

        private static void AddBaseClassProperties(Type type, List<PropertyInfo> propertyList) => propertyList.AddRange((IEnumerable<PropertyInfo>)type.GetTypeInfo().BaseType.GetTypeInfo().DeclaredProperties.Where<PropertyInfo>((Func<PropertyInfo, bool>)(p => p.CanRead && p.GetMethod.IsPublic && !p.GetMethod.IsStatic)).ToList<PropertyInfo>());

        private static Action<object, Hash> GenerateMapper(
          Type type,
          bool includeBaseClassProperties)
        {
            ParameterExpression parameterExpression3 = Expression.Parameter(typeof(object), "objParam");
            ParameterExpression parameterExpression4 = Expression.Parameter(typeof(Hash), "hashParam");
            List<Expression> expressionList = new List<Expression>();
            ParameterExpression parameterExpression5 = Expression.Variable(type, "castedObj");
            expressionList.Add((Expression)Expression.Assign((Expression)parameterExpression5, (Expression)Expression.Convert((Expression)parameterExpression3, type)));
            List<PropertyInfo> list = type.GetTypeInfo().DeclaredProperties.Where<PropertyInfo>((Func<PropertyInfo, bool>)(p => p.CanRead && p.GetMethod.IsPublic && !p.GetMethod.IsStatic)).ToList<PropertyInfo>();
            if (includeBaseClassProperties)
                Hash.AddBaseClassProperties(type, list);
            foreach (PropertyInfo property in list)
            {
                expressionList.Add((Expression)Expression.Assign((Expression)Expression.MakeIndex((Expression)parameterExpression4, typeof(Hash).GetTypeInfo().GetDeclaredProperty("Item"), (IEnumerable<Expression>)new ConstantExpression[1]
        {
          Expression.Constant((object) property.Name, typeof (string))
        }), (Expression)Expression.Convert((Expression)Expression.Property((Expression)parameterExpression5, property), typeof(object))));
            }

            return ((Expression<Action<object, Hash>>)((parameterExpression1, parameterExpression2) => Expression.Block(typeof(void), (IEnumerable<ParameterExpression>)new ParameterExpression[1]
     {
        parameterExpression5
     }, (IEnumerable<Expression>)expressionList))).Compile();
        }

        public static Hash FromDictionary(IDictionary<string, object> dictionary)
        {
            Hash hash = new Hash();
            hash.Merge(dictionary);
            return hash;
        }

        public Hash(object defaultValue)
          : this()
        {
            this._defaultValue = defaultValue;
        }

        public Hash(Func<Hash, string, object> lambda)
          : this()
        {
            this._lambda = lambda;
        }

        public Hash() => this._nestedDictionary = new Dictionary<string, object>((IEqualityComparer<string>)Template.NamingConvention.StringComparer);

        public void Merge(IDictionary<string, object> otherValues)
        {
            foreach (string key in (IEnumerable<string>)otherValues.Keys)
                this._nestedDictionary[key] = otherValues[key];
        }

        protected virtual object GetValue(string key)
        {
            if (this._nestedDictionary.ContainsKey(key))
                return this._nestedDictionary[key];
            if (this._lambda != null)
                return this._lambda(this, key);
            return this._defaultValue != null ? this._defaultValue : (object)null;
        }

        public T Get<T>(string key) => (T)this[key];

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => (IEnumerator<KeyValuePair<string, object>>)this._nestedDictionary.GetEnumerator();

        public void Remove(object key) => ((IDictionary)this._nestedDictionary).Remove(key);

        object IDictionary.this[object key]
        {
            get => key is string ? this.GetValue((string)key) : throw new NotSupportedException();
            set => ((IDictionary)this._nestedDictionary)[key] = value;
        }

        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this._nestedDictionary.GetEnumerator();

        public void Add(KeyValuePair<string, object> item) => ((ICollection<KeyValuePair<string, object>>)this._nestedDictionary).Add(item);

        public virtual bool Contains(object key) => ((IDictionary)this._nestedDictionary).Contains(key);

        public void Add(object key, object value) => ((IDictionary)this._nestedDictionary).Add(key, value);

        public void Clear() => this._nestedDictionary.Clear();

        IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)this._nestedDictionary).GetEnumerator();

        public bool Contains(KeyValuePair<string, object> item) => ((ICollection<KeyValuePair<string, object>>)this._nestedDictionary).Contains(item);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => ((ICollection<KeyValuePair<string, object>>)this._nestedDictionary).CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, object> item) => ((ICollection<KeyValuePair<string, object>>)this._nestedDictionary).Remove(item);

        public void CopyTo(Array array, int index) => ((ICollection)this._nestedDictionary).CopyTo(array, index);

        public int Count => this._nestedDictionary.Count;

        public object SyncRoot => ((ICollection)this._nestedDictionary).SyncRoot;

        public bool IsSynchronized => ((ICollection)this._nestedDictionary).IsSynchronized;

        ICollection IDictionary.Values => ((IDictionary)this._nestedDictionary).Values;

        public bool IsReadOnly => ((ICollection<KeyValuePair<string, object>>)this._nestedDictionary).IsReadOnly;

        public bool IsFixedSize => ((IDictionary)this._nestedDictionary).IsFixedSize;

        public bool ContainsKey(string key) => this._nestedDictionary.ContainsKey(key);

        public void Add(string key, object value) => this._nestedDictionary.Add(key, value);

        public bool Remove(string key) => this._nestedDictionary.Remove(key);

        public bool TryGetValue(string key, out object value) => this._nestedDictionary.TryGetValue(key, out value);

        public object this[string key]
        {
            get => this.GetValue(key);
            set => this._nestedDictionary[key] = value;
        }

        public ICollection<string> Keys => (ICollection<string>)this._nestedDictionary.Keys;

        ICollection IDictionary.Keys => ((IDictionary)this._nestedDictionary).Keys;

        public ICollection<object> Values => (ICollection<object>)this._nestedDictionary.Values;
    }
}
