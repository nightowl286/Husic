using System;
using System.Diagnostics.CodeAnalysis;

namespace Husic.Standard.DataAccess.Cache
{
   public interface IEntryHolder : IDisposable
   {
      #region Methods
      bool CanGetValue();
      void Invalidate();
      #endregion
   }

   public interface IEntryHolder<T> : IEntryHolder
   {
      #region Methods
      bool TryGetValue(out T value);
      void Update(T newValue);
      #endregion
   }
}
