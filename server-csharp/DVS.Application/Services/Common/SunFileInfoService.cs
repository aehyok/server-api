using AutoMapper;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
// using AutoMapper.Configuration;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace DVS.Application.Services.Common
{
    public class SunFileInfoService : ServiceBase<SunFileInfo>, ISunFileInfoService
    {
        private readonly IConfiguration configuration;
        public SunFileInfoService(DbContext dbContext, IMapper mapper, IConfiguration configuration)
            : base(dbContext, mapper)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// 获取前面那一段路径
        /// </summary>
        /// <returns></returns>
        public string GetFileAccessUrl()
        {
            string store = this.configuration["File:Store"];
            return $"{this.configuration[$"File:{store}:AccessUrl"]}/";
        }

        public async Task<List<SunFileInfoDto>> GetSunFileInfoList(string ids)
        {
            List<SunFileInfoDto> fileList = new List<SunFileInfoDto>();
            if (!string.IsNullOrEmpty(ids))
            {
                List<string> imageIdList = new List<string>(ids.Split(','));
                List<int> idsList = new List<int>();
                Regex rex = new Regex(@"^\d+$");
                foreach (var id in imageIdList)
                {
                    if (rex.IsMatch(id))
                    {
                        idsList.Add(int.Parse(id));
                    }
                }
                var list = await this.GetListAsync(a => a.IsDeleted == 0 && idsList.Contains(a.Id));
                fileList = mapper.Map<List<SunFileInfoDto>>(list);
            }

            return fileList;
        }

        public async Task<SunFileInfoDto> GetSunFileInfo(int id)
        {
            SunFileInfoDto file = null;
            if (id > 0)
            {
                SunFileInfo sunFileInfo = await this.GetQueryable().FirstOrDefaultAsync(f => f.Id == id);
                file = mapper.Map<SunFileInfoDto>(sunFileInfo);
            }

            return file;
        }

        /// <summary>
        /// 获取相对路径字符串
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<string> GetSunFileRelativeUrls(string ids)
        {

            var list = await this.GetSunFileInfoList(ids);
            string str = "";
            foreach (var item in list)
            {
                str += item.RelativePath + ",";
            }
            return str.TrimEnd(',');
        }

        /// <summary>
        /// 相对路径转化为绝对路径
        /// </summary>
        /// <param name="imageUrls"></param>
        /// <returns></returns>
        public string ToAbsolutePath(string imageUrls)
        {
            string str = "";
            if (!string.IsNullOrWhiteSpace(imageUrls))
            {
                string fileAccessUrl = this.GetFileAccessUrl();
                string[] list = imageUrls.Split(',');
                foreach (var url in list)
                {
                    if (!string.IsNullOrWhiteSpace(url))
                    {

                        if (url.IndexOf("http") == 0)
                        {
                            str += url + ",";
                        }
                        else
                        {
                            str += fileAccessUrl + url + ",";
                        }
                    }
                }
            }
            return str.TrimEnd(',');
        }

    }
}
