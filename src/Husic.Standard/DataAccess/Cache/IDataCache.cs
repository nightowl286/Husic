using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Husic.Standard.DataAccess.Cache
{
   public interface IDataCache : IDisposable
   {
      #region Methods
      void Clear();
      void ClearInvalidated();
      void Optimise();
      #endregion
   }

   public interface IDataCache<TKey, TData> where TKey : notnull
   {
      #region Methods
      void Add(TKey key, [DisallowNull] ICacheEntry<TData> entry);
      void Delete(TKey key);
      bool TryGet(TKey key, out TData data);
      bool TryGet(TKey key, [NotNullWhen(true)] out ICacheEntry<TData>? entry);
      IEnumerable<KeyValuePair<TKey, ICacheEntry<TData>>> ByAccessDate();
      #endregion
   }
}
