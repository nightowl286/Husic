using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Husic.Standard.DataAccess.Repositories
{
   public interface ISongRepository : IKeyBasedRepository<int, ISong>
   {
      #region Methods
      Task SaveUpdatedDuration(ISong song);
      ISong CreateNew(string name, TimeSpan duration, Uri source);
      Task<IEnumerable<ISong>> GetSongs(uint page, string sortBy = "Id", bool ascending = true);
      Task<IEnumerable<ISong>> SearchSongs(string query, uint page, string sortBy = "Id", bool ascending = true);
      #endregion
   }
}
