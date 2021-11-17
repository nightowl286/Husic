using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Husic.Standard.DataAccess
{
   public interface ISongRepository
   {
      #region Basic CRUD
      Task<ISong> CreateSong(ISong data);
      Task<ISong> GetSong(int id);
      Task<ISong> UpdateSong(int id, ISong data);
      Task DeleteSong(int id);
      #endregion

      #region Methods
      Task<IEnumerable<ISong>> GetSongs(uint page, string sortBy = "Id", bool ascending = true);
      Task<IEnumerable<ISong>> SearchSongs(string query, uint page, string sortBy = "Id", bool ascending = true);
      #endregion
   }
}
