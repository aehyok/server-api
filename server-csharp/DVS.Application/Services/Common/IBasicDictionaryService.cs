using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Common
{
    public interface IBasicDictionaryService : IServiceBase<BasicDictionary>
    {
        Task<IEnumerable<BasicDictionary>> GetBasicDictionaryList(int typeCode);
        Task<IEnumerable<BasicDictionary>> GetBasicDictionaryList(List<string> ids);
        Task<string> GetNameById(string id, IEnumerable<BasicDictionary> basicDictionaries = null);

        Task<IEnumerable<BasicDictionary>> GetBasicDictionaryList(List<int> codes);

        Task<IEnumerable<BasicDictionary>> GetBasicDictionaryCodeList(List<string> codes);

        Task<string> GetNameByCode(string code, IEnumerable<BasicDictionary> basicDictionaries = null);

        Task<BasicDictionaryDto> GetOneByCode(string code, IEnumerable<BasicDictionary> basicDictionaries = null);

        Task<IPagedList<BasicDictionaryHouseTagDto>> GetDictionaryHouseTagPageListAsync(HouseTagQueryModel body);

        Task<bool> SaveDictionaryHouseTagAsync(BasicDictionaryHouseTagBody body, int updatedBy = 0);
    }
}
