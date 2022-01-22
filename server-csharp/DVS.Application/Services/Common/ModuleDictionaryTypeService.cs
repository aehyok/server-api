using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
   public  class ModuleDictionaryTypeService:ServiceBase<ModuleDictionaryType>,IModuleDictionaryTypeService
    {
        public ModuleDictionaryTypeService(DbContext dbContext,IMapper mapper) : base(dbContext,mapper) { 
        
        }

        public async Task<List<ModuleDictionaryType>> GetMdouleDictionaryTypeAsync(string keyword,string moduleCode="")
        {
            var query= this.GetQueryable();
            if (!keyword.IsNullOrWhiteSpace()) {
               query =  query.Where(type => type.Name.Contains(keyword));
            }
            if (!moduleCode.IsNullOrWhiteSpace()) {
                query = query.Where(type => type.Module == moduleCode);
            }
            query = query.OrderBy(type => type.Sequence);
            List<ModuleDictionaryType> dictionaryTypes= await query.ToListAsync();
            return dictionaryTypes;
        }

        public async Task<ModuleDictionaryType> GetModuleDictionaryTypeDetailAsync(string typeCode)
        {
            if (typeCode.IsNullOrWhiteSpace()) {
                throw new ValidException("无效的编码");
            }
            ModuleDictionaryType dictionaryType= await this.GetQueryable().Where(type => type.Code == typeCode).FirstOrDefaultAsync();
            return dictionaryType;
        }
    }
}
