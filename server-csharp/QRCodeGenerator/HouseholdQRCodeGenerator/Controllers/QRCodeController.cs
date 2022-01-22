using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SLQRCode.Generator;
using SLQRCode.Model;

namespace HouseholdQRCodeGenerator.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QRCodeController : ControllerBase
    {
        IConfiguration configuration;
        public QRCodeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public object Get([FromServices] ILogger<QRCodeController> logger)
        {
            logger.LogInformation($"Logger :{DateTime.Now}");
            Console.WriteLine($"Date:{DateTime.Now}");
            return new { Date = DateTime.Now };
        }
        /// <summary>
        /// 获取户码，有模板
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> GetHouseholdCode([FromBody] HouseholdeTask task)
        {
            QRCodeTask qrTask = new QRCodeTask();
            qrTask.TaskId = task.TaskId;
            QRCodeContent content = JsonConvert.DeserializeObject<QRCodeContent>(task.CodeData);
            qrTask.QRCodeContents .Add(content);
            qrTask.Template=task.Template;
            HouseholdGenerator generator = new HouseholdGenerator(configuration);
            Image background = await generator.DownloadTemplateImage(qrTask);
  
            Image image = generator.GetHouseholdCodeImage(qrTask.QRCodeContents[0],task.Template, background,task.TaskType);
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
        }

        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<string> GetQrCode([FromBody] QRCodeContent content)
        {
            HouseholdGenerator generator = new HouseholdGenerator(configuration);
            Image image = generator.GetQRCodeImage(content);
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
        }
        /// <summary>
        /// 下载生成的户码，压缩文件
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpGet]
        public FileResult DownloadHouseholdCodeZipFile(int taskId)
        {
            HouseholdGenerator generator = new HouseholdGenerator(configuration);
            Stream stream = generator.GetHouseholdZipFile(taskId);
            return File(stream, "application/vnd.openxmlformats", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}-{taskId}.zip");
        }
    }
}
