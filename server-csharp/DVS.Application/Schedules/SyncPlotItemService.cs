using DVS.Application.Schedules;
using DVS.Application.Services.GIS;
using DVS.Common;
using DVS.Models.Dtos.GIS.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sunlight.Application.Schedules
{
    public class SyncPlotItemService : CronScheduleService
    {
        public SyncPlotItemService(ILogger<SyncPlotItemService> logger, IServiceProvider service)
            : base(logger, service)
        { }

        protected override string CronExpression => Configuration.GetValue<string>("Scheduler:SyncPlotItemService");

        protected override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            if (!await Utils.GetTaskExecuteLockAsync("SyncPlotItemService"))
            {
                this.Logger.LogInformation("未获取到锁(SyncPlotItemService)，下一次执行");
                return;
            }

            var host = Configuration.GetValue<string>("BIGDATA:HOST");
            if ( host == null)
            {
                this.Logger.LogInformation("未配置同步接口地址");
                return;
            }
            using var scope = this.ServiceProvider.CreateScope();

            var plotitemService = scope.ServiceProvider.GetService<IGISPlotItemSyncService>();

            // 分类 1摄像头，2传感器，3区域地图，4农户标记，5土地标记，6公共设施，7规划用地，8大棚, 9自定义
            GISListQueryModel model = new GISListQueryModel()
            {
                
            };
            this.Logger.LogInformation(" +++++++++++++++++++++++地理信息打点数据同步到数字大屏+++++++++++++++++++++");
            var plotitems = await plotitemService.ListPlotItemAsync(model);

            foreach (var plotitem in plotitems)
            {
                await plotitemService.SyncToDVM(plotitem);
                this.Logger.LogInformation(plotitem.Name + "+++++++++++++++++++++");
            }
            if (plotitems.Count == 0) {
                this.Logger.LogInformation("没有同步内容+++++++++++++++++++++");
            }
        }
    }

}