using Microsoft.EntityFrameworkCore;
using Net6.SFTP.Services;
using Net6.SFTP.Services.Config;
using Net6.SFTP.Services.Context;
using Net6.SFTP.Services.Handlers;
using Net6.SFTP.Services.Repositories;
using Net6.SFTP.Services.Services;
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

            services.AddScoped<IProcessSftp, ProcessSftp>();
            services.AddScoped<DbContext, SftpInfoDb>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ISFTPHandler, SFTPHandler>();


        })
    .UseSerilog()
    //.UseDefaultServiceProvider(options => options.ValidateScopes = false)
    .Build();



await host.RunAsync();
