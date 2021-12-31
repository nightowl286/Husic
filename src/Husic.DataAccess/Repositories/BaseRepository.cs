using Husic.Standard.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.DataAccess.Repositories
{
   public class BaseRepository : IBaseRepository
   {
      public int PageSize => throw new NotImplementedException();
   }
}
