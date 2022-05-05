using Mediafon.SFTP.Services;
using Mediafon.SFTP.Services.Config;
using Mediafon.SFTP.Services.Context;
using Mediafon.SFTP.Services.Handlers;
using Mediafon.SFTP.Services.Repositories;
using Mediafon.SFTP.Services.Services;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using Serilog;



// Error handling
AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    Exception exception = (Exception)e.ExceptionObject;
    Console.WriteLine(exception.Message);
    Environment.Exit(-1);
};

Log.Logger = new LoggerConfiguration()
    .MinimumLevel
        .Information()
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
    .WriteTo
        .Console()
        .CreateLogger();


IHost host = Host.CreateDefaultBuilder(args)

    .ConfigureLogging((ctx, logging) =>
    {
        logging.AddConsole();
    })
    .ConfigureAppConfiguration((ctx, appConfiguration) =>
    {
        appConfiguration.SetBasePath(Directory.GetCurrentDirectory());
        appConfiguration.AddJsonFile("appsettings.json", false, true);
        appConfiguration.AddEnvironmentVariables();
    })
        .ConfigureServices((ctx, services) =>
        {
            services.AddHostedService<SFTPWorker>();
            services.AddEntityFrameworkNpgsql();
            services.Configure<SftpSettings>(options => ctx.Configuration.GetSection("SftpSettings").Bind(options));

            //connectionstring
            var connectionString = ctx.Configuration.GetConnectionString("SftpConnection");
            services.AddDbContext<SftpInfoDb>(options => options.UseNpgsql(connectionString));

            //DI

            services.AddTransient<DbContext, SftpInfoDb>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ISFTPHandler, SFTPHandler>();
            services.AddScoped<IProcessSftp, ProcessSftp>();


        })
    .UseSerilog()
    .UseDefaultServiceProvider(options => options.ValidateScopes = false)
    .Build();



await host.RunAsync();
