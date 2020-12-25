using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using S7Svr.Simulator.MessageHandlers;
using S7Svr.Simulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace S7Svr.Simulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {



        public IHost _host { get; private set; }
        public IServiceProvider ServiceProvider { get; internal set; }

        private CancellationTokenSource cts = new CancellationTokenSource();

        public App()
        {
            this._host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder
                        .SetBasePath(context.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
                    builder.AddEnvironmentVariables();
                })
                .ConfigureLogging((ctx, logging) => {
                    if (ctx.HostingEnvironment.IsDevelopment())
                    {
                        logging.AddDebug();
                    }
                })
                .ConfigureServices((ctx, services) => {
                    this.ConfigureServices(ctx, services);
                })
                .Build();
            this.ServiceProvider = this._host.Services;
        }

        private void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            services.AddMediatR(typeof(MessageNotificationHandler).Assembly);
            services.AddSingleton<MainWindow>();
            services.AddSingleton<S7ServerService>();
            services.AddSingleton<MainVM>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                using (var scope = this.ServiceProvider.CreateScope())
                {
                    var sp = scope.ServiceProvider;
                    var env = sp.GetRequiredService<IHostEnvironment>();
                    var config = sp.GetRequiredService<IConfiguration>();
                    var logger = sp.GetRequiredService<ILogger<App>>();
                }
                var mainWin = this.ServiceProvider.GetRequiredService<MainWindow>();
                mainWin.Show();
                _ = Task.Run(async () =>
                {
                    await _host.RunAsync(cts.Token);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                var lieftime = _host.Services.GetRequiredService<IHostApplicationLifetime>();
                lieftime.StopApplication();
            }
            base.OnExit(e);
        }


    }
}
