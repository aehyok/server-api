using DVS.Models.Dtos.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
    public interface IHWService
    {
        Task<HWOcrIDCardRes> ScanIdCard(IFormFile file);
    }
}
