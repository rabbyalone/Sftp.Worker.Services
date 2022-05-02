
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Mediafon.SFTP.Services.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DbContext _context;
        public Repository(DbContext context)
        {
            _context = context;
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
        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            DbSet.Add(entity);
            await SaveChangesAsync();
            return entity;
        }

        public async Task<int> SaveChangesAsync()
        {

            try
            {
                int result;
                var addedEntries = _context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();

                using (var scope = new TransactionScope(TransactionScopeOption.Required,
                        new System.TimeSpan(0, 30, 0), TransactionScopeAsyncFlowOption.Enabled))
                {
                    bool saveFailed;
                    do
                    {
                        try
                        {
                            result = await _context.SaveChangesAsync();
                            saveFailed = false;
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;
                            result = 0;
                            if (ex.Entries != null && ex.Entries.Any())
                            {
                                ex.Entries.ToList()
                                    .ForEach(entry =>
                                    {
                                        entry.OriginalValues.SetValues(entry.CurrentValues);
                                    });
                            }

                        }
                    } while (saveFailed);
                    scope.Complete();
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
