using Husic.Standard.DataAccess.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.DataAccess.Cache
{
   public abstract class WeakEntryHolder : IEntryHolder
   {
      #region Methods
      public abstract bool CanGetValue();
      public abstract void Invalidate();
      public void Dispose() => Invalidate();
      #endregion
   }

   public class WeakEntryHolder<T> : WeakEntryHolder, IEntryHolder<T> where T : class
   {
      #region Private
      private WeakReference<T?>? _Reference;
      #endregion
      public WeakEntryHolder(T? value) => _Reference = new WeakReference<T?>(value);

      #region Methods
      public override bool CanGetValue() => _Reference?.TryGetTarget(out _) == true;
      public bool TryGetValue(out T value)
      {
         if (_Reference?.TryGetTarget(out T? val) == true)
         {
            value = val;
            return true;
         }
         else
         {
            value = default!; // okay to be null
            return false;
         }
      }
      public void Update(T? newValue)
      {
         if (_Reference == null)
            _Reference = new WeakReference<T?>(newValue);
         else
            _Reference.SetTarget(newValue);
      }
      public override void Invalidate() => _Reference = null;
      #endregion
   }
}
