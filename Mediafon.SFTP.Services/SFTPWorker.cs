using Mediafon.SFTP.Services.Services;

namespace Mediafon.SFTP.Services
{
    public class SFTPWorker : BackgroundService
    {
        private readonly ILogger<SFTPWorker> _logger;
        private readonly IServiceProvider _serviceProvider ;
        private IProcessSftp _readSftp;


        public SFTPWorker(ILogger<SFTPWorker> logger, IServiceProvider serviceProvider, IProcessSftp readSftp)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                using var scope = _serviceProvider.CreateScope();
                _readSftp = scope.ServiceProvider.GetRequiredService<IProcessSftp>();
                await _readSftp.FindFilesInSftp();

                await Task.Delay(1000, stoppingToken);

                
            }
        }
    }
}