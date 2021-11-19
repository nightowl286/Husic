using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.Standard.DataAccess
{
   public interface IDataCache<TKey, TData> where TData : class
   {
      #region Methods
      void Delete(TKey key);
      bool TryGet(TKey key, out TData data);
      void Add(TKey key, TData data);
      void Clear();
      void CleanDead();
      #endregion
   }
}
