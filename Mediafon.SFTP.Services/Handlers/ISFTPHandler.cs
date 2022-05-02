
using Mediafon.SFTP.Services.Models;

namespace Mediafon.SFTP.Services.Handlers
{
    public interface ISFTPHandler
    {
        Task<bool> Connect();
        void Disconnect();
        Task<bool> CheckFileAvailablility();
        Task<List<SftpFileInfo>> ProcessFile();
    }
}
