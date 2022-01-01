using Dapper;
using Husic.DataAccess.Cache;
using Husic.DataAccess.Internal.Models;
using Husic.Standard.DataAccess.Repositories;
using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husic.DataAccess.Repositories
{
   public class SongRepository : KeyBasedRepository<int, ISong>, ISongRepository
   {
      #region Private
      private readonly Cache.DataCache<int, ISong> _Cache = new Cache.DataCache<int, ISong>();
      #endregion
      public SongRepository() : base()
      {
         CreateAssemblyScriptContainer("Internal.Scripts.Songs.{0}_Songs.sqlite");
      }

      #region Basic CRUD Methods
      protected override async Task<ISong> Create(ISong data, string script)
      {
         dynamic param = new
         {
            data.Name,
            Duration = data.Duration.TotalSeconds,
            Source = GetPath(data.Source)
         };

         int songId = await SqliteDataAccess.QueryFirst<int, dynamic>(script, param);

         // Maybe change this, unsure which approach is better, but this
         // seems to reduce duplicate code for determining which columns
         // will be received
         ISong song = await Get(songId);

         _Cache.AddWeak(songId, song);

         return song;
      }
      protected override async Task<ISong> Get(int id, string script)
      {
         if (_Cache.TryGet(id, out ISong cachedSong))
            return cachedSong;

         SongModel song = await GetFirst<SongModel>(id, script);

         ISong converted = Convert(song);

         return converted;
      }
      protected override async Task<ISong> Update(int id, ISong data, string script)
      {
         dynamic param = new
         {
            Id = id,
            data.Name,
            Source = GetPath(data.Source),
            Duration = data.Duration.TotalSeconds
         };

         await SqliteDataAccess.Execute(script, param);
         if (_Cache.TryGet(id, out ISong cachedData))
         {
            cachedData.Name = data.Name;
            cachedData.Source = data.Source;
            cachedData.Duration = data.Duration;
            return cachedData;
         }
         else
         {
            ISong updatedSong = await Get(id);
            _Cache.AddWeak(updatedSong.Id, updatedSong);
            return updatedSong;
         }
      }
      protected override Task Delete(int id, string script)
      {
         _Cache.Delete(id);

         return base.Delete(id, script);  
      }
      #endregion

      #region Methods
      public Task SaveUpdatedDuration(ISong song)
      {
         string sql = GetScript("UpdateDuration");
         dynamic param = new
         {
            song.Id,
            Duration = song.Duration.TotalSeconds
         };

         Debug.Assert(_Cache.TryGet(song.Id, out ISong _));
         return SqliteDataAccess.Execute(sql, param);
      }
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

         _Cache.AddWeak(song.Id, song);
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
         string sql = GetScript("Query");

         if (!IsValidSongColumn(sortBy))
            throw new ArgumentException("Invalid column name specified for the sort.", nameof(sortBy));

         sql = string.Format(sql, sortBy, ascending ? "ASC" : "DESC");

         dynamic param = new
         {
            Offset = page * PageSize,
            Page = PageSize,
         };

         IEnumerable<SongModel> songs = await SqliteDataAccess.Query<SongModel, dynamic>(sql, param);

         return songs.Select(Convert);
      }
      public async Task<IEnumerable<ISong>> SearchSongs(string query, uint page, string sortBy = "Id", bool ascending = true)
      {
         string sql = GetScript("Search");

         if (!IsValidSongColumn(sortBy))
            throw new ArgumentException("Invalid column name specified for the sort.", nameof(sortBy));

         sql = string.Format(sql, sortBy, ascending ? "ASC" : "DESC");

         dynamic param = new
         {
            Query = query,
            Offset = page * PageSize,
            Page = PageSize,
         };

         IEnumerable<SongModel> songs = await SqliteDataAccess.Query<SongModel, dynamic>(sql, param);

         return songs.Select(Convert);
      }
      #endregion

      #region Helpers
      private static string GetPath(Uri? source)
      {
         if (source == null)
            throw new ArgumentNullException("Can't get the path of a null uri.", nameof(source));

         return System.Web.HttpUtility.UrlDecode(source.LocalPath);
      }
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
      #endregion
   }
}
