using AutoMapper.Configuration;
using Castle.Core.Logging;
using DVS.Application.Services.SunFSAgent;
using DVS.Common;
using DVS.Common.MQ;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using DVS.Models.Enum;
using DVS.Models.Models.MediaTransform;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DVS.Application.Schedules
{
   public  class SunfileMediaTransform: CronScheduleService
    {
        public SunfileMediaTransform(ILogger<SunfileMediaTransform> logger,  IServiceProvider service) : base(logger,service) { }

        protected override string CronExpression => Configuration["Scheduler:SunfileMediaTransform"];

        protected override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            bool taskLock= await Utils.GetTaskExecuteLockAsync("SunfileMediaTransform");
            if (taskLock) {
                using var scope = this.ServiceProvider.CreateScope();
                IServiceBase<SunFileInfo> service = scope.ServiceProvider.GetService<IServiceBase<SunFileInfo>>( )  ;
                SunFileInfo  sunFileInfo =  await service.GetQueryable().Where(file => file.IsTransformed == false && file.FileType == (int)FileType.Video).FirstOrDefaultAsync();
                string store = Configuration["File:Store"];
                if (sunFileInfo != null) {
                    SunFileInfoDto sunFileInfoDto = Mapper.Map<SunFileInfoDto>(sunFileInfo);
                    string notifyUrl = Configuration[$"File:{store}:NotifyTransformCompletedUrl"];
                    string completedUploadUrl = Configuration[$"File:{store}:TransformCompletedUploadUrl"];
                    if (!notifyUrl .IsNullOrWhiteSpace() &&!completedUploadUrl .IsNullOrWhiteSpace())
                    {
                        TransformTask task = new TransformTask()
                        {
                            Md5 = sunFileInfo.Md5,
                            DownloadUrl = sunFileInfoDto.Url,
                            UploadUrl = completedUploadUrl,
                            FileId = sunFileInfo.Id,
                            NotifyCompletedUrl = notifyUrl
                        };
                        task.SendTask(option =>
                        {
                            option.VirtualHost = Configuration["RabbitMQ:VirtualHost"];
                            option.Host = Configuration["RabbitMQ:Host"];
                            option.Port = int.Parse(Configuration["RabbitMQ:Port"]);
                            option.UserName = Configuration["RabbitMQ:UserName"];
                            option.Password = Configuration["RabbitMQ:Password"];
                        });
                    }
                }
            } 
        }
    }
}
