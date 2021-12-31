using System;

namespace Husic.Standard.DataAccess.Cache
{
   public interface ICacheRestraint : IDisposable
   {
      #region Methods
      bool ShouldInvalidate(ICacheEntry entry);
      #endregion
   }
}
