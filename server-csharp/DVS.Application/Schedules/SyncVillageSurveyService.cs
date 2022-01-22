using DVS.Application.Schedules;
using DVS.Application.Services.Village;
using DVS.Common;
using DVS.Models.Dtos.Village.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sunlight.Application.Schedules
{
    public class SyncVillageSurveyService : CronScheduleService
    {
        public SyncVillageSurveyService(ILogger<SyncVillageSurveyService> logger, IServiceProvider service)
            : base(logger, service)
        { }

        protected override string CronExpression => Configuration.GetValue<string>("Scheduler:SyncVillageSurveyService");

        protected override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            if (!await Utils.GetTaskExecuteLockAsync("SyncVillageSurveyService"))
            {
                this.Logger.LogInformation("未获取到锁(SyncVillageSurveyService)，下一次执行");
                return;
            }

            var host = Configuration.GetValue<string>("BIGDATA:HOST");
            if (host == null)
            {
                this.Logger.LogInformation("未配置同步接口地址");
                return;
            }
            using var scope = this.ServiceProvider.CreateScope();

            var service = scope.ServiceProvider.GetService<IVillageSyncSurveyService>();

            this.Logger.LogInformation(" +++++++++++++++++++++++乡村概况同步到数字大屏+++++++++++++++++++++");
            await service.SyncVillageSurveyToDVM();
        }
    }

}