

using Mediafon.SFTP.Services.Config;
using Mediafon.SFTP.Services.Handlers;
using Mediafon.SFTP.Services.Models;
using Mediafon.SFTP.Services.Repositories;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace Mediafon.SFTP.Services.Services
{
    public class ProcessSftp : IProcessSftp
    {
        private readonly ISFTPHandler _handler;
        private readonly IRepository<SftpFileInfo> _repo;
        public ProcessSftp(IRepository<SftpFileInfo> repo, ISFTPHandler handler)
        {
            _repo = repo;
            _handler = handler;
        }
        public async Task<bool> ProcessFiles()
        {
            try
            {
                bool IsNewFileAvailable = false;

                //connecting server
                bool connected = await _handler.Connect();

                if (connected)
                    IsNewFileAvailable = await _handler.CheckFileAvailablility();

                if (IsNewFileAvailable)
                {
                    //download sftp file into local location and return all file info for db entry
                    List<SftpFileInfo> sftpFileInfos = await _handler.ProcessFile();
                    foreach (SftpFileInfo fileInfo in sftpFileInfos)
                    {
                        await _repo.CreateAsync(fileInfo);
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
    }
}
