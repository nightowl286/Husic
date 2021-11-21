using Dapper;
using Husic.DataAccess.Internal.Models;
using Husic.Standard.DataAccess;
using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husic.DataAccess
{
   public class SongRepository : ISongRepository
   {
      #region Consts
      private const int PAGE_SIZE = 30;
      #endregion

      #region Private
      private readonly DataCache<int, ISong> _Cache = new DataCache<int, ISong>();
      private readonly IDataAccess _DataAccess;
      #endregion
      public SongRepository(IDataAccess dataAccess)
      {
         _DataAccess = dataAccess;
      }

      #region Basic CRUD Methods
      public async Task<ISong> CreateSong(ISong data)
      {
         string sql = Load("Create");
         dynamic param = new
         {
            Name = data.Name,
            Duration = data.Duration.TotalSeconds,
            Source = GetPath(data.Source)
         };

         int songId = await SqliteDataAccess.QueryFirst<int, dynamic>(sql, param);

         // Maybe change this, unsure which approach is better, but this
         // seems to reduce duplicate code for determining which columns
         // will be received
         ISong song = await GetSong(songId);

         _Cache.Add(songId, song);

         return song;
      }
      public async Task<ISong> GetSong(int id)
      {
         if (_Cache.TryGet(id, out ISong cachedSong))
            return cachedSong;

         string sql = Load("Get");
         dynamic param = new { Id = id };

         SongModel song = await SqliteDataAccess.QueryFirst<SongModel, dynamic>(sql, param);

         ISong converted = Convert(song);

         return converted;
      }
      public async Task<ISong> UpdateSong(int id, ISong data)
      {
         string sql = Load("Update");
         dynamic param = new
         {
            Id = id,
            Name = data.Name,
            Source = GetPath(data.Source),
            Duration = data.Duration.TotalSeconds
         };

         await SqliteDataAccess.Execute(sql, param);
         if (_Cache.TryGet(id, out ISong cachedData))
         {
            cachedData.Name = data.Name;
            cachedData.Source = data.Source;
            cachedData.Duration = data.Duration;
            return cachedData;
         }
         else
         {
            ISong updatedSong = await GetSong(id);
            _Cache.Add(updatedSong.Id, updatedSong);
            return updatedSong;
         }
      }
      public Task SaveUpdatedDuration(ISong song)
      {
         string sql = Load("UpdateDuration");
         dynamic param = new
         {
            Id = song.Id,
            Duration = song.Duration.TotalSeconds
         };

         Debug.Assert(_Cache.TryGet(song.Id, out _));
         return SqliteDataAccess.Execute(sql, param);
      }
      public Task DeleteSong(int id)
      {
         string sql = Load("Delete");
         dynamic param = new { Id = id };

         _Cache.Delete(id);

         return SqliteDataAccess.Execute(sql, param);
      }
      #endregion

      #region Methods
      private ISong Convert(SongModel model)
      {
         if (_Cache.TryGet(model.Id, out ISong cachedSong))
            return cachedSong;

         ISong song = new Models.SongModel(model.Id)
         {
            Duration = TimeSpan.FromSeconds(model.Duration),
            Name = model.Name,
            Source = new Uri(model.Source)
         };

         _Cache.Add(song.Id, song);
         return song;
      }
      public ISong CreateNew(string name, TimeSpan duration, Uri source)
      {
         return new Models.SongModel(-1)
         {
            Name = name,
            Duration = duration,
            Source = source
         };
      }
      public async Task<IEnumerable<ISong>> GetSongs(uint page, string sortBy = "Id", bool ascending = true)
      {
         string sql = Load("Query");

         if (!IsValidSongColumn(sortBy))
            throw new ArgumentException("Invalid column name specified for the sort.", nameof(sortBy));

         sql = string.Format(sql, sortBy, ascending ? "ASC" : "DESC");

         dynamic param = new
         {
            Offset = page * PAGE_SIZE,
            Page = PAGE_SIZE,
         };

         IEnumerable<SongModel> songs = await SqliteDataAccess.Query<SongModel, dynamic>(sql, param);

         return songs.Select(Convert);
      }
      public async Task<IEnumerable<ISong>> SearchSongs(string query, uint page, string sortBy = "Id", bool ascending = true)
      {
         string sql = Load("Search");

         if (!IsValidSongColumn(sortBy))
            throw new ArgumentException("Invalid column name specified for the sort.", nameof(sortBy));

         sql = string.Format(sql, sortBy, ascending ? "ASC" : "DESC");

         dynamic param = new
         {
            Query = query,
            Offset = page * PAGE_SIZE,
            Page = PAGE_SIZE,
         };

         IEnumerable<SongModel> songs = await SqliteDataAccess.Query<SongModel, dynamic>(sql, param);

         return songs.Select(Convert);
      }
      #endregion

      #region Helpers
      private static string GetPath(Uri source) => System.Web.HttpUtility.UrlDecode(source.LocalPath);
      private static bool IsValidSongColumn(string column)
      {
         // Definitely need to find a better way of doing this
         // adding cases in the SQL script doesn't seem like a better approach
         switch (column.ToLower())
         {
            case "id":
            case "name":
            case "duration":
            case "source":
               return true;

            default: 
               return false;
         }
      }
      private string Load(string action) => _DataAccess.Load($"Songs.{action}_Songs");
      #endregion
   }
}
