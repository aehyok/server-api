using DVS.Application.Schedules;
using DVS.Application.Services.Common;
using DVS.Application.Services.IPS;
using DVS.Application.Services.Village;
using DVS.Common;
using DVS.Common.Http;
using DVS.Core.Domains.IPS;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sunlight.Application.Schedules
{
    public class SyncFFPService : CronScheduleService
    {        
        public SyncFFPService(ILogger<SyncFFPService> logger, IServiceProvider service)
            : base(logger, service)
        {
        }

        protected override string CronExpression => Configuration.GetValue<string>("Scheduler:SyncFFPService");

        protected override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            if (!await Utils.GetTaskExecuteLockAsync("SyncFFPService"))
            {
                this.Logger.LogInformation("未获取到锁(SyncFFPService)，下一次执行");
                return;
            }

            using var scope = this.ServiceProvider.CreateScope();

            this.Logger.LogInformation(" +++++++++++++++++++++++每月更新摸排状态+++++++++++++++++++++");
            var messageService = scope.ServiceProvider.GetService<IHouseholdCodeService>();
            await messageService.GetQueryable().Where(a=>a.IsDeleted == 0).UpdateFromQueryAsync(a=> new VillageHouseholdCode(){
                IsMoPai = 0,
            });
        }
    }
}