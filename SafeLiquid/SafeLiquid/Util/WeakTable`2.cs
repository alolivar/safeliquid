
// Type: SafeLiquid.Util.WeakTable`2




using System;

namespace SafeLiquid.Util
{
  internal class WeakTable<TKey, TValue> where TValue : class
  {
    private readonly WeakTable<TKey, TValue>.Bucket[] _buckets;

    public WeakTable(int size) => this._buckets = new WeakTable<TKey, TValue>.Bucket[size];

    public TValue this[TKey key]
    {
      get
      {
        TValue obj;
        if (!this.TryGetValue(key, out obj))
          throw new ArgumentException(Liquid.ResourceManager.GetString("WeakTableKeyNotFoundException"));
        return obj;
      }
      set
      {
        int index = Math.Abs(key.GetHashCode()) % this._buckets.Length;
        this._buckets[index].Key = key;
        this._buckets[index].Value = new WeakReference((object) value);
      }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      int index = Math.Abs(key.GetHashCode()) % this._buckets.Length;
      WeakReference weakReference;
      if ((weakReference = this._buckets[index].Value) == null || !this._buckets[index].Key.Equals((object) key))
      {
        value = default (TValue);
        return false;
      }
      value = (TValue) weakReference.Target;
      return weakReference.IsAlive;
    }

    public void Remove(TKey key)
    {
      int index = Math.Abs(key.GetHashCode()) % this._buckets.Length;
      if (!this._buckets[index].Key.Equals((object) key))
        return;
      this._buckets[index].Value = (WeakReference) null;
    }

    private struct Bucket
    {
      public TKey Key;
      public WeakReference Value;
    }
  }
}
