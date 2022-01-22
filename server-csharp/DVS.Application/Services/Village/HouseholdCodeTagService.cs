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
using DVS.Common.RPC;
using DVS.Models.Dtos.RPC;

namespace DVS.Application.Services.Village
{
    public class HouseholdCodeTagService : ServiceBase<VillageHouseholdCodeTag>, IHouseholdCodeTagService
    {
        private readonly IBasicDictionaryService basicDictionaryService;
        private readonly ISunFileInfoService sunFileInfoService;
        public HouseholdCodeTagService(DbContext dbContext, IMapper mapper,
            IBasicDictionaryService basicDictionaryService,
            ISunFileInfoService sunFileInfoService
            )
            : base(dbContext, mapper)
        {
            this.basicDictionaryService = basicDictionaryService;
            this.sunFileInfoService = sunFileInfoService;
        }

        public async Task<bool> SaveTags(int householdId, string tags, int loginUserId)
        {
            List<VillageHouseholdCodeTag> exitsTags = this.GetQueryable().Where(tag => tag.HouseholdId == householdId && tag.IsDeleted == 0).ToList();
            if (exitsTags == null)
            {
                exitsTags = new List<VillageHouseholdCodeTag>();
            }
            List<int> oldIds = exitsTags.Select(tag => tag.TagId).ToList();


            await this.ExecuteSqlAsync("delete  from  VillageHouseholdCodeTag where householdId=" + householdId);

            if (!string.IsNullOrWhiteSpace(tags))
            {
                string[] ids = tags.Split(',');

                List<VillageHouseholdCodeTag> list = new List<VillageHouseholdCodeTag>();
                foreach (var id in ids)
                {
                    if (!oldIds.Contains(int.Parse(id)))
                    {//新的在旧的标签中找不到，那就新增积分
                        BasicRPC.AllotScore(new IntegralReq()
                        {
                            IntegralAction = IntegralAction.FamilyTag,
                            Description = "添加门牌标签",
                            HouseholdId = householdId,
                            UserId = loginUserId
                        });
                    }
                    int _id = 0;
                    int.TryParse(id, out _id);
                    if (_id > 0)
                    {
                        list.Add(new VillageHouseholdCodeTag()
                        {
                            HouseholdId = householdId,
                            TagId = _id
                        });
                    }
                }

                if (list.Count > 0)
                {

                    var res = await this.InsertRangeAsync(list);
                    return res > 0;
                }
            }
            return false;
        }

        public async Task<IEnumerable<VillageTagDto>> GetTags(int householdId)
        {


            var list = from t in this.GetQueryable()
                       join d in this.basicDictionaryService.GetQueryable()
                       on t.TagId equals d.Code
                       where t.HouseholdId == householdId && t.IsDeleted == 0
                       select new VillageTagDto()
                       {
                           Id = t.TagId,
                           Name = d.Name,
                           IconFileUrl = d.IconFileUrl,
                           Pid = t.HouseholdId,
                           FontColor = d.FontColor,
                       };
            var datas = await list.ToListAsync();

            foreach (var item in datas)
            {
                item.IconFileUrl = this.sunFileInfoService.ToAbsolutePath(item.IconFileUrl);
            }

            return datas;
        }

        public async Task<IEnumerable<VillageTagDto>> GetTags(List<int> householdIds)
        {
            List<VillageTagDto> datas = new List<VillageTagDto>();
            if (householdIds != null && householdIds.Count > 0)
            {

                var list = from t in this.GetQueryable()
                           join d in this.basicDictionaryService.GetQueryable()
                           on t.TagId equals d.Code
                           where householdIds.Contains(t.HouseholdId) && t.IsDeleted == 0
                           select new VillageTagDto()
                           {
                               Id = t.TagId,
                               Name = d.Name,
                               IconFileUrl = d.IconFileUrl,
                               Pid = t.HouseholdId,
                               FontColor = d.FontColor,
                           };
                datas = await list.ToListAsync();
                var fileAccessUrl = this.sunFileInfoService.GetFileAccessUrl();
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
