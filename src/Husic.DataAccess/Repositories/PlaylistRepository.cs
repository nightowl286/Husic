using Husic.DataAccess.Internal.Models;
using Husic.Standard.DataAccess.Repositories;
using Husic.Standard.DataAccess;
using Husic.Standard.Playback;
using Husic.DataAccess.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husic.DataAccess.Repositories
{
   public class PlaylistRepository : KeyBasedRepository<int, IPlaylist>, IPlaylistRepository
   {
      #region Private
      private readonly Cache.DataCache<int, IPlaylist> _Cache = new Cache.DataCache<int, IPlaylist>();
      #endregion
      public PlaylistRepository() : base()
      {
         CreateAssemblyScriptContainer("Internal.Scripts.Playlists.{0}_Playlists.sqlite");
      }

      #region CRUD
      protected override async Task<IPlaylist> Create(IPlaylist data, string script)
      {
         dynamic param = new
         {
            data.Name,
         };

         int playlistId = await SqliteDataAccess.QueryFirst<int, dynamic>(script, param);

         // Maybe change this, unsure which approach is better, but this
         // seems to reduce duplicate code for determining which columns
         // will be received
         IPlaylist playlist = await Get(playlistId);

         _Cache.AddWeak(playlistId, playlist);

         return playlist;
      }
      protected override async Task<IPlaylist> Get(int id, string script)
      {
         if (_Cache.TryGet(id, out IPlaylist cachedPlaylist))
            return cachedPlaylist;

         PlaylistModel playlist = await GetFirst<PlaylistModel>(id, script);

         IPlaylist converted = Convert(playlist);

         return converted;
      }
      protected override async Task<IPlaylist> Update(int id, IPlaylist data, string script)
      {
         dynamic param = new
         {
            Id = id,
            data.Name,
         };

         await SqliteDataAccess.Execute(script, param);
         if (_Cache.TryGet(id, out IPlaylist cachedData))
         {
            cachedData.Name = data.Name;
            return cachedData;
         }
         else
         {
            IPlaylist updatedPlaylist = await Get(id);
            _Cache.AddWeak(updatedPlaylist.Id, updatedPlaylist);
            return updatedPlaylist;
         }
      }
      protected override Task Delete(int id, string script)
      {
         _Cache.Delete(id);

         return base.Delete(id, script);
      }
      #endregion

      #region Methods
      public async Task<IEnumerable<IPlaylist>> GetPlaylists(uint page, string sortBy = "Id", bool ascending = true)
      {
         string sql = GetScript("Query");

         if (!IsValidPlaylistColumn(sortBy))
            throw new ArgumentException("Invalid column name specified for the sort.", nameof(sortBy));

         sql = string.Format(sql, sortBy, ascending ? "ASC" : "DESC");

         dynamic param = new
         {
            Offset = page * PageSize,
            Page = PageSize,
         };

         IEnumerable<PlaylistModel> playlists = await SqliteDataAccess.Query<PlaylistModel, dynamic>(sql, param);

         return playlists.Select(Convert);
      }
      public async Task<IEnumerable<IPlaylist>> SearchPlaylists(string query, uint page, string sortBy = "Id", bool ascending = true)
      {
         string sql = GetScript("Search");

         if (!IsValidPlaylistColumn(sortBy))
            throw new ArgumentException("Invalid column name specified for the sort.", nameof(sortBy));

         sql = string.Format(sql, sortBy, ascending ? "ASC" : "DESC");

         dynamic param = new
         {
            Query = query,
            Offset = page * PageSize,
            Page = PageSize,
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

         _Cache.AddWeak(playlist.Id, playlist);
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
      #endregion
   }
}
