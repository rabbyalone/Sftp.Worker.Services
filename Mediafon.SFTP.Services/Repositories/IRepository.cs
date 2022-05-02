

using System.Linq.Expressions;

namespace Mediafon.SFTP.Services.Repositories
{
    public interface IRepository<TEntity>: IDisposable where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> CreateAsync(TEntity entity);
    }
}
