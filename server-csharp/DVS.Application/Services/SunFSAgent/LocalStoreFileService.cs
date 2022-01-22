using AutoMapper;
using DVS.Common;
using DVS.Common.Http;
using DVS.Common.Models;
using DVS.Common.MQ;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.SunFSAgent;
using DVS.Models.Enum;
using DVS.Models.Models.MediaTransform;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DVS.Application.Services.SunFSAgent
{
    public class LocalStoreFileService : ServiceBase<SunFileInfo>, IFileService
    {

        IConfiguration configuration;
        IHttpClientFactory factory;

        public LocalStoreFileService(DbContext dbContext, IMapper mapper, IConfiguration configuration, IHttpClientFactory factory)
            : base(dbContext, mapper)
        {
            this.configuration = configuration;
            this.factory = factory;
        }

        public async Task TransformCompletedAsync(TransformCompletedInfo completedInfo)
        {
            if (completedInfo.Completed)
            {
                SunFileInfo transformVideo = await this.GetAsync(file => file.Id == completedInfo.TransformVideoId);
                SunFileInfo thumbnail = await this.GetAsync(file => file.Id == completedInfo.ThumbnailId);
                SunFileInfo sunFileInfo = await this.GetAsync(file => file.Id == completedInfo.FileId);
                await this.GetQueryable().Where(file => file.Id == completedInfo.FileId).UpdateFromQueryAsync(file => new SunFileInfo()
                {
                    ThumbnailId = thumbnail.Id,
                    ThumbnailRelativePath = thumbnail.RelativePath,
                    RelativePath = transformVideo.RelativePath,
                    Md5 = transformVideo.Md5,
                    ExtensionName = transformVideo.ExtensionName,
                    ByteSize = transformVideo.ByteSize,
                    IsTransformed = true
                });
                // 删除转码记录
                await this.DeleteAsync(completedInfo.TransformVideoId);
                //通知SunDFS删除原文件
                string store = configuration["File:Store"];
                string storePath = configuration[$"File:{store}:StorePath"];
                string toDeletePath = Path.Combine(storePath, sunFileInfo.ExtensionName);
                if (File.Exists(toDeletePath))
                {
                    File.Delete(toDeletePath);
                }
            }
        }

        public async Task<List<SunFileInfoDto>> UploadFiles(List<IFormFile> files, bool isMaterial = false, string src = "")
        {
            List<SunFileInfoDto> list = new List<SunFileInfoDto>();
            foreach (IFormFile file in files)
            {
                List<string> imageFilters = new List<string>() { ".jpg", ".jpeg", ".png", ".git" };
                List<string> zipFilters = new List<string>() { ".rar", ".tar.gz", ".zip" };
                List<string> textFilters = new List<string>() { ".txt", ".pdf", ".xlsx", ".xls", ".docx", ".doc" };
                List<string> videoFilters = new List<string>() { ".mp4", ".ts",".mov" };
                List<string> audioFilters = new List<string>() { ".mp3" };
                List<string> fileFilters = new List<string>();

                fileFilters.AddRange(imageFilters);
                fileFilters.AddRange(zipFilters);
                fileFilters.AddRange(textFilters);
                fileFilters.AddRange(videoFilters);
                fileFilters.AddRange(audioFilters);


                string originFileName = file.FileName;
                string extenName = Path.GetExtension(originFileName).ToLower();

                if (!fileFilters.Contains(extenName))
                {
                    throw new ValidException($"不允许上传{extenName}文件");
                }
                FileType fileType = FileType.UnKnown;
                if (imageFilters.Contains(extenName))
                {
                    fileType = FileType.Image;
                }
                if (audioFilters.Contains(extenName))
                {
                    fileType = FileType.Audio;
                }
                if (videoFilters.Contains(extenName))
                {
                    fileType = FileType.Video;
                }
                if (textFilters.Contains(extenName))
                {
                    fileType = FileType.Text;
                }
                if (zipFilters.Contains(extenName))
                {
                    fileType = FileType.Zip;
                }
                string store = configuration["File:Store"];
                long fileByteSize = file.Length;
                string dateDir = DateTime.Now.ToString("yyyyMMdd");
                string fileDir = Path.Combine(configuration[$"File:{store}:UploadDir"] ,  dateDir);
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                string fileName = DateTimeOffset.Now.ToUnixTimeMilliseconds() + extenName;
                string filePath = Path.Combine(fileDir, fileName);
                using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
      
                string storePath = configuration[$"File:{store}:AccessPath"];
                //计算MD5
                string md5 = Utils.GetMD5HashFromFile(filePath);

                SunFileInfo sunFileInfo = new SunFileInfo()
                {
                    Md5 = md5,
                    FileType = (int)fileType,
                    OriginName = originFileName,
                    ExtensionName = extenName,
                    RelativePath = Path.Combine(storePath, dateDir, fileName).Replace("\\","/"),
                    RefId = "",
                    ByteSize = fileByteSize,
                    ThumbnailRelativePath = fileType == FileType.Image ? Path.Combine(storePath, dateDir, fileName).Replace("\\", "/") : "",
                    IsMaterial = isMaterial
                };

                SunFileInfo insertFileInfo = await InsertAsync(sunFileInfo);
                SunFileInfoDto sunFileInfoDto = mapper.Map<SunFileInfoDto>(insertFileInfo);
                list.Add(sunFileInfoDto);
                // 如果是视频，通知转码
                if (FileType.Video == fileType && src != "transform")
                {
                    string noftifyUrl = configuration[$"File:{store}:NotifyTransformCompletedUrl"];
                    string completedUploadUrl = configuration[$"File:{store}:TransformCompletedUploadUrl"];
                    TransformTask task = new TransformTask()
                    {
                        Md5 = md5,
                        DownloadUrl = sunFileInfoDto.Url,
                        UploadUrl = completedUploadUrl,
                        FileId = insertFileInfo.Id,
                        NotifyCompletedUrl = noftifyUrl
                    };
                    task.SendTask(option =>
                    {
                        option.VirtualHost = configuration["RabbitMQ:VirtualHost"];
                        option.Host = configuration["RabbitMQ:Host"];
                        option.Port = int.Parse(configuration["RabbitMQ:Port"]);
                        option.UserName = configuration["RabbitMQ:UserName"];
                        option.Password = configuration["RabbitMQ:Password"];
                    });
                }
            }

            return list;
        }
    }
}
