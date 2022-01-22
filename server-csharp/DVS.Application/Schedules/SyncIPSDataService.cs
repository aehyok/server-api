using DVS.Application.Services.IPS;
using DVS.Common;
using DVS.Core.Domains.IPS;
using DVS.Models.Dtos.IPS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DVS.Application.Schedules
{
    public class SyncIPSDataService : CronScheduleService
    {

        public SyncIPSDataService(ILogger<SyncIPSDataService> logger,
             IServiceProvider service
             )
            : base(logger, service)
        {
        }

        protected override string CronExpression => Configuration.GetValue<string>("Scheduler:SyncIPSDataService");


        protected override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            if (!await Utils.GetTaskExecuteLockAsync("SyncIPSDataService"))
            {
                this.Logger.LogInformation("未获取到锁(SyncIPSDataService)，下一次执行");
                return;
            }
            var url = Configuration.GetValue<string>("IPS:Url");
            var companyId = Configuration.GetValue<string>("IPS:CompanyId");
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(companyId))
            {
                this.Logger.LogInformation("未配置IPS的同步接口地址");
                return;
            }
            using var scope = this.ServiceProvider.CreateScope();

            var ts = scope.ServiceProvider.GetService<IHttpClientFactory>();
            var client = ts.CreateClient();


            #region  同步IPS的company数据
            this.Logger.LogInformation(" +++++++++++++++++++++++开始同步IPS的company数据+++++++++++++++++++++");
            try
            {
                var companyUrl = $"{url}/api/DataSync/GetCompanyList?CompanyId={companyId}&Page=1&Limit=500";
                var companyService = scope.ServiceProvider.GetService<IIPSCompanyService>();
                HttpResponseMessage response = await client.GetAsync(companyUrl);
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var code = jo.GetValue("code").ToString();
                    if (code == "0")
                    {
                        var n = jo.GetValue("data").ToString();
                        JObject ss = (JObject)JsonConvert.DeserializeObject(n);
                        var doc = ss.GetValue("docs").ToString();
                        if (!string.IsNullOrEmpty(doc))
                        {
                            var data = JsonConvert.DeserializeObject<List<IPSCompanyDto>>(doc);
                            foreach (var item in data)
                            {
                                var status = item.Status == 0 ? 1 : 0;    // IPS的0和1刚好相反，做一下转换
                                var info = await companyService.GetAsync(a => a.CompanyId == item.Id);
                                if (info != null)
                                {
                                    info.CompanyNo = item.CompanyNo;
                                    info.CompanyName = item.CompanyName;
                                    info.Status = status;
                                    info.ParentId = item.ParentId == null ? "" : item.ParentId;
                                    info.TopCompanyId = item.TopCompanyId == null ? "" : item.TopCompanyId;
                                    info.Category = item.Category;
                                    info.IsDeleted = item.IsDelete ? 1 : 0;
                                    info.UpdatedAt = DateTime.Now;
                                    await companyService.UpdateAsync(info);
                                }
                                else
                                {
                                    var entity = new IpsCompany();
                                    entity.CompanyId = item.Id;
                                    entity.CompanyNo = item.CompanyNo;
                                    entity.CompanyName = item.CompanyName;
                                    entity.Status = status;
                                    entity.ParentId = item.ParentId == null ? "" : item.ParentId;
                                    entity.TopCompanyId = item.TopCompanyId == null ? "" : item.TopCompanyId;
                                    entity.Category = item.Category;
                                    entity.IsDeleted = item.IsDelete ? 1 : 0;
                                    entity.CreatedAt = DateTime.Now;
                                    entity.UpdatedAt = DateTime.Now;
                                    await companyService.InsertAsync(entity);
                                }
                            }
                        }
                    }
                    else
                    {
                        this.Logger.LogInformation(" 同步IPS的company数据返回CODE为：" + code);
                    }
                }
                else
                {
                    this.Logger.LogInformation(" ###########++同步IPS的company数据服务器返回错误++###########");
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogInformation("同步IPS的company数据发生错误：" + ex.Message);

            }

            #endregion

            #region  同步IPS的device数据
            this.Logger.LogInformation(" +++++++++++++++++++++++开始同步IPS的device数据+++++++++++++++++++++");
            try
            {
                var deviceUrl = $"{url}/api/DataSync/GetDeviceList?CompanyId={companyId}&Page=1&Limit=500";
                var deviceService = scope.ServiceProvider.GetService<IIPSDeviceService>();
                HttpResponseMessage response = await client.GetAsync(deviceUrl);
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var code = jo.GetValue("code").ToString();
                    if (code == "0")
                    {
                        var n = jo.GetValue("data").ToString();
                        JObject ss = (JObject)JsonConvert.DeserializeObject(n);
                        var doc = ss.GetValue("docs").ToString();
                        if (!string.IsNullOrEmpty(doc))
                        {
                            var data = JsonConvert.DeserializeObject<List<IPSDeviceDto>>(doc);
                            foreach (var item in data)
                            {
                                var status = item.Status == 0 ? 1 : 0;    // IPS的0和1刚好相反，做一下转换
                                var info = await deviceService.GetAsync(a => a.DeviceId == item.Id);
                                if (info != null)
                                {
                                    info.DeviceNo = item.DeviceNo;
                                    info.DeviceName = item.DeviceName;
                                    info.Status = status;
                                    info.CompanyId = item.CompanyId;
                                    info.Category = item.Category;
                                    info.IsDeleted = item.IsDelete ? 1 : 0;
                                    info.UpdatedAt = DateTime.Now;
                                    await deviceService.UpdateAsync(info);
                                }
                                else
                                {
                                    var entity = new IpsDevice();
                                    entity.DeviceId = item.Id;
                                    entity.DeviceNo = item.DeviceNo;
                                    entity.DeviceName = item.DeviceName;
                                    entity.Status = status;
                                    entity.CompanyId = item.CompanyId;
                                    entity.Category = item.Category;
                                    entity.IsDeleted = item.IsDelete ? 1 : 0;
                                    entity.CreatedAt = DateTime.Now;
                                    entity.UpdatedAt = DateTime.Now;
                                    await deviceService.InsertAsync(entity);
                                }
                            }
                        }
                    }
                    else
                    {
                        this.Logger.LogInformation(" 同步IPS的device数据返回CODE为：" + code);
                    }
                }
                else
                {
                    this.Logger.LogInformation(" ###########++同步IPS的device数据服务器返回错误++###########");
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogInformation("同步IPS的device数据发生错误：" + ex.Message);

            }

            #endregion
        }
    }
}
