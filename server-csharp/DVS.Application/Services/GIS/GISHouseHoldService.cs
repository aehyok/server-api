using AutoMapper;
using DVS.Application.Services.Village;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Model.Dtos.Village;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Dtos.Village;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.GIS
{
    public class GISHouseHoldService : ServiceBase<VillageHouseholdCode>, IGISHouseHoldService
    {
        readonly IHouseholdCodeService householdservice;
        readonly IGISPlotItemService plotitemService;
        public GISHouseHoldService(DbContext dbContext, IMapper mapper, IHouseholdCodeService service, IGISPlotItemService plotitemService) : base(dbContext, mapper)
        {
            this.householdservice = service;
            this.plotitemService = plotitemService;
        }

        public async Task<HouseholderAndHouseNumberDto> DetailHouseholdCodeAsync(int id)
        {
            return await this.householdservice.GethouseholderAndHouseNumber(id);
        }

        public async Task<IPagedList<HouseholdCodeDto>> ListHouseholdCodeAsync(GISListQueryModel model)
        {
            PostBody body = new PostBody
            {
                Page = model.Page,
                Limit = model.Limit,
                AreaId = model.AreaId,
                Keyword = model.Keyword,
                Orderby = model.Orderby,
                HouseName = model.HouseName,
                Desc = model.Desc
            };
            if (model.RelationId > 0) {
                var relationIds = await this.plotitemService.GetListAsync(a => a.RelationId == model.RelationId && a.PlotType == (int)PlotType.HOUSEHOLD && a.IsDeleted == 0);
                if (relationIds != null)
                {
                    body.Ids = relationIds.Select(a => a.ObjectId).Distinct().ToList();
                }
            }
            var pagelist = await this.householdservice.GetHouseholdCodeList(body);
            List<int> ids = pagelist.Select(a => a.Id).Distinct().ToList();
            var list = await this.plotitemService.GetListAsync(a => a.IsDeleted == 0 && ids.Contains(a.ObjectId) && a.PlotType == (int)PlotType.HOUSEHOLD);
            foreach (var item in pagelist)
            {
                Core.Domains.GIS.GISPlotItem data = list.FirstOrDefault(a => a.ObjectId == item.Id && a.PlotType == (int)PlotType.HOUSEHOLD);
                item.IsPloted = data != null;
                item.PlotId = data == null ? 0 : data.Id;
            }
            return pagelist;
        }
    }
}
