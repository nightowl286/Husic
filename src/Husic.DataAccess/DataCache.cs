using Husic.Standard.DataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Husic.DataAccess
{
   public class DataCache<TKey, TData> : IDataCache<TKey, TData> where TData : class
   {
      #region Private
      Dictionary<TKey, WeakReference<TData>> _References;
      #endregion
      public DataCache()
      {
         _References = new Dictionary<TKey, WeakReference<TData>>();
      }

      #region Methods
      public void Add(TKey key, TData data)
      {
         if (_References.TryGetValue(key, out WeakReference<TData> reference))
            reference.SetTarget(data);
         else
            _References.Add(key, new WeakReference<TData>(data));
      }
      public bool TryGet(TKey key, out TData data)
      {
         if (_References.TryGetValue(key, out WeakReference<TData> reference))
            return reference.TryGetTarget(out data);

         data = null;
         return false;
      }
      public void Delete(TKey key) => _References.Remove(key);
      public void Clear() => _References.Clear();
      public void CleanDead()
      {
         foreach ((TKey key, WeakReference<TData> reference) in _References)
         {
            if (!reference.TryGetTarget(out _))
               _References.Remove(key);
         }
      }
      #endregion
   }
}
