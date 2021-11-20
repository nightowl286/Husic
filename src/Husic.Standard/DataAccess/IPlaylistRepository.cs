using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Husic.Standard.DataAccess
{
   public interface IPlaylistRepository
   {
      #region Basic CRUD
      Task<IPlaylist> CreatePlaylist(IPlaylist data);
      Task<IPlaylist> GetPlaylist(int id);
      Task<IPlaylist> UpdatePlaylist(int id, IPlaylist data);
      Task DeletePlaylist(int id);
      #endregion

      #region Methods
      IPlaylist CreateNew(string name);
      Task<IEnumerable<IPlaylistEntry>> GetEntries(int playlistId, uint page);
      #endregion
   }
}
