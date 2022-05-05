using Mediafon.SFTP.Services.Services;

namespace Mediafon.SFTP.Services
{
    public class SFTPWorker : BackgroundService
    {
        private readonly ILogger<SFTPWorker> _logger;
        private readonly IServiceProvider _serviceProvider ;


        public SFTPWorker(ILogger<SFTPWorker> logger, IServiceProvider serviceProvider, IProcessSftp readSftp)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Sftp service running at: {time}", DateTimeOffset.Now);

                using var scope = _serviceProvider.CreateScope();
                IProcessSftp _processSftpFiles = scope.ServiceProvider.GetRequiredService<IProcessSftp>();
                await _processSftpFiles.ProcessFiles();

                //Start every 1 minute interval
                await Task.Delay(60000, stoppingToken);                
            }
        }
    }
}