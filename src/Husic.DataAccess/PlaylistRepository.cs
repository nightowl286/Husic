using Husic.DataAccess.Internal.Models;
using Husic.Standard.DataAccess;
using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husic.DataAccess
{
   public class PlaylistRepository : IPlaylistRepository
   {
      #region Consts
      private const int PAGE_SIZE = 30;
      #endregion

      #region Private
      private readonly DataCache<int, IPlaylist> _Cache = new DataCache<int, IPlaylist>();
      private readonly IDataAccess _DataAccess;
      #endregion
      public PlaylistRepository(IDataAccess dataAccess)
      {
         _DataAccess = dataAccess;
      }

      #region CRUD
      public async Task<IPlaylist> CreatePlaylist(IPlaylist data)
      {
         string sql = Load("Create");
         dynamic param = new
         {
            Name = data.Name,
         };

         int playlistId = await SqliteDataAccess.QueryFirst<int, dynamic>(sql, param);

         // Maybe change this, unsure which approach is better, but this
         // seems to reduce duplicate code for determining which columns
         // will be received
         IPlaylist playlist = await GetPlaylist(playlistId);

         _Cache.Add(playlistId, playlist);

         return playlist;
      }
      public async Task<IPlaylist> GetPlaylist(int id)
      {
         if (_Cache.TryGet(id, out IPlaylist cachedPlaylist))
            return cachedPlaylist;

         string sql = Load("Get");
         dynamic param = new { Id = id };

         PlaylistModel playlist = await SqliteDataAccess.QueryFirst<PlaylistModel, dynamic>(sql, param);

         IPlaylist converted = Convert(playlist);

         return converted;
      }
      public async Task<IPlaylist> UpdatePlaylist(int id, IPlaylist data)
      {
         string sql = Load("Update");
         dynamic param = new
         {
            Id = id,
            Name = data.Name,
         };

         await SqliteDataAccess.Execute(sql, param);
         if (_Cache.TryGet(id, out IPlaylist cachedData))
         {
            cachedData.Name = data.Name;
            return cachedData;
         }
         else
         {
            IPlaylist updatedPlaylist = await GetPlaylist(id);
            _Cache.Add(updatedPlaylist.Id, updatedPlaylist);
            return updatedPlaylist;
         }
      }
      public Task DeletePlaylist(int id)
      {
         string sql = Load("Delete");
         dynamic param = new { Id = id };

         _Cache.Delete(id);

         return SqliteDataAccess.Execute(sql, param);
      }
      #endregion

      #region Methods
      public async Task<IEnumerable<IPlaylist>> GetPlaylists(uint page, string sortBy = "Id", bool ascending = true)
      {
         string sql = Load("Query");

         if (!IsValidPlaylistColumn(sortBy))
            throw new ArgumentException("Invalid column name specified for the sort.", nameof(sortBy));

         sql = string.Format(sql, sortBy, ascending ? "ASC" : "DESC");

         dynamic param = new
         {
            Offset = page * PAGE_SIZE,
            Page = PAGE_SIZE,
         };

         IEnumerable<PlaylistModel> playlists = await SqliteDataAccess.Query<PlaylistModel, dynamic>(sql, param);

         return playlists.Select(Convert);
      }
      public async Task<IEnumerable<IPlaylist>> SearchPlaylists(string query, uint page, string sortBy = "Id", bool ascending = true)
      {
         string sql = Load("Search");

         if (!IsValidPlaylistColumn(sortBy))
            throw new ArgumentException("Invalid column name specified for the sort.", nameof(sortBy));

         sql = string.Format(sql, sortBy, ascending ? "ASC" : "DESC");

         dynamic param = new
         {
            Query = query,
            Offset = page * PAGE_SIZE,
            Page = PAGE_SIZE,
         };

         IEnumerable<PlaylistModel> playlists = await SqliteDataAccess.Query<PlaylistModel, dynamic>(sql, param);

         return playlists.Select(Convert);
      }
      public Task RemoveSongFromPlaylists(int songId) => throw new NotImplementedException();
      public Task RemoveEntryFromPlaylist(IPlaylist playlist, int entryIndex) => throw new NotImplementedException();
      public Task MoveEntryInPlaylist(IPlaylist playlist, int fromIndex, int toIndex) => throw new NotImplementedException();
      public IPlaylist CreateNew(string name)
      {
         return new Models.PlaylistModel(-1)
         {
            Name = name
         };
      }
      public Task<IEnumerable<IPlaylistEntry>> GetEntries(int playlistId, uint page) => throw new NotImplementedException();
      #endregion

      #region Helpers
      private IPlaylist Convert(PlaylistModel model)
      {
         if (_Cache.TryGet(model.Id, out IPlaylist cachedPlaylist))
            return cachedPlaylist;

         IPlaylist playlist = new Models.PlaylistModel(model.Id)
         {
            Name = model.Name,
            Count = model.Count
         };

         _Cache.Add(playlist.Id, playlist);
         return playlist;
      }
      private static bool IsValidPlaylistColumn(string column)
      {
         // Definitely need to find a better way of doing this
         // adding cases in the SQL script doesn't seem like a better approach
         switch (column.ToLower())
         {
            case "id":
            case "name":
            case "count":
               return true;

            default:
               return false;
         }
      }
      private string Load(string action) => _DataAccess.Load($"Playlists.{action}_Playlists");
      #endregion
   }
}
