

using Mediafon.SFTP.Services.Config;
using Mediafon.SFTP.Services.Models;
using Mediafon.SFTP.Services.Services;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace Mediafon.SFTP.Services.Handlers
{
    public class SFTPHandler : ISFTPHandler
    {
        private readonly IOptions<SftpSettings> _sftpSettings;
        private readonly ILogger<ProcessSftp> _logger;
        private SftpClient sftp;
        public bool Connected { get { return sftp.IsConnected; } }

        public SFTPHandler(IOptions<SftpSettings> sftpSettings, ILogger<ProcessSftp> logger)
        {
            _sftpSettings = sftpSettings;
            _logger = logger;
            var connectionInfo = new ConnectionInfo(_sftpSettings.Value.SftpServer,
                        _sftpSettings.Value.UserName,
                        new PasswordAuthenticationMethod(_sftpSettings.Value.UserName, _sftpSettings.Value.Password));
            sftp = new SftpClient(connectionInfo);
        }
        public Task<bool> Connect()
        {
            try
            {
                if (!Connected)
                {
                    sftp.Connect();
                    _logger.LogInformation("Connection established!");
                }
                return Task.FromResult(true);

            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
                throw new ArgumentException($"Connection not established! {ex.Message}");
            }
        }

        public void Disconnect()
        {
            try
            {
                if (sftp != null && Connected)
                {
                    sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to disconnect! {ex.Message}");
            }
        }

        public Task<bool> CheckFileAvailablility()
        {

            var files = sftp.ListDirectory(_sftpSettings.Value.SftpFolderLocation);
            if (files.Any(a=> !a.Name.StartsWith('.')))
            {
                _logger.LogInformation($"{files.Count(a => !a.Name.StartsWith('.'))} new file found in remote directory");
                return Task.FromResult(true);
            }
            else
                return Task.FromResult(false);
        }

        public Task<List<SftpFileInfo>> ProcessFile()
        {
            List<SftpFileInfo> sftpFileInfos = new List<SftpFileInfo>();

            var files = sftp.ListDirectory(_sftpSettings.Value.SftpFolderLocation);

            foreach (var file in files.Where(a => !a.Name.StartsWith('.')))
            {
                string remoteFileName = file.Name;

                using (Stream file1 = File.OpenWrite(_sftpSettings.Value.LocalFolderLocation + "/Downloads/" + remoteFileName))
                {
                    sftp.DownloadFile(remoteFileName, file1);
                }
                var sftpFileInfo = new SftpFileInfo
                {
                    FileName = remoteFileName,
                    FilePath = _sftpSettings.Value.LocalFolderLocation + remoteFileName,
                    MovingTime = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString(),
                };
                sftpFileInfos.Add(sftpFileInfo);
            }
            return Task.FromResult(sftpFileInfos);
        }
       
    }
}
