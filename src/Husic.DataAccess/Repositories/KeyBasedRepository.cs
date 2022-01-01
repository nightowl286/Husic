using Dapper;
using Husic.Standard.DataAccess.Repositories;
using Husic.Standard.DataAccess.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Husic.DataAccess.Repositories
{
   public abstract class KeyBasedRepository<TKey, TData> : IKeyBasedRepository<TKey, TData> where TKey : notnull
   {
      #region Other
      public const int DEFAULT_PAGE_SIZE = 30;
      public const string SCRIPT_NAME_CREATE = "Create";
      public const string SCRIPT_NAME_GET = "Get";
      public const string SCRIPT_NAME_UPDATE = "Update";
      public const string SCRIPT_NAME_DELETE = "Delete";
      protected readonly IScriptContainer _Scripts;
      #endregion

      #region Properties
      public int PageSize { get; protected set; } = DEFAULT_PAGE_SIZE;
      public string KeyColumn { get; protected set; }
      #endregion
      public KeyBasedRepository(IScriptContainer scriptContainer, string keyColumn = "Id")
      {
         _Scripts = scriptContainer;
         KeyColumn = keyColumn;
      }

      #region Methods
      protected abstract Task<TData> Create(TData data, string script);
      protected abstract Task<TData> Get(TKey key, string script);
      protected abstract Task<TData> Update(TKey key, TData data, string script);
      protected virtual Task Delete(TKey key, string script)
      {
         DynamicParameters param = new DynamicParameters();
         param.Add(KeyColumn, key);

         return SqliteDataAccess.Execute(script, param);
      }

      public Task<TData> Create(TData data)
      {
         string script = _Scripts.GetScript(SCRIPT_NAME_CREATE);
         return Create(data, script);
      }
      public Task<TData> Get(TKey key)
      {
         string script = _Scripts.GetScript(SCRIPT_NAME_GET);
         return Get(key, script);
      }
      public Task<TData> Update(TKey key, TData data)
      {
         string script = _Scripts.GetScript(SCRIPT_NAME_UPDATE);
         return Update(key, data, script);
      }
      public Task Delete(TKey key)
      {
         string script = _Scripts.GetScript(SCRIPT_NAME_DELETE);
         return Delete(key, script);
      }
      #endregion

      #region Helpers
      protected Task<T> GetFirst<T>(TKey key, string script)
      {
         DynamicParameters param = new DynamicParameters();
         param.Add(KeyColumn, key);

         return SqliteDataAccess.QueryFirst<T, DynamicParameters>(script, param);
      }
      #endregion
   }
}
