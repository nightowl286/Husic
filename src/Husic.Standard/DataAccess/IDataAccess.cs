using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Husic.Standard.Playback;

namespace Husic.Standard.DataAccess
{
   public interface IDataAccess
   {
      #region Methods
      Task<IEnumerable<ISong>> GetSongs();
      void EnsureTables();
      #endregion
   }
}
