using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SLQRCode.Generator;
using SLQRCode.Model;
using SLQRCode.Model.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HouseholdQRCodeGenerator.Tasks
{
  public class QRCodeFileDeleteTask : BackgroundService
  {
    Timer timer;
    IConfiguration configuration;
    public QRCodeFileDeleteTask(IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      if (!stoppingToken.IsCancellationRequested)
      {
        timer = new Timer(async obj =>
        {
          try
          {
            string dvsServerUrl = configuration["AppSettings:dvsServer"];
            if (string.IsNullOrWhiteSpace(dvsServerUrl))
            {
              Console.WriteLine("没有配置数据服务器");
            }
            HouseholdGenerator generator = new HouseholdGenerator(configuration);
            ResultInfo<List<FileCleanTask>> resultInfo = await generator.CleanFileTask();
            Console.WriteLine($"获取({dvsServerUrl})文件删除任务:" + JsonConvert.SerializeObject(resultInfo));
            foreach (FileCleanTask item in resultInfo.Data)
            {
              File.Delete(item.FilePath);
              await generator.NotifyFileDeleted(item.TaskId);
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(300));
      }

      await Task.CompletedTask;
    }
  }
}
