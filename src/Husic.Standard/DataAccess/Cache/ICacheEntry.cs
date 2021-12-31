using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Husic.Standard.DataAccess.Cache
{
   public interface ICacheEntry : IDisposable
   {
      #region Properties
      DateTime StoreTime { get; }
      DateTime AccessTime { get; }
      IEnumerable<ICacheRestraint> Restraints { get; }
      bool WasInvalidated { get; }
      #endregion

      #region Methods
      bool CanGetValue();
      void Invalidate();
      bool ShouldInvalidate();
      bool TryGetRestraint<U>([NotNullWhen(true)] out U? restraint) where U : class, ICacheRestraint
      {
         IEnumerable<U> typed = GetRestraints<U>();
         U? value = typed.FirstOrDefault();
         if (value is null)
         {
            restraint = null;
            return false;
         }
         else
         {
            restraint = value;
            return true;
         }
      }
      IEnumerable<U> GetRestraints<U>() where U : class, ICacheRestraint
      {
         foreach (ICacheRestraint restraint in Restraints)
         {
            if (restraint is U typed)
               yield return typed;
         }
      }
      ICacheEntry AddRestraint([DisallowNull] ICacheRestraint restraint);
      #endregion
   }

   public interface ICacheEntry<T> : ICacheEntry
   {
      #region Methods
      void Update(T newValue);
      bool TryGetValue(out T value);
      new ICacheEntry<T> AddRestraint([DisallowNull] ICacheRestraint restraint);
      #endregion
   }
}
