using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Husic.Standard.DataAccess.Repositories
{
   public interface IPlaylistRepository : IKeyBasedRepository<int, IPlaylist>
   {

      #region Methods
      Task<IEnumerable<IPlaylist>> GetPlaylists(uint page, string sortBy = "Id", bool ascending = true);
      Task<IEnumerable<IPlaylist>> SearchPlaylists(string query, uint page, string sortBy = "Id", bool ascending = true);
      Task RemoveSongFromPlaylists(int songId);
      Task RemoveEntryFromPlaylist(IPlaylist playlist, int entryIndex);
      Task MoveEntryInPlaylist(IPlaylist playlist, int fromIndex, int toIndex);
      IPlaylist CreateNew(string name);
      Task<IEnumerable<IPlaylistEntry>> GetEntries(int playlistId, uint page);
      #endregion
   }
}
