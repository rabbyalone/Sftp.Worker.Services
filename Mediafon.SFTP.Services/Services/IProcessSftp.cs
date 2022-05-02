

namespace Mediafon.SFTP.Services.Services
{
    public interface IProcessSftp
    {
        Task<bool> FindFilesInSftp();
    }
}
