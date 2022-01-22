using DVS.Application.Services.SunFSAgent;
using DVS.Models.Dtos.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DVS.SunFSAgent.Api.Controllers
{
    /// <summary>
    /// 文件上传
    /// </summary>
    [Route("/api/sunfs")]
    public class FileUploadController : DvsControllerBase
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<List<SunFileInfoDto>> Upload([FromServices] IFileService service, List<IFormFile> files,[FromForm]bool isMaterial, [FromForm] string src="")
        {
            Console.WriteLine(src);
            List<SunFileInfoDto> list = await service.UploadFiles(files,isMaterial,src);

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="completedInfo"></param>
        /// <returns></returns>
        [HttpPost("transformCompleted")]
        public async Task<bool> TransformCompletedAsync([FromServices] IFileService service, TransformCompletedInfo completedInfo) {
            await service.TransformCompletedAsync(completedInfo);
            return true;
        }
    }
}