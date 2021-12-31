using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.Standard.DataAccess.Repositories
{
   public interface IBaseRepository
   {
      #region Properties
      int PageSize { get; }
      #endregion
   }
}
