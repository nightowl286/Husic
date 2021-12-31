using Husic.Standard.DataAccess.Cache;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Husic.DataAccess.Cache
{
   public abstract class CacheEntry : ICacheEntry
   {
      #region Private
      protected readonly List<ICacheRestraint> _Restraints = new List<ICacheRestraint>();
      #endregion

      #region Properties
      public DateTime StoreTime { get; protected set; }
      public DateTime AccessTime { get; protected set; }
      public IEnumerable<ICacheRestraint> Restraints => _Restraints;
      public abstract bool WasInvalidated { get; }
      #endregion

      #region Methods
      public CacheEntry AddRestraint([DisallowNull] ICacheRestraint restraint)
      {
         _Restraints.Add(restraint);
         return this;
      }
      ICacheEntry ICacheEntry.AddRestraint([DisallowNull] ICacheRestraint restraint) => AddRestraint(restraint);
      public abstract bool CanGetValue();
      public abstract void Invalidate();
      public bool ShouldInvalidate()
      {
         foreach (ICacheRestraint restraint in _Restraints)
         {
            if (restraint.ShouldInvalidate(this))
               return true;
         }
         return false;
      }
      public abstract void Dispose();
      #endregion
   }

   public class CacheEntry<T> : CacheEntry, ICacheEntry<T>
   {
      #region Private
      private readonly IEntryHolder<T> _Holder;
      #endregion

      #region Properties
      public override bool WasInvalidated => !_Holder.CanGetValue();
      #endregion
      public CacheEntry(IEntryHolder<T> holder)
      {
         _Holder = holder;
         AccessTime = StoreTime = DateTime.UtcNow;
      }

      #region Methods
      public override bool CanGetValue() => _Holder.CanGetValue();
      public override void Invalidate() => _Holder.Invalidate();
      public bool TryGetValue(out T value) => _Holder.TryGetValue(out value);
      public void Update(T newValue) => _Holder.Update(newValue);
      public new CacheEntry<T> AddRestraint([DisallowNull] ICacheRestraint restraint)
      {
         _Restraints.Add(restraint);
         return this;
      }
      ICacheEntry<T> ICacheEntry<T>.AddRestraint(ICacheRestraint restraint) => AddRestraint(restraint);
      public override void Dispose()
      {
         _Holder.Dispose();
         _Restraints.Clear();
      }
      #endregion
   }
}
