using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Husic.DataAccess;
using Husic.Engine.Playback;
using Models = Husic.Engine.Models;
using Internals = Husic.Engine.DataAccess.Internal.Models;
using System.Linq;

namespace Husic.Engine.DataAccess
{
   public class DataAccess : IDataAccess
   {
      #region Private
      private static readonly Assembly _ContainingAssembly;
      private static readonly string _ContainingAssemblyName;
      #endregion
      static DataAccess()
      {
         SqliteDataAccess.ConnectionString = $"Data Source='data.db'";

         _ContainingAssembly = Assembly.GetAssembly(typeof(DataAccess));
         _ContainingAssemblyName = _ContainingAssembly.GetName().Name;
      }

      #region Methods
      public async Task<IEnumerable<ISong>> GetSongs()
      {
         static ISong Convert(Internals.SongModel model)
         {
            return new Models.SongModel()
            {
               Duration = TimeSpan.FromSeconds(model.Duration),
               Name = model.Name,
               Id = model.Id,
               Source = new Uri(model.Source)
            };
         }

         string sql = LoadInternal("QuerySongsTable");

         IEnumerable<Internals.SongModel> songs = await SqliteDataAccess.Query<Internals.SongModel>(sql);

         return songs.Select(Convert);
      }
      public void EnsureTables()
      {
         Task[] tasks = new[]
         {
            EnsureTable("Songs", false)
         };

         Task.WaitAll(tasks);
      }
      #endregion

      #region Functions
      private static async Task EnsureTable(string tableName, bool update = true)
      {
         string sql;
         if (await Exists(tableName))
         {
            if (!update)
               return;
            sql = LoadSql($"Update{tableName}Table");
         }
         else
            sql = LoadSql($"Create{tableName}Table");

         await SqliteDataAccess.Execute(sql);
      }
      private static async Task<bool> Exists(string name, string type = "table")
      {
         DynamicParameters param = new DynamicParameters();
         param.Add("@Name", name);
         param.Add("@Type", type);
         string sql = LoadSql("Exists");

         string result = await SqliteDataAccess.QueryFirst<string,DynamicParameters>(sql, param);

         return name.Equals(result, StringComparison.OrdinalIgnoreCase);

      }
      private static string LoadInternal(string name)
      {
         string fullName = GetName(name);
         return SqliteDataAccess.LoadSql(fullName, _ContainingAssembly);
      }
      private static string LoadSql(string name)
      {
         string fullName = GetName(name);
         return SqliteDataAccess.LoadSql(fullName);
      }
      private static string GetName(string name)
      {
         return _ContainingAssemblyName+ ".DataAccess.Internal.Scripts." + name + ".sqlite";
      }
      #endregion
   }
}
