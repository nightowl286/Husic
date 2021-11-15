using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Husic.Standard.Playback;
using Models = Husic.DataAccess.Models;
using Internals = Husic.DataAccess.Internal.Models;
using System.Linq;
using Husic.Standard.DataAccess;
using System.Xml.Linq;

namespace Husic.DataAccess
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
            return new Models.SongModel(model.Id)
            {
               Duration = TimeSpan.FromSeconds(model.Duration),
               Name = model.Name,
               Source = new Uri(model.Source)
            };
         }

         string sql = LoadInternal("QueryTable_Songs");

         IEnumerable<Internals.SongModel> songs = await SqliteDataAccess.Query<Internals.SongModel>(sql);

         return songs.Select(Convert);
      }
      public void EnsureTables()
      {
         EnsureVersionsTable().Wait();
         Task[] tasks = new[]
         {
            EnsureTable("Songs", 1)
         };

         Task.WaitAll(tasks);
      }
      #endregion

      #region Functions
      private static async Task<int> GetTableVersion(string table)
      {
         string sql = LoadInternal("GetTableVersion");
         dynamic param = new { Name = table };
         IEnumerable<int> results = await SqliteDataAccess.Query<int, dynamic>(sql, param);
         return results.FirstOrDefault();
      }
      private static Task SetTableVersion(string table, int version)
      {
         string sql = LoadInternal("SetTableVersion");
         dynamic param = new { Name = table, Version = version };
         return SqliteDataAccess.Execute(sql, param);
      }
      private static Task EnsureVersionsTable() => EnsureTable("Versions", false);
      private static async Task EnsureTable(string tableName, int minVersion)
      {
         string sql;
         if (await Exists(tableName))
         {
            int version = await GetTableVersion(tableName);
            if (version >= minVersion)
               return;

            await SetTableVersion(tableName, minVersion);
            sql = LoadSql($"UpdateTable_{tableName}"); // might be possible to use LoadInternal
         }
         else
            sql = LoadSql($"CreateTable_{tableName}");

         await SqliteDataAccess.Execute(sql);
      }
      private static async Task EnsureTable(string tableName, bool update = true)
      {
         string sql;
         if (await Exists(tableName))
         {
            if (!update)
               return;
            sql = LoadSql($"UpdateTable_{tableName}");
         }
         else
            sql = LoadSql($"CreateTable_{tableName}");

         await SqliteDataAccess.Execute(sql);
      }
      private static async Task<bool> Exists(string name, string type = "table")
      {
         DynamicParameters param = new DynamicParameters();
         param.Add("@Name", name);
         param.Add("@Type", type);
         string sql = LoadSql("Exists");

         string result = (await SqliteDataAccess.Query<string,DynamicParameters>(sql, param)).FirstOrDefault();

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
         return _ContainingAssemblyName+ ".Internal.Scripts." + name + ".sqlite";
      }
      #endregion
   }
}
