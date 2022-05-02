

namespace Mediafon.SFTP.Services.Repositories
{
    public interface IRepository<TEntity>: IDisposable where TEntity : class
    {
        Task<TEntity> CreateAsync(TEntity entity);
    }
}
