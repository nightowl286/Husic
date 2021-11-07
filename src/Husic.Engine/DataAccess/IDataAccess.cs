using Husic.Engine.Playback;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Husic.Engine.DataAccess
{
   public interface IDataAccess
   {
      #region Methods
      Task<IEnumerable<ISong>> GetSongs();
      void EnsureTables();
      #endregion
   }
}
