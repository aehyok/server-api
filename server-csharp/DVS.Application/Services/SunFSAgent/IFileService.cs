using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.SunFSAgent
{
    public interface IFileService
    {
        Task<List<SunFileInfoDto>> UploadFiles(List<IFormFile> files,bool isMaterial=false,string src="");
        Task TransformCompletedAsync(TransformCompletedInfo completedInfo);
    }
}
