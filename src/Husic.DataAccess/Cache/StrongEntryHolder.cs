using Husic.Standard.DataAccess.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.DataAccess.Cache
{
   public abstract class StrongEntryHolder : IEntryHolder
   {
      #region Private
      protected bool _Invalid = false;
      #endregion

      #region Methods
      public bool CanGetValue() => !_Invalid;
      public void Dispose() => Invalidate();
      public abstract void Invalidate();
      #endregion
   }

   public class StrongEntryHolder<T> : StrongEntryHolder, IEntryHolder<T>
   {
      #region Private
      private T _Value;
      #endregion
      public StrongEntryHolder(T value) => _Value = value;

      #region Methods
      public bool TryGetValue(out T value)
      {
         if (_Invalid)
         {
            value = default!; // okay to be null
            return false;
         }
         else
         {
            value = _Value;
            return true;
         }
      }
      public void Update(T newValue)
      {
         _Value = newValue;
         _Invalid = false;
      }
      public override void Invalidate()
      {
         _Value = default!; // okay to be null
         _Invalid = true;
      }
      #endregion
   }
}
