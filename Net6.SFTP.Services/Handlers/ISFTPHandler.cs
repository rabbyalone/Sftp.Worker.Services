using Net6.SFTP.Services.Models;
using Renci.SshNet.Sftp;

namespace Net6.SFTP.Services.Handlers
{
    public interface ISFTPHandler
    {
        Task<bool> Connect();
        void Disconnect();
        Task<IEnumerable<SftpFile>> CheckFileAvailablility(DateTime lastFileWriteDate);
        Task<List<SftpFileInfo>> DownloadFiles(IEnumerable<SftpFile> sftpFiles);
    }
}
