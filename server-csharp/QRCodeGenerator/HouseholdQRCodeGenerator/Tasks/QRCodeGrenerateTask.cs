using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SLQRCode.Generator;
using SLQRCode.Model;
using SLQRCode.Model.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HouseholdQRCodeGenerator.Tasks
{
    public class QRCodeGrenerateTask : BackgroundService
    {
        public QRCodeGrenerateTask(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IConfiguration configuration;
        Timer timer = null;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                timer = new Timer(async obj =>
                 {
                     try
                     {
                         Console.WriteLine("开始执行二维码生成任务");
                         string dvsServerUrl = configuration["AppSettings:dvsServer"];
                         if (string.IsNullOrWhiteSpace(dvsServerUrl))
                         {
                             Console.WriteLine("没有配置数据服务器");
                         }

                         HouseholdGenerator generator = new HouseholdGenerator(configuration);
                         ResultInfo<QRCodeTask> resultInfo = await generator.GetQRCodeTask();
                         Console.WriteLine($"获取({dvsServerUrl})生成任务:" + JsonConvert.SerializeObject(resultInfo));
                         if (resultInfo != null && resultInfo.Data != null)
                         {
                             ResultInfo<bool> genResult = await generator.GenerateQRCode(resultInfo.Data);
                             if (genResult.Data)
                             {
                                 ResultInfo<int> result = await generator.UploadQRCode(resultInfo.Data.TaskId);
                                 ResultInfo<int> notifyResult= await generator.NotifyTaskComplete(resultInfo.Data.TaskId, result.Data);
                                 Console.WriteLine("通知完成任务："+ notifyResult.Message);
                             }

                         }
                     }
                     catch (Exception ex)
                     {
                         Console.WriteLine(ex.Message);
                     }
                 }, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
            }

            await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => { Console.WriteLine("已注销停止服务！"); });
        }
    }
}
