using AutoMapper;
using DVS.Core;
using Lychee.Core.Infrastructures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DVS.Application.Schedules
{
    public abstract class CronScheduleService : BackgroundService
    {
        protected readonly ILogger Logger;
        protected readonly IConfiguration Configuration;
        protected readonly IMapper Mapper;
        protected readonly IServiceProvider ServiceProvider;

        protected abstract string CronExpression { get; }

        protected abstract Task ProcessAsync(CancellationToken cancellationToken);

        public CronScheduleService(ILogger logger, IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
            this.Configuration = serviceProvider.GetService<IConfiguration>();
            this.Logger = logger;
            this.Mapper = serviceProvider.GetService<IMapper>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (CronExpression == ""|| CronExpression == null) {
                return;
            }
            var nextExcuteTime = CronHelper.GetNextOccurrence(CronExpression, Cronos.CronFormat.IncludeSeconds);

            if (nextExcuteTime.HasValue)
            {
                this.Logger.LogInformation($"下次执行时间:{nextExcuteTime.Value.ToLocalTime()}");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTimeOffset.UtcNow < nextExcuteTime)
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }
                ServiceLocator.Current.ResolverFunc = (type ) =>
                {
                    using var scope = this.ServiceProvider.CreateScope();
                    return scope.ServiceProvider.GetService(type);
                };

                await ProcessAsync(stoppingToken);

                nextExcuteTime = CronHelper.GetNextOccurrence(CronExpression, Cronos.CronFormat.IncludeSeconds);

                if (nextExcuteTime.HasValue)
                {
                    this.Logger.LogInformation($"下次执行时间:{nextExcuteTime.Value.ToLocalTime()}");                    
                }
            }
        }
    }
}