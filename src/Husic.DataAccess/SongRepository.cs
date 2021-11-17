using Dapper;
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
   public class SongRepository : ISongRepository
   {
      #region Consts
      private const int PAGE_SIZE = 30;
         #endregion
      #region Private
      private readonly IDataAccess _DataAccess;
      #endregion
      public SongRepository(IDataAccess dataAccess)
      {
         _DataAccess = dataAccess;
      }

      #region Basic CRUD Methods
      public Task<ISong> CreateSong(ISong data) => throw new NotImplementedException();
      public Task<ISong> GetSong(int id) => throw new NotImplementedException();
      public Task<ISong> UpdateSong(int id, ISong data) => throw new NotImplementedException();
      public Task DeleteSong(int id) => throw new NotImplementedException();
      #endregion

      #region Methods
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
      private static ISong Convert(SongModel model)
      {
         return new Models.SongModel(model.Id)
         {
            Duration = TimeSpan.FromSeconds(model.Duration),
            Name = model.Name,
            Source = new Uri(model.Source)
         };
      }
      #endregion
   }
}
