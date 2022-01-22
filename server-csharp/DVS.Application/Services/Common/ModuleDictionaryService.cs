using AutoMapper;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Common
{
    public class ModuleDictionaryService : ServiceBase<ModuleDictionary>, IModuleDictionaryService
    {

        private ISunFileInfoService sunFileInfoService;
        public ModuleDictionaryService(DbContext dbContext,
            IMapper mapper,
             ISunFileInfoService sunFileInfoService) : base(dbContext, mapper)
        {
            this.sunFileInfoService = sunFileInfoService;
        }

        public async Task<IPagedList<ModuleDictionary>> GetPageModuleDictionaryAsync(string typeCode, string keyword = "", int page = 1, int limit = 10)
        {
            string sql = $"select * from ModuleDictionary where typeCode=@typeCode";
            string countSql = $"select * from ModuleDictionary where typeCode=@typeCode";

            var parameters = new List<DbParameter>() {
                new MySqlParameter(nameof(typeCode),typeCode)
            };

            if (!keyword.IsNullOrWhiteSpace())
            {
                string keywordParameter = $"{keyword}%";
                sql += $"  and name like   @keywordParameter ";
                countSql += $"  and name like @keywordParameter ";
                parameters.Add(new MySqlParameter(nameof(keywordParameter), keywordParameter));
            }

            IPagedList<ModuleDictionary> dictionarys = this.Context.Database.SqlQueryPagedList<ModuleDictionary>(page, limit, sql, countSql, "  order by sequence asc  ", parameters.ToArray());
            List<int> fileIds = dictionarys.Select(dict => dict.IconFileId).ToList();
            if (fileIds != null && fileIds.Count > 0)
            {
                string ids = string.Join(',', fileIds);
                if (!ids.IsNullOrWhiteSpace())
                {
                    List<SunFileInfoDto> sunFileInfoDtos = await sunFileInfoService.GetSunFileInfoList(ids);
                    foreach (var item in dictionarys)
                    {
                        if (item.IconFileId > 0)
                        {
                            item.IconFileUrl = sunFileInfoDtos.Find(file => file.Id == item.IconFileId).Url;
                        }
                    }
                }
            }
            return dictionarys;
        }

        public async Task<ModuleDictionary> GetModuleDictionaryDetailAsync(string code)
        {
            if (code.IsNullOrWhiteSpace())
            {
                throw new ValidException("无效的编码");
            }
            var query = this.GetQueryable().Where(type => type.Code == code);
            
            ModuleDictionary dictionary = await query.FirstOrDefaultAsync();
            SunFileInfoDto sunFileInfo= await sunFileInfoService.GetSunFileInfo(dictionary.IconFileId);
            if (sunFileInfo != null) {
                dictionary.IconFileUrl = sunFileInfo.Url;
            }
            return dictionary;
        }

        public async Task<List<ModuleDictionary>> GetModuleDictionaryAsync(List<string> typeCodes, string keyword = "")
        {
            if (typeCodes==null||typeCodes.Count==0)
            {
                throw new ValidException("无效的类型编码");
            }
            var query = this.GetQueryable().Where(type => typeCodes.Contains(type.TypeCode ) );
            if (!keyword.IsNullOrWhiteSpace())
            {
                query = query.Where(type => type.Name.Contains(keyword));
            }
            query = query.OrderBy(type => type.Sequence);
            return await query.ToListAsync();
        }


        public async Task<List<ModuleDictionary>> GetModuleDictionaryListAsync(List<string> moduleCodes)
        {
            var data = from d in this.GetQueryable()
                       where moduleCodes.Contains(d.Code)
                       select d;
            return await data.ToListAsync();
        }

        public async Task<string> GetModuleDictionaryNameByCode(string moduleCode, List<ModuleDictionary> moduleDictionaries = null)
        {

            if (moduleDictionaries != null && moduleDictionaries.Count() > 0)
            {
                var data = moduleDictionaries.FirstOrDefault(a => a.Code == moduleCode);
                if (data != null)
                {
                    return data.Name;
                }
            }
            else
            {
                var data = await this.GetAsync(a => a.Code == moduleCode);
                if (data != null)
                {
                    return data.Name;
                }
            }
            return "";
        }

        public async Task<ModuleDictionaryDto> GetModuleDictionaryByCode(string code, List<ModuleDictionary> moduleDictionaries = null)
        {

            if (moduleDictionaries != null && moduleDictionaries.Count() > 0)
            {
                var data = moduleDictionaries.FirstOrDefault(a => a.Code == code);
                if (data != null)
                {
                    return mapper.Map<ModuleDictionaryDto>(data);
                }
            }
            else
            {
                var data = await this.GetAsync(a => a.Code == code);
                if (data != null)
                {
                    return mapper.Map<ModuleDictionaryDto>(data);
                }
            }

            return null;

        }

        public async Task<bool> ChangeStatus(string code, int status)
        {
            if (code.IsNullOrWhiteSpace())
            {
                throw new ValidException("无效的字典编码");
            }
            int count = await this.GetQueryable().Where(dictionary => dictionary.Code == code).UpdateFromQueryAsync(dict => new ModuleDictionary()
            {
                Status = status
            });
            return count > 0;
        }

        public async Task<bool> Save(ModuleDictionaryEditReq dictionary)
        {
            if (dictionary == null) {
                throw new ValidException("无效的字典数据");
            }
            if (dictionary.Code.IsNullOrWhiteSpace()) {
                throw new ValidException("无效的字典编码");
            }
            int count = await this.GetQueryable().Where(dict => dict.Code == dictionary.Code).UpdateFromQueryAsync(dict => new ModuleDictionary()
            {
                Name = dictionary.Name,
                IconFileId = dictionary.IconFileId,
                FontColor = dictionary.FontColor ?? "#000000",
                Remark=dictionary.Remark,
                Sequence=dictionary.Sequence
            });
            return count > 0;
        }

        public async Task<List<ModuleDictionary>> GetModuleDictionaryAsync(string typeCode)
        {
            if (typeCode.IsNullOrWhiteSpace()) {
                throw new ValidException("无效的类型编码");
            }
            return await this.GetModuleDictionaryAsync(new List<string>() { typeCode });
        }

        public async Task<byte[]> GetExcelData(string typeCode, List<int> dictionaryIds, string keyword)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            package.Workbook.Properties.Title = "地块信息";
            var workSheet = package.Workbook.Worksheets.Add("地块信息");
            // 表头
            workSheet.Cells[1, 1].Value = "名称";
            workSheet.Cells[1, 2].Value = "编码";
            workSheet.Cells[1, 3].Value = "排序";
            workSheet.Cells[1, 4].Value = "备注";
            workSheet.Cells[1, 5].Value = "状态";
            // 获取字典信息
            var query = this.GetQueryable().Where(dict => dict.TypeCode == typeCode);
            if (dictionaryIds != null && dictionaryIds.Count > 0)
            {
                query = query.Where(dict => dictionaryIds.Contains(dict.Id));
            }
            if (!keyword.IsNullOrWhiteSpace())
            {
                query = query.Where(dict => dict.Name.Contains(keyword));
            }
            query = query.OrderBy(dict => dict.Sequence);
            List<ModuleDictionary> dictionaries = await query.ToListAsync();
            for (int i = 0; i < dictionaries.Count; i++)
            {
                int rowIndex = i + 2;
                var dictionary = dictionaries[i];
                workSheet.Cells[rowIndex, 1].Value = dictionary.Name;
                workSheet.Cells[rowIndex, 2].Value = dictionary.Code;
                workSheet.Cells[rowIndex, 3].Value = dictionary.Sequence;
                workSheet.Cells[rowIndex, 4].Value = dictionary.Remark;
                workSheet.Cells[rowIndex, 5].Value = dictionary.Status==0?"禁用":"启用";
                if (dictionary.Status == 0) {
                    workSheet.Cells[rowIndex, 5].Style.Font.Color.SetColor(Color.Red);
                }
            }
            return await package.GetAsByteArrayAsync();
        }
    }
}
