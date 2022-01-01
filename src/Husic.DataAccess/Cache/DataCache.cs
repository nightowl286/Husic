using Husic.Standard.DataAccess.Cache;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Husic.DataAccess.Cache
{
   public abstract class DataCache : IDataCache
   {
      #region Methods
      public abstract void Clear();
      public abstract void ClearInvalidated();
      public abstract void Optimise();
      public void Dispose() => Clear();
      #endregion
   }

   public class DataCache<TKey, TData> : DataCache, IDataCache<TKey, TData> where TKey : notnull
   {
      #region Private
      private readonly Dictionary<TKey, ICacheEntry<TData>> _Entries = new Dictionary<TKey, ICacheEntry<TData>>();
      #endregion

      #region Methods
      public void AddStrong(TKey key, TData data)
      {
         StrongEntryHolder<TData> holder = new StrongEntryHolder<TData>(data);
         CacheEntry<TData> entry = new CacheEntry<TData>(holder);

         Add(key, entry);
      }
      public void Add(TKey key, [DisallowNull] ICacheEntry<TData> entry)
      {
         if (_Entries.TryGetValue(key, out ICacheEntry<TData>? stored))
         {
            stored.Dispose();
            _Entries[key] = entry;
         }
         else
            _Entries.Add(key, entry);
      }
      public void Delete(TKey key)
      {
         if (_Entries.Remove(key, out ICacheEntry<TData>? stored))
            stored.Dispose();
      }
      public bool TryGet(TKey key, out TData data)
      {
         if (_Entries.TryGetValue(key, out ICacheEntry<TData>? stored))
            return stored.TryGetValue(out data);

         data = default!; // okay to be null
         return false;
      }
      public bool TryGet(TKey key, [NotNullWhen(true)] out ICacheEntry<TData>? entry)
      {
         if (_Entries.TryGetValue(key, out ICacheEntry<TData>? stored))
         {
            if (stored.CanGetValue())
            {
               entry = stored;
               return true;
            }
         }

         entry = null;
         return false;
      }
      public override void Clear()
      {
         foreach (KeyValuePair<TKey, ICacheEntry<TData>> pair in _Entries)
            pair.Value.Dispose();
         _Entries.Clear();
      }
      public override void ClearInvalidated()
      {
         HashSet<TKey> toRemove = new HashSet<TKey>();
         foreach (KeyValuePair<TKey, ICacheEntry<TData>> pair in _Entries)
         {
            if (pair.Value.WasInvalidated)
            {
               toRemove.Add(pair.Key);
               pair.Value.Dispose();
            }
         }

         foreach (TKey key in toRemove)
            _Entries.Remove(key);
      }
      public override void Optimise()
      {
         HashSet<TKey> toRemove = new HashSet<TKey>();
         foreach (KeyValuePair<TKey, ICacheEntry<TData>> pair in _Entries)
         {
            if (pair.Value.WasInvalidated)
            {
               toRemove.Add(pair.Key);
               pair.Value.Dispose();
            }
            else if (pair.Value.ShouldInvalidate())
            {
               pair.Value.Invalidate();
               toRemove.Add(pair.Key);
               pair.Value.Dispose();
            }
         }

         foreach (TKey key in toRemove)
            _Entries.Remove(key);
      }
      public IEnumerable<KeyValuePair<TKey, ICacheEntry<TData>>> ByAccessDate() => _Entries.OrderBy(pair => pair.Value.AccessTime);
      #endregion
   }

   public static class DataCacheExtensions
   {
      public static void AddWeak<TKey, TData>(this DataCache<TKey, TData> cache, TKey key, TData? data) where TKey : notnull where TData : class
      {
         WeakEntryHolder<TData> holder = new WeakEntryHolder<TData>(data);
         CacheEntry<TData> entry = new CacheEntry<TData>(holder);

         cache.Add(key, entry);
      }
   }
}
