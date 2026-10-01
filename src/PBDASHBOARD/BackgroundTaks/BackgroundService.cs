using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PBDASHBOARD.Data;

namespace PBDASHBOARD.BackgroundTaks
{
    public class BackgroundService : ServiceTask, IHostedService, IDisposable
    {
        private Timer _timer;
        private static bool _isRun;
        private readonly ILogger<BackgroundService> _logger;


        public BackgroundService(IHttpClientFactory clientFactory, IServiceProvider serviceProvider,
            ILogger<BackgroundService> logger) : base(
            clientFactory,
            serviceProvider)
        {
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }


        public Task StartAsync(CancellationToken cancellationToken)

        {
            _logger.LogInformation("Starting IHostedService registered in Startup");
            _timer = new Timer(BackgroundWorks, null, TimeSpan.Zero, TimeSpan.FromDays(30));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping IHostedService registered in Startup");

            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async Task runOnce()
        {
            while (!_isRun)
            {
                reset(); // starts the program by first assigning firstyear 2017.
                await GetResearchGroupFromUia();
                await MapUnits();
                await MapCategories();
                _isRun = isRunOnce(); // sets isrun to true after successfully reaching here. 
            }
        }

        private async void BackgroundWorks(object o)
        {
            using var serviceScope = ServiceProvider.CreateScope();
            DbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
            
            try
            {
                _isRun = getIsrunOnce();
                if (!_isRun)
                {
                    await runOnce();
                }

                if (setDatePublished() )
                {
                    await MapPublications();
                    await MapProjects();
                    UpdatePublishedYear(getDateTimeNow); // this should fetch the lastsync year from database. 
                }
                else
                {
                    if (setDatePublished())
                    {
                        await MapPublications();
                        await MapProjects();
                        UpdatePublishedYear(getDateTimeNow); // this should fetch the lastsync year from database. 
                    }
                }
            }
            catch (TaskCanceledException exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
           
        }
    }
}