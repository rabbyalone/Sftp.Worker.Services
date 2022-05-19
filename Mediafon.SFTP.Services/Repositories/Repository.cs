
using Mediafon.SFTP.Services.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Transactions;

namespace Mediafon.SFTP.Services.Repositories
{
    public class Repository<TEntity> :IRepository<TEntity> where TEntity : class
    {
        private readonly SftpInfoDb _context;
        public Repository(SftpInfoDb context)
        {
            _context =  context;
        }

        protected DbSet<TEntity> DbSet
        {
            get
            {
                return _context.Set<TEntity>();
            }
            set
            {
                value = _context.Set<TEntity>();
            }
        }       

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
           var data = await DbSet.FirstOrDefaultAsync(predicate);
           if (data == null)
                return Activator.CreateInstance<TEntity>();
           else
                return data;
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            DbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }      


    }
}
