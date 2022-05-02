
using Mediafon.SFTP.Services.Models;
using Renci.SshNet.Sftp;

namespace Mediafon.SFTP.Services.Handlers
{
    public interface ISFTPHandler
    {
        Task<bool> Connect();
        void Disconnect();
        Task<IEnumerable<SftpFile>> CheckFileAvailablility(DateTime lastFileWriteDate);
        Task<List<SftpFileInfo>> DownloadFiles(IEnumerable<SftpFile> sftpFiles);
    }
}
