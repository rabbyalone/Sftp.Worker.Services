
using Mediafon.SFTP.Services.Models;

namespace Mediafon.SFTP.Services.Handlers
{
    public interface ISFTPHandler
    {
        Task<bool> Connect();
        void Disconnect();
        Task<bool> CheckFileAvailablility(DateTime lastWriteDate);
        Task<List<SftpFileInfo>> ProcessFile();
    }
}
