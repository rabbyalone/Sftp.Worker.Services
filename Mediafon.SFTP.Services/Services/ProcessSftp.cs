

using Mediafon.SFTP.Services.Config;
using Mediafon.SFTP.Services.Handlers;
using Mediafon.SFTP.Services.Models;
using Mediafon.SFTP.Services.Repositories;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Mediafon.SFTP.Services.Services
{
    public class ProcessSftp : IProcessSftp
    {
        private readonly ISFTPHandler _handler;
        private readonly IRepository<SftpFileInfo> _repo;
        private readonly ILogger<ProcessSftp> _logger;

        public ProcessSftp(IRepository<SftpFileInfo> repo, ISFTPHandler handler, ILogger<ProcessSftp> logger)
        {
            _repo = repo;
            _handler = handler;
            _logger = logger;
        }
        public async Task<bool> ProcessFiles()
        {
            try
            {
                IEnumerable<SftpFile> sftpFiles;

                //connecting server
                bool connected = await _handler.Connect();

                if (connected)
                {
                    //getting last file entry time from database
                    DateTime lastWriteDate = await GetLastFileWriteTime();

                    //checking for new file in sftp 
                    sftpFiles = await _handler.CheckFileAvailablility(lastWriteDate);

                    if(sftpFiles.Any())
                    {
                        //download sftp file into local location and return all file info for db entry
                        List<SftpFileInfo> sftpFileInfos = await _handler.DownloadFiles(sftpFiles);

                        //Db entry of downloaded sftpFiles
                        foreach (SftpFileInfo fileInfo in sftpFileInfos)
                        {
                            var created = await _repo.CreateAsync(fileInfo);
                            _logger.LogInformation($"Created sftp file info entry in database with identifier {created.Id}");

                        }
                    }
                }     
               
                //disconnect
                _handler.Disconnect();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new ApplicationException($"Something went wrong! {ex.Message}");
            }
        }

        private async Task<DateTime> GetLastFileWriteTime()
        {
            var allFileDataFromDb = await _repo.GetAllAsync();
            var maxFileWritingDate =  allFileDataFromDb.OrderByDescending(a => a.LastWriteTime).FirstOrDefault()?.LastWriteTime;
            return maxFileWritingDate ?? DateTime.MinValue;
        }
    }
}
