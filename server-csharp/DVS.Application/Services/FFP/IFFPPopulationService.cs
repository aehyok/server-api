using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
    public interface IFFPPopulationService : IServiceBase<FFPPopulation>
    {

        Task<List<FFPPopulationDto>> GetFFPPopulationMemebers(int householdId, int isConvertDictionary = 0);
        Task<MessageResult<int>> SaveFFPPopulation(List<FFPPopulationPostDto> populationDtos, int createdBy);


    }
}
