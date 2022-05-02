

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
        public async Task<bool> FindFilesInSftp()
        {
            bool IsNewFileAvailable = false;
            bool connected = await _handler.Connect();
            if (connected)
               IsNewFileAvailable = await _handler.CheckFileAvailablility();
            if (IsNewFileAvailable)
            {
                List<SftpFileInfo> sftpFileInfos = await _handler.ProcessFile();
                foreach (SftpFileInfo fileInfo in sftpFileInfos)
                {
                    await _repo.CreateAsync(fileInfo);
                }
            }

            return connected;
        }

        public async Task<bool> CheckDb()
        {
            var sf = new SftpFileInfo
            {
                FileName = "Test",
                FilePath = "hfadsl",
                MovingTime = DateTime.Today,
                Id = Guid.NewGuid().ToString()
            };
            await _repo.CreateAsync(sf);

            return await FindFilesInSftp();
        }
    }
}
