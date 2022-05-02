

using Mediafon.SFTP.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Mediafon.SFTP.Services.Context
{
    public class SftpInfoDb: DbContext
    {
        public SftpInfoDb(DbContextOptions<SftpInfoDb> options) : base(options)
        {

        }
        public DbSet<SftpFileInfo> SftpFileInfos => Set<SftpFileInfo>();
    }
}
