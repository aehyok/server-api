using DVS.Application.Schedules;
using DVS.Application.Services.Common;
using DVS.Application.Services.IPS;
using DVS.Common;
using DVS.Common.Http;
using DVS.Core.Domains.IPS;
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
    public class SyncIPSService : CronScheduleService
    {
        private readonly IHttpClientFactory factory;
        public SyncIPSService(ILogger<SyncIPSService> logger, IServiceProvider service, IHttpClientFactory factory)
            : base(logger, service)
        {
            this.factory = factory;
        }

        protected override string CronExpression => Configuration.GetValue<string>("Scheduler:SyncIPSService");

        protected override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            if (!await Utils.GetTaskExecuteLockAsync("SyncIPSService"))
            {
                this.Logger.LogInformation("未获取到锁(SyncIPSService)，下一次执行");
                return;
            }

            var url = Configuration.GetValue<string>("IPS:Url");
            if (url == null)
            {
                this.Logger.LogInformation("未配置IPS同步接口地址");
                return;
            }

            var companyId = Configuration.GetValue<string>("IPS:CompanyId");
            var messageUrl = $"{url}/api/DataSync/AddMessageSchedule";
            var scheduleUrl = $"{url}/api/DataSync/AddContentSchedule";
            using var scope = this.ServiceProvider.CreateScope();

            this.Logger.LogInformation(" +++++++++++++++++++++++同步滚动消息到IPS+++++++++++++++++++++");
            var messageService = scope.ServiceProvider.GetService<IIPSMessageService>();
            var messages = await messageService.GetListAsync(a => a.IsSyncToIPS == 0);
            foreach (var message in messages)
            {
                var syncBody = new
                {
                    appId = message.Id,
                    scheduleId = message.IpsMessageId,
                    scheduleName = message.Name,
                    devices = message.Devices.Split(",").ToList(),
                    content = message.Content,
                    isDelete = message.IsDeleted,
                    companyId = companyId,
                };

                var ipsMessageId = await PostAsync(messageUrl, syncBody);
                if (!string.IsNullOrWhiteSpace(ipsMessageId)){
                    int res = await messageService.GetQueryable().Where(a => a.Id == message.Id).UpdateFromQueryAsync(a => new IpsMessage()
                    {
                        IpsMessageId = ipsMessageId,
                        IsSyncToIPS = 1,
                    });
                }
            }

            this.Logger.LogInformation(" +++++++++++++++++++++++同步发布内容到IPS+++++++++++++++++++++");
            var scheduleService = scope.ServiceProvider.GetService<IIPSScheduleService>();
            var schedules = await scheduleService.GetListAsync(a => a.IsSyncToIPS == 0);
            var sunFileInfoService = scope.ServiceProvider.GetService<ISunFileInfoService>();
            foreach (var schedule in schedules)
            {
                List<SunFileInfoDto> list = new List<SunFileInfoDto>();
                if (schedule.Type == 1)
                {
                    list = await sunFileInfoService.GetSunFileInfoList(schedule.Images);
                }
                else {
                    list = await sunFileInfoService.GetSunFileInfoList(schedule.Videos);
                }

                List<Object> resource = new List<Object>();
                foreach (var item in list) {
                    resource.Add(new
                    {
                        name = item.OriginName, // 图片/视频名称
                        url = item.Url, // 资源的URL地址
                        size = item.ByteSize, // 大小
                        width = 0, // 宽
                        height = 0, // 高
                        mD5 = item.Md5, // MD5值
                        type = schedule.Type, // 资源类型 1 图片 2 视频
                        duration = 0, // 视频时长
                    });
                }
                var data = new
                {
                    appId = schedule.Id,
                    scheduleId = schedule.IpsScheduleId,
                    scheduleName = schedule.Name, //   日程名称
                    devices = schedule.Devices.Split(",").ToList(),
                    resourceType = schedule.Type, // 类型 1：图片、2：视频
                    intervalTime = schedule.Showduration,
                    resource = resource,
                    isDelete = schedule.IsDeleted,
                    companyId = companyId,
                };

                var ipsMessageId = await PostAsync(scheduleUrl, data);
                if (!string.IsNullOrWhiteSpace(ipsMessageId))
                {
                    await scheduleService.GetQueryable().Where(a => a.Id == schedule.Id).UpdateFromQueryAsync(a => new IpsSchedule()
                    {
                        IpsScheduleId = ipsMessageId,
                        IsSyncToIPS = 1,
                    });
                }
            }
        }

        private async Task<string> PostAsync(string url, Object data)
        {
            try
            {
                HttpResponseMessage response = await this.factory.PostAsync(url, data);
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (jo.GetValue("code").ToString() == "0")
                        return jo.GetValue("data").ToString();
                    else {
                        var SyncRes = jo != null ? jo.GetValue("message").ToString() : "同步失败";
                        this.Logger.LogInformation(SyncRes);
                        return "";
                    }
                }
                else
                {
                    var SyncRes = jo != null ? jo.GetValue("message").ToString() : "同步失败";
                    this.Logger.LogInformation(SyncRes);
                    return "";
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogInformation(ex.Message);
                return "";
            }
        }
    }

}