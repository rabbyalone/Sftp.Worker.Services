

using Mediafon.SFTP.Services.Config;
using Mediafon.SFTP.Services.Models;
using Mediafon.SFTP.Services.Services;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Mediafon.SFTP.Services.Handlers
{
    public class SFTPHandler : ISFTPHandler
    {
        private readonly SftpSettings _sftpSettings;
        private readonly ILogger<SFTPHandler> _logger;
        private readonly IHostEnvironment _hostingEnv;
        private SftpClient sftp;
        public bool Connected { get { return sftp.IsConnected; } }

        public SFTPHandler(IOptionsSnapshot<SftpSettings> sftpSettings, ILogger<SFTPHandler> logger, IHostEnvironment hostingEnv)
        {
            _sftpSettings = sftpSettings.Value;
            _logger = logger;
            var connectionInfo = new ConnectionInfo(_sftpSettings.SftpServer,
                        _sftpSettings.UserName,
                        new PasswordAuthenticationMethod(_sftpSettings.UserName, _sftpSettings.Password));
            sftp = new SftpClient(connectionInfo);
            _hostingEnv = hostingEnv;
        }
        public Task<bool> Connect()
        {
            try
            {
                bool IsConnected = false;   
                if (!Connected)
                {
                    if (string.IsNullOrEmpty(_sftpSettings.UserName)
                        && string.IsNullOrEmpty(_sftpSettings.Password) && string.IsNullOrEmpty(_sftpSettings.SftpServer))
                        throw new ArgumentException("Sever name, username and password are required to start application!");

                    sftp.Connect();
                    _logger.LogInformation("Connection established!");
                    IsConnected = true;
                }
                return Task.FromResult(IsConnected);

            }
            catch (Exception ex)
            {
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
                    _logger.LogInformation("Server Disconnected!!!=============\r\n");

                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to disconnect! {ex.Message}");
            }
        }

        public Task<IEnumerable<SftpFile>> CheckFileAvailablility(DateTime lastFileWriteDate)
        {
            if (string.IsNullOrEmpty(_sftpSettings.SftpFolderLocation))
                throw new ArgumentException("No remote folder location is specified");

            var files = sftp.ListDirectory(_sftpSettings.SftpFolderLocation);

            //checked against last db entry time and removed server files
            Func<SftpFile, bool> checkingCondition = a => !a.Name.StartsWith('.') && a.LastWriteTimeUtc > lastFileWriteDate;

            if (files.Any(checkingCondition))
            {
                _logger.LogInformation($"{files.Count(checkingCondition)} new file found in remote directory");
            }
            else
            {
                _logger.LogInformation($"{files.Count(checkingCondition)} new file found in remote directory");
            }

            return Task.FromResult(files.Where(checkingCondition));
        }

        public Task<List<SftpFileInfo>> DownloadFiles(IEnumerable<SftpFile> sftpFiles)
        {
            List<SftpFileInfo> sftpFileInfos = new List<SftpFileInfo>();

            try
            {

                string localPath = $"{_hostingEnv.ContentRootPath}/{_sftpSettings.LocalFolderLocation}/";

                foreach (var file in sftpFiles)
                {
                    string remoteFileName = file.Name;

                    //started downloading 
                    using (Stream file1 = File.OpenWrite(localPath + remoteFileName))
                    {
                        _logger.LogInformation($"File download started...");
                        sftp.DownloadFile(_sftpSettings.SftpFolderLocation + remoteFileName, file1);
                        _logger.LogInformation($"{remoteFileName} downloaded at {localPath}");
                    }

                    //building models with file info
                    var sftpFileInfo = new SftpFileInfo(remoteFileName)
                    {                        
                        LocalFilePath = $"{localPath}/{remoteFileName}",
                        LastWriteTime = file.LastWriteTimeUtc,
                        LastAccessTime = file.LastAccessTimeUtc,
                        FileDowloadTime = DateTime.UtcNow,
                        RemoteFilePath = _sftpSettings.SftpFolderLocation,
                        Id = Guid.NewGuid().ToString(),
                    };

                    sftpFileInfos.Add(sftpFileInfo);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Problem in downloading file {ex.Message}");
            }
            return Task.FromResult(sftpFileInfos);
        }
       
    }
}
