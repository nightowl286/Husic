using System.Threading.Tasks;

namespace Husic.Standard.DataAccess.Repositories
{
   public interface IKeyBasedRepository<TKey> : IRepository where TKey : notnull
   {
      #region Methods
      Task Delete(TKey key);
      #endregion
   }
   
   public interface IKeyBasedRepository<TKey, TData> : IKeyBasedRepository<TKey> where TKey : notnull
   {
      #region Methods
      Task<TData> Create(TData data);
      Task<TData> Get(TKey key);
      Task<TData> Update(TKey key, TData data);
      #endregion
   }
}
