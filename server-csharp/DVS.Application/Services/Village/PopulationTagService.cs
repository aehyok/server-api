using AutoMapper;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using X.PagedList;
using Lychee.Extensions;
using LinqKit;
using DVS.Common.Services;
using DVS.Common.Infrastructures;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village;
using DVS.Application.Services.Common;

namespace DVS.Application.Services.Village
{
    public class PopulationTagService : ServiceBase<VillagePopulationTag>, IPopulationTagService
    {

        readonly IBasicDictionaryService basicDictionaryService;
        private readonly ISunFileInfoService sunFileInfoService;
        public PopulationTagService(DbContext dbContext, IMapper mapper,
            IBasicDictionaryService basicDictionaryService,
            ISunFileInfoService sunFileInfoService
            )
            : base(dbContext, mapper)
        {
            this.basicDictionaryService = basicDictionaryService;
            this.sunFileInfoService = sunFileInfoService;
        }

        public async Task<bool> SaveTags(int populationId, string tags)
        {
            if (!string.IsNullOrWhiteSpace(tags))
            {
                string[] ids = tags.Split(',');
                List<VillagePopulationTag> list = new List<VillagePopulationTag>();
                foreach (var id in ids)
                {
                    int _id = 0;
                    int.TryParse(id, out _id);
                    if (_id > 0)
                    {
                        list.Add(new VillagePopulationTag()
                        {
                            PopulationId = populationId,
                            TagId = _id
                        });
                    }
                }

                if (list.Count > 0)
                {
                    await this.ExecuteSqlAsync("delete  from  VillagePopulationTag where populationId=" + populationId);
                    var res = await this.InsertRangeAsync(list);
                    return res > 0;
                }
            }
            return false;
        }

        public async Task<IEnumerable<VillageTagDto>> GetTags(int populationId)
        {
            var list = from t in this.GetQueryable().Where(a => a.PopulationId == populationId && a.IsDeleted == 0)
                       join d in this.basicDictionaryService.GetQueryable()
                       on t.TagId equals d.Code
                       select new VillageTagDto()
                       {
                           Id = t.TagId,
                           Name = d.Name,
                           IconFileUrl = d.IconFileUrl,
                           Pid = t.PopulationId,
                       };

            // var fileAccessUrl = this.sunFileInfoService.GetFileAccessUrl();
            foreach (var item in list)
            {
                if (!string.IsNullOrWhiteSpace(item.IconFileUrl))
                {
                    item.IconFileUrl = this.sunFileInfoService.ToAbsolutePath(item.IconFileUrl);
                }
            }
            return (await list.ToListAsync());
        }

        public async Task<IEnumerable<VillageTagDto>> GetTags(List<int> populationIds)
        {
            List<VillageTagDto> datas = new List<VillageTagDto>();
            if (populationIds != null && populationIds.Count > 0)
            {

                var list = from t in this.GetQueryable()
                           join d in this.basicDictionaryService.GetQueryable()
                           on t.TagId equals d.Code
                           where populationIds.Contains(t.PopulationId) && t.IsDeleted == 0
                           select new VillageTagDto()
                           {
                               Id = t.TagId,
                               Name = d.Name,
                               IconFileUrl = d.IconFileUrl,
                               Pid = t.PopulationId,
                               FontColor = d.FontColor,
                           };
                datas = await list.ToListAsync();
                // var fileAccessUrl = this.sunFileInfoService.GetFileAccessUrl();
                foreach (var item in datas)
                {
                    if (!string.IsNullOrWhiteSpace(item.IconFileUrl))
                    {
                        item.IconFileUrl = this.sunFileInfoService.ToAbsolutePath(item.IconFileUrl);
                    }
                }
            }
            return datas;
        }

    }
}
