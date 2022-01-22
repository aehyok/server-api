using AutoMapper;
using DVS.Common;
using DVS.Common.FFMpeg;
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
using OBS;
using OBS.Model;
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
    public class IHWOBSService : ServiceBase<SunFileInfo>, IFileService
    {
        IConfiguration configuration;
        IHttpClientFactory factory;
        string fileDir = string.Empty;
        public IHWOBSService(DbContext dbContext, IMapper mapper, IConfiguration configuration, IHttpClientFactory factory)
            : base(dbContext, mapper)
        {
            fileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploadfiles");
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
                //通知OBS删除原文件
                string store = configuration["File:Store"];
                string sunDFSUrl = configuration[$"File:{store}:DeleteUrl"];
                HttpResponseMessage httpResponse = await factory.PostAsync(sunDFSUrl, new { FileId = sunFileInfo.RefId, Sign = getSign(sunFileInfo) });
                string deleteResult = await httpResponse.Content.ReadAsStringAsync();
            }
        }


        private string getSign(SunFileInfo sunFileInfo)
        {

            string store = configuration["File:Store"];
            string signKey = configuration[$"File:{store}:SignKey"];
            if (signKey.IsNullOrWhiteSpace())
            {
                signKey = "FvjiH39cQtiYFKhaZjPX9kfUepVf5hh0";
            }
            return Utils.MD5(sunFileInfo.RefId + sunFileInfo.Md5.ToLower() + signKey);
        }


        public async Task<List<SunFileInfoDto>> UploadFiles(List<IFormFile> files, bool isMaterial, string src = "")
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
                long fileByteSize = file.Length;

                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                string filePath = Path.Combine(fileDir, DateTimeOffset.Now.ToUnixTimeMilliseconds() + file.FileName);
                using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                //计算MD5
                string md5 = Utils.GetMD5HashFromFile(filePath);

                FileUploadSunDFSResult uploadSunDFSResult = uploadFileToObs(originFileName, filePath);
                string thumbnailPath = fileType == FileType.Image ? uploadSunDFSResult.data.SavePath : "";
                SunFileInfo sunFileInfo = new SunFileInfo()
                {
                    Md5 = md5,
                    FileType = (int)fileType,
                    OriginName = originFileName,
                    ExtensionName = extenName,
                    RelativePath = uploadSunDFSResult.data.SavePath,
                    RefId = uploadSunDFSResult.data.FileId,
                    ByteSize = fileByteSize,
                    ThumbnailRelativePath = thumbnailPath,
                    IsMaterial = isMaterial
                };
                SunFileInfo insertFileInfo = await InsertAsync(sunFileInfo);
                SunFileInfoDto sunFileInfoDto = mapper.Map<SunFileInfoDto>(insertFileInfo);

                list.Add(sunFileInfoDto);
                // 如果是视频，通知转码
                if (FileType.Video == fileType && src != "transform")
                {

                    // 截图
                    VideoTransform videoTransform = new VideoTransform(factory, configuration);
                    thumbnailPath = Path.Combine(fileDir, DateTimeOffset.Now.ToUnixTimeMilliseconds() + ".jpg");
                    videoTransform.GetThumbnail(thumbnailPath, filePath);
                    //上传截图
                    FileUploadSunDFSResult uploadThumbnailSunDFSResult =   uploadFileToObs(originFileName, thumbnailPath);
                    File.Delete(thumbnailPath);
                    //保存信息
                    await GetQueryable().Where(f => f.Id == insertFileInfo.Id).UpdateFromQueryAsync(f => new SunFileInfo()
                    {
                        ThumbnailRelativePath = uploadThumbnailSunDFSResult.data.SavePath
                    });
                    sunFileInfo.ThumbnailRelativePath = uploadThumbnailSunDFSResult.data.SavePath;
                    SunFileInfoDto sunFileInfoDtoNew = mapper.Map<SunFileInfoDto>(insertFileInfo);
                    sunFileInfoDto.ThumbnailUrl = sunFileInfoDtoNew.ThumbnailUrl;

                    string store = configuration["File:Store"];
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

                File.Delete(filePath);
            }


            return list;

        }

        private FileUploadSunDFSResult uploadFileToObs(string originFileName, string filePath)
        {
            string store = configuration["File:Store"];
            string ak = configuration[$"File:{store}:AK"];
            string sk = configuration[$"File:{store}:SK"];
            string endPoint = configuration[$"File:{store}:EndPoint"];
            string bucketName = configuration[$"File:{store}:BucketName"];
   
            // 创建ObsClient实例
            ObsClient client = new ObsClient( ak,sk,endPoint);
            // 上传文件
            try
            {
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName =bucketName,
                    ObjectKey = originFileName,
                    FilePath = filePath,//待上传的本地文件路径，需要指定到具体的文件名
                };
                PutObjectResponse response = client.PutObject(request);
                FileUploadSunDFSResult uploadSunDFSResult = new FileUploadSunDFSResult()
                {
                    data = new SunDFSResultData()
                    {
                        FileId = "",
                        SavePath = originFileName
                    }
                };
                return uploadSunDFSResult;
            }
            catch (ObsException ex)
            {
                Console.WriteLine("ErrorMessage: {0}", ex.ErrorMessage);
                return null;
            }
        }
    }
}
