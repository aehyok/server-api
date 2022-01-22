using DVS.Core.Domains.Common;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Common
{
   public  interface IModuleDictionaryService
    {
        public Task<ModuleDictionary> GetModuleDictionaryDetailAsync(string code);
        public Task<IPagedList<ModuleDictionary>> GetPageModuleDictionaryAsync(string typeCode, string keyword = "", int page = 1, int limit = 10);
        public Task<List<ModuleDictionary>> GetModuleDictionaryAsync(List<string> typeCodes, string keyword = "");
        public Task<List<ModuleDictionary>> GetModuleDictionaryAsync(string typeCode);


        Task<List<ModuleDictionary>> GetModuleDictionaryListAsync(List<string> moduleCodes);
        Task<string> GetModuleDictionaryNameByCode(string moduleCode, List<ModuleDictionary> moduleDictionaries = null);
        Task<ModuleDictionaryDto> GetModuleDictionaryByCode(string code, List<ModuleDictionary> moduleDictionaries = null);
        Task<bool> ChangeStatus(string code,int status);
        Task<bool> Save(ModuleDictionaryEditReq dictionary);
        Task<byte[]> GetExcelData(string typeCode, List<int> dictionaryIds, string keyword);
    }
}
