using AutoMapper;
using AutoMapper.QueryableExtensions;
using DVS.Application.Services.Common;
using DVS.Application.Services.Village;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.GIS;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.GIS;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Enum;
using LinqKit;
using Lychee.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.GIS
{
    public class GISPlotItemService : ServiceBase<GISPlotItem>, IGISPlotItemService
    {
        private readonly IBasicUserService basicUserService;
        private readonly IPopulationService populationService;
        private readonly IServiceBase<VillageHouseCodeMember> memberService;
        public GISPlotItemService(DbContext dbContext, IMapper mapper,
            IBasicUserService basicUserService,
            IServiceBase<VillageHouseCodeMember> memberService,
            IPopulationService populationService)
            : base(dbContext, mapper)
        {
            this.memberService = memberService;
            this.basicUserService = basicUserService;
            this.populationService = populationService;
        }

        public async Task<int> DeletePlotItemAsync(int id, int userid)
        {
            var plot = await this.GetAsync(a => a.Id == id);
            if (plot == null)
            {
                throw new ValidException("数据不存在");
            }

            //if (plot.CreatedBy != userid)
            //{
            //    throw new ValidException("非本人标绘不能删除");
            //}
            plot.IsDeleted = 1;
            plot.UpdatedBy = userid;
            plot.IsSync = 0; // 删除打点记录，重置上传标记，重新上传
            if (plot.PlotType == 4) {
                // 户码打点数据删除，将户码下的户籍人口的同步返回值置空
                var members = await this.memberService.GetListAsync(a => a.HouseholdId == plot.ObjectId && a.IsDeleted == 0);
                List<int> ids = new List<int>();
                foreach (var item in members)
                {
                    ids.Add(item.PopulationId);
                }
                await this.populationService.GetQueryable().Where(a => ids.Contains(a.Id)).UpdateFromQueryAsync(a => new VillagePopulation()
                {
                    SyncId = ""
                });
            }
            return await this.UpdateAsync(plot);
        }

        public async Task<GISPlotItemDto> DetailPlotItemAsync(int id)
        {
            var plot = await this.GetAsync(a => a.Id == id);
            if (plot == null)
            {
                throw new ValidException("数据不存在");
            }

            var result = mapper.Map<GISPlotItemDto>(plot);
            if (result != null)
            {
                List<int> userIds = new List<int>
                {
                    result.CreatedBy,
                    result.UpdatedBy
                };
                var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));

                var user = userList.FirstOrDefault(a => a.Id == result.CreatedBy);
                result.CreatedByName = user != null ? user.NickName : "";

                user = userList.FirstOrDefault(a => a.Id == result.UpdatedBy);
                result.UpdatedByName = user != null ? user.NickName : "";

            }
            return result;
        }

        public async Task<IPagedList<GISPlotItemDto>> ListPlotItemAsync(GISListQueryModel model)
        {
            Expression<Func<GISPlotItem, bool>> expression = a => a.IsDeleted == 0;
            if (model.PlotType > 0)
            {
                expression = expression.And(a => a.PlotType == model.PlotType);
            }

            if (model.ObjectId > 0)
            {
                expression = expression.And(a => a.ObjectId == model.ObjectId);
            }

            if (!string.IsNullOrWhiteSpace(model.ObjectIds))
            {
                List<string> objectlist = model.ObjectIds.Split(",").ToList();
                expression = expression.And(a => objectlist.Contains(a.ObjectId.ToString()));
            }

            if (model.RelationId > 0) {
                Expression<Func<GISPlotItem, bool>> expression1 = a => a.RelationId == model.RelationId;
                Expression<Func<GISPlotItem, bool>> expression2 = a => a.Id == model.RelationId;
                expression = expression.And(expression1.Or<GISPlotItem>(expression2));
            }

            var pageInfo = await this.GetQueryable().Where(expression).OrderByDescending(a => a.CreatedAt).ProjectTo<GISPlotItemDto>(mapper.ConfigurationProvider).ToPagedListAsync(model.Page, model.Limit);

            List<int> userIds = pageInfo.Select(a => a.CreatedBy).ToList().Union(pageInfo.Select(a => a.UpdatedBy).ToList()).ToList();
            List<int> ids = pageInfo.Select(a => a.Id).Distinct().ToList();
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));

            foreach (var item in pageInfo)
            {
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
                item.CreatedByName = user != null ? user.NickName : "";

                user = userList.FirstOrDefault(a => a.Id == item.UpdatedBy);
                item.UpdatedByName = user != null ? user.NickName : "";
                if (model.PlotType == (int)PlotType.HOUSEHOLD)
                {
                    // 统计此户码打点的地方有几户
                    var relations = await this.GetListAsync(a => a.RelationId == item.RelationId && a.IsDeleted == 0);
                    item.Cnt = relations != null ? relations.Count() : 0;
                }
            }
            return pageInfo;
        }

        public async Task<int> SavePlotItemAsync(GISPlotItem model)
        {
            if (model == null)
            {
                throw new ValidException("参数无效");
            }

            if (string.IsNullOrWhiteSpace(model.Name) && model.ObjectIds == null)
            {
                throw new ValidException("名称不能为空");
            }


            if (model.ObjectId == 0 && model.ObjectIds == null)
            {
                throw new ValidException("参数无效");
            }

            if (model.PlotType == 0)
            {
                throw new ValidException("参数无效");
            }

            if (model.Id == 0)
            {
                if (model.ObjectIds != null && model.ObjectIds.Count() > 0)
                {
                    GISPlotItem res = new GISPlotItem();
                    foreach (var item in model.ObjectIds)
                    {
                        model.ObjectId = item.ObjectId;
                        model.Name = item.Name;
                        model.Id = 0;
                        res = await this.InsertAsync(model);
                    }
                    return res.Id;
                }
                else
                {
                    var res = await this.InsertAsync(model);
                    await this.GetQueryable().Where(a => a.Id == res.Id).UpdateFromQueryAsync(a => new GISPlotItem()
                    {
                        RelationId = res.Id
                    });
                    return res.Id;
                }
            }
            else
            {
                var data = await this.GetAsync(a => a.Id == model.Id);
                if (data == null)
                {
                    throw new ValidException("id不存在");
                }
                data.IsSync = 0; // 修改打点记录，重置上传标记，重新上传
                data.Point = model.Point;
                data.PointItems = model.PointItems;
                data.Name = model.Name;
                data.Remark = model.Remark;
                int res = await this.UpdateAsync(data);
                if (res > 0)
                {
                    return model.Id;
                }
                return res;
            }
        }
    }
}
