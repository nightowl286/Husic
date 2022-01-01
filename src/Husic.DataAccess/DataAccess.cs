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
using Husic.Standard.DataAccess.Scripts;
using Husic.DataAccess.Scripts;

namespace Husic.DataAccess
{
   public class DataAccess : IDataAccess
   {
      #region Private
      private static readonly IScriptContainer _Scripts;
      #endregion
      static DataAccess()
      {
         SqliteDataAccess.ConnectionString = $"Data Source='data.db'";

         _Scripts = new AssemblyScriptContainer(Assembly.GetExecutingAssembly(), "Internal.Scripts.{0}.sqlite");
      }

      #region Methods
      public void EnsureTables()
      {
         EnsureVersionsTable().Wait();
         Task[] tasks = new[]
         {
            EnsureTable("Songs", 1),
            EnsureTable("Playlists", 1),
            EnsureTable("Playlist_Entries", 1),
         };

         Task.WaitAll(tasks);
      }
      public string Load(string scriptName) => GetScript(scriptName);
      #endregion

      #region Functions
      private static async Task<int> GetTableVersion(string table)
      {
         string sql = GetScript("Tables.GetTableVersion");
         dynamic param = new { Name = table };
         IEnumerable<int> results = await SqliteDataAccess.Query<int, dynamic>(sql, param);
         return results.FirstOrDefault();
      }
      private static Task SetTableVersion(string table, int version)
      {
         string sql = GetScript("Tables.SetTableVersion");
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
            sql = GetScript($"Tables.Update.UpdateTable_{tableName}"); // might be possible to use LoadInternal
         }
         else
         {
            sql = GetScript($"Tables.Create.CreateTable_{tableName}");
            await SetTableVersion(tableName, minVersion);
         }

         await SqliteDataAccess.Execute(sql);
      }
      private static async Task EnsureTable(string tableName, bool update = true)
      {
         string sql;
         if (await Exists(tableName))
         {
            if (!update)
               return;
            sql = GetScript($"Tables.Update.UpdateTable_{tableName}");
         }
         else
            sql = GetScript($"Tables.Create.CreateTable_{tableName}");

         await SqliteDataAccess.Execute(sql);
      }
      private static async Task<bool> Exists(string name, string type = "table")
      {
         string sql = GetScript("Exists");

         dynamic param = new { Name = name, Type = type };

         IEnumerable<string> results = (await SqliteDataAccess.Query<string, dynamic>(sql, param));

         string result = results.FirstOrDefault();
         return name.Equals(result, StringComparison.OrdinalIgnoreCase);

      }
      public static string GetScript(string name) => _Scripts.GetScript(name);
      #endregion
   }
}
