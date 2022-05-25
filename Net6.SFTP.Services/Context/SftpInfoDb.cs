using Microsoft.EntityFrameworkCore;
using Net6.SFTP.Services.Models;

namespace Net6.SFTP.Services.Context
{
    public class SftpInfoDb : DbContext
    {
        public SftpInfoDb(DbContextOptions<SftpInfoDb> options) : base(options)
        {

        }
        public DbSet<SftpFileInfo> SftpFileInfos => Set<SftpFileInfo>();
    }
}
