using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System.IO;

namespace Husic.DataAccess
{
   public class SqliteDataAccess
   {
      #region Variables
      public static string ConnectionString { get; set; }
      #endregion

      #region Methods
      public static Task<IEnumerable<T>> Query<T>(string sql) => Query<T, object>(sql, null);
      public static async Task<IEnumerable<T>> Query<T,U>(string sql, U parameters)
      {
         using (SqliteConnection conn = await Get())
         {
            return await conn.QueryAsync<T>(sql, parameters);
         }
      }
      public static async Task<T> QueryFirst<T,U>(string sql, U parameters)
      {
         using (SqliteConnection conn = await Get())
         {
            return await conn.QueryFirstAsync<T>(sql, parameters);
         }
      }
      public static Task<int> Execute(string sql) => Execute<object>(sql, null);
      public static async Task<int> Execute<U>(string sql, U parameters)
      {
         using (SqliteConnection conn = await Get())
         {
            return await conn.ExecuteAsync(sql, parameters);
         }
      }
      public static string LoadSql(string name)
      {
         Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
         foreach (Assembly assembly in assemblies)
         {
            if (assembly.GetName()?.Name.StartsWith("Husic") != true)
               continue;

            string[] names = assembly.GetManifestResourceNames();
            if (names.Contains(name))
               return LoadSql(name, assembly);
         }
         throw new FileNotFoundException("The specified resource file could not be found.",name);
      }
      public static string LoadSql(string name, Assembly assembly)
      {
         using (Stream resource = assembly.GetManifestResourceStream(name))
         {
            using (StreamReader sr = new StreamReader(resource))
            {
               string sql = sr.ReadToEnd();
               return sql;
            }
         }
      }


      private static async Task<SqliteConnection> Get()
      {
         using (SqliteConnection conn = new SqliteConnection(ConnectionString))
         {
            await conn.OpenAsync();
            return conn;
         }
      }
      #endregion
   }
}
