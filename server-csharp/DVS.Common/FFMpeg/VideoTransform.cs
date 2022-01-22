using DVS.Common.Http;
using DVS.Common.Infrastructures;
using DVS.Common.ModelDtos;
using DVS.Models.Dtos.Common;
using DVS.Models.Models.MediaTransform;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DVS.Common.FFMpeg
{
    public class VideoTransform
    {
        private readonly IHttpClientFactory factory;
        private readonly IConfiguration configuration;
        private readonly string ffmpegPath;

        public VideoTransform(IHttpClientFactory factory,IConfiguration configuration)
        {
            this.factory = factory;
            this.configuration = configuration;
            ffmpegPath = this.configuration["FFMpeg:Path"];
        }
        /// <summary>
        /// 获取视频的第一帧截图
        /// </summary>
        /// <param name="task"></param>
        public async Task GetVideoThumbnailAsync(TransformTask task)
        {
            try
            {
                using (Stream steam = await factory.GetStreamAsync(task.DownloadUrl))
                {
                    string dir = AppDomain.CurrentDomain.BaseDirectory;
                    string fileName = Path.GetFileName(task.DownloadUrl);
                    string fileNameWithExtension = Path.GetFileNameWithoutExtension(task.DownloadUrl);
                    string filePath = Path.Combine(dir, fileName);
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await steam.CopyToAsync(fileStream);

                        // 截取缩略图
                        string thumbnailFilePath = dir + fileNameWithExtension + ".jpg";
                        GetThumbnail(thumbnailFilePath,filePath);
                        // 获取视频信息
                        VideoFile videoFile = Ecoder.GetVideoInfo(filePath,ffmpegPath);
                        string h264FilePath = Path.Combine(dir, fileNameWithExtension + "." + DateTimeOffset.Now.ToUnixTimeMilliseconds()) + ".mp4";
                        string  cmd = $" -i {filePath}  -c:v copy -c:a copy {h264FilePath}  -loglevel error ";
                        // 如果不是H264，则进行转码
                        if (!videoFile.VideoFormat.ToLower().Contains("h264"))
                        {
                            cmd = $"-i {filePath} -metadata:s:v rotate=270 -c:v libx264 -c:a copy {h264FilePath} -loglevel error";
                        }
                        Ecoder.executeFFmpegCmd(cmd,ffmpegPath);
                        // 转码完成，上传文件
                        HttpResponseMessage thumbnailMessage = await factory.UploadFilesAsync(task.UploadUrl, thumbnailFilePath, "transform");
                        HttpResponseMessage transformVideoMessage = await factory.UploadFilesAsync(task.UploadUrl, h264FilePath, "transform");
                        string thumbnailStringResult = await thumbnailMessage.Content.ReadAsStringAsync();
                        string transformVideoStringResult = await transformVideoMessage.Content.ReadAsStringAsync();
                        ResultModel<List<SunFileInfoDto>> thumbnailResult = JsonSerializer.Deserialize<ResultModel<List<SunFileInfoDto>>>(thumbnailStringResult, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        ResultModel<List<SunFileInfoDto>> transformVideoResult = JsonSerializer.Deserialize<ResultModel<List<SunFileInfoDto>>>(transformVideoStringResult, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        // 文件上传完成，通知转码完成
                        await factory.PostAsync(task.NotifyCompletedUrl, new { Completed = true, FileId = task.FileId, ThumbnailId = thumbnailResult.Data[0].Id, TransformVideoId = transformVideoResult.Data[0].Id });
                        // 删除文件
                        File.Delete(thumbnailFilePath);
                        File.Delete(h264FilePath);
                        File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                await factory.PostAsync(task.NotifyCompletedUrl, new { Completed = false, FileId = task.FileId });
                Console.WriteLine(ex.Message);
            }

        }

        public  void GetThumbnail(string thumbnailFilePath, string filePath )
        {
            string cmd = "-i " + filePath + " -ss 00:00:01 -vframes 1 -an -y  -f mjpeg " + thumbnailFilePath;
            Ecoder.executeFFmpegCmd(cmd,ffmpegPath);
        }
    }
}
