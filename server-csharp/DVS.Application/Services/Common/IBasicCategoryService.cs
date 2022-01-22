using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
    public interface IBasicCategoryService : IServiceBase<BasicCategory>
    {
        Task<BasicCategoryDto> GetBasicCategory(int id);

        Task<List<BasicCategoryDto>> GetBasicCategoryList(List<int> ids);
    }
}
