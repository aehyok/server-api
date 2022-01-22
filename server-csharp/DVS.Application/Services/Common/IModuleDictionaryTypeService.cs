using DVS.Core.Domains.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
    public interface IModuleDictionaryTypeService
    {
        /// <summary>
        /// 获取模块字典详情
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public Task<ModuleDictionaryType> GetModuleDictionaryTypeDetailAsync(string typeCode);
        /// <summary>
        /// 获取模块字典列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public Task<List<ModuleDictionaryType>> GetMdouleDictionaryTypeAsync(string keyword, string moduleCode = "");
    }
}
