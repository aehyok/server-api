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
    public class SyncVillageService : CronScheduleService
    {
        public SyncVillageService(ILogger<SyncVillageService> logger, IServiceProvider service)
            : base(logger, service)
        { }

        protected override string CronExpression => Configuration.GetValue<string>("Scheduler:SyncVillageService");

        protected override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            if (!await Utils.GetTaskExecuteLockAsync("SyncVillageService"))
            {
                this.Logger.LogInformation("未获取到锁(SyncVillageService)，下一次执行");
                return;
            }

            var host = Configuration.GetValue<string>("BIGDATA:HOST");
            if (host == null)
            {
                this.Logger.LogInformation("未配置同步接口地址");
                return;
            }
            using var scope = this.ServiceProvider.CreateScope();

            var service = scope.ServiceProvider.GetService<IVillageSyncService>();

            PostBody model = new PostBody()
            {

            };
            this.Logger.LogInformation(" +++++++++++++++++++++++乡村治理数据同步到数字大屏+++++++++++++++++++++");
            var households = await service.ListHouseHoldAsync(model);

            foreach (var household in households)
            {
                await service.SyncHouseHoldToDVM(household);
                this.Logger.LogInformation("++++++++++++++++++++++户码数据" + household.HouseName + "+++++++++++++++++++++");
            }

            var populations = await service.ListPopulationAsync(model);
            foreach (var population in populations)
            {
                await service.SyncPersonToDVM(population);
                this.Logger.LogInformation("++++++++++++++++++++++户籍人口" + population.RealName + "+++++++++++++++++++++");
            }

            var epidemics = await service.ListEpidemicAsync(model);
            foreach (var epidemic in epidemics)
            {
                await service.SyncEpidemicToDVM(epidemic);
                this.Logger.LogInformation("++++++++++++++++++++++防疫情况" + epidemic.Year + "+++++++++++++++++++++");
            }

            var incomes = await service.ListIncomeAsync(model);
            foreach (var income in incomes)
            {
                await service.SyncIncomeToDVM(income);
                this.Logger.LogInformation("++++++++++++++++++++++家庭收入" + income.Year + "+++++++++++++++++++++");
            }

            var works = await service.ListWorkAsync(model);
            foreach (var work in works)
            {
                await service.SyncWorkToDVM(work);
                this.Logger.LogInformation("++++++++++++++++++++++外出务工" + work.Year + "+++++++++++++++++++++");
            }
        }
    }

}