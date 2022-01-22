using AutoMapper;
using DVS.Common.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using X.PagedList;
using DVS.Core.Domains.GIS;
using DVS.Models.Dtos.GIS;
using DVS.Common.Models;
using System.Linq.Expressions;
using LinqKit;
using System.Linq;
using AutoMapper.QueryableExtensions;
using System.Text;
using System.Collections.Generic;
using DVS.Application.Services.Common;
using DVS.Models.Dtos.GIS.Query;

namespace DVS.Application.Services.GIS
{
	public class GISPlotItemService1 : ServiceBase<GISPlotItem>, IGISPlotItemService
	{
		private readonly IBasicUserService basicUserService;
		private readonly IBasicAreaService basicAreaService;
		private readonly IGISPlotService gisPlotService;

		public GISPlotItemService1(DbContext dbContext, IMapper mapper, IBasicUserService basicUserService, IBasicAreaService basicAreaService, IGISPlotService gisPlotService)
			: base(dbContext, mapper)
		{
			this.basicUserService = basicUserService;
			this.basicAreaService = basicAreaService;
			this.gisPlotService = gisPlotService;
		}


		public async Task<int> DeletePlotItemAsync(int id, int userid)
		{
			var plot = await this.GetAsync(a => a.Id == id);
			if (plot == null) {
				throw new ValidException("数据不存在");
			}

			if (plot.CreatedBy != userid)
			{
				throw new ValidException("不能删除");
			}
			plot.IsDeleted = 1;
			plot.UpdatedBy = userid;

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
				List<int> userIds = new List<int>();
				userIds.Add(result.CreatedBy);
				userIds.Add(result.UpdatedBy);
				var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));

				var user = userList.FirstOrDefault(a => a.Id == result.CreatedBy);
				result.CreatedByName = user != null ? user.NickName : "";

				user = userList.FirstOrDefault(a => a.Id == result.UpdatedBy);
				result.UpdatedByName = user != null ? user.NickName : "";

				result.AreaName = await this.basicAreaService.GetAreaName(result.AreaId);
			}
			return result;
		}

		public async Task<IPagedList<GISPlotItemDto>> ListPlotItemAsync(GISListQueryModel model)
		{

			StringBuilder countBuilder = new StringBuilder(@$"select GISPlotItem.id from GISPlot 
															left join GISPlotItem on GISPlotItem.plotid=GISPlot.id 
															 WHERE GISPlot.isDeleted = 0 ");

			StringBuilder queryBuilder = new StringBuilder(@$"select GISPlotItem.* from GISPlot left join GISPlotItem on GISPlotItem.plotid=GISPlot.id 
															WHERE GISPlot.isDeleted = 0 ");
			StringBuilder where = new StringBuilder();
			where.Append($" and GISPlotItem.isDeleted = 0 ");
			if (model.Category > 0)
			{
				where.Append($" and GISPlot.category = {model.Category}");
			}

			if (model.AreaId > 0)
			{
				where.Append($" and GISPlot.areaId = {model.AreaId}");
			}

			if (!string.IsNullOrWhiteSpace(model.Keyword))
			{
				where.Append($" and GISPlot.name like '%{model.Keyword}'");
			}

			//if (model.PlotType > 0)
			//{
			//	where.Append($" and GISPlot.plottype = {model.PlotType}");
			//}

			if (model.PlotId>0) {
				where.Append($" and GISPlotItem.plotid = {model.PlotId}");
			}

			if (!string.IsNullOrWhiteSpace(model.PlotIds)) {
				where.Append($" and GISPlotItem.plotid in ({model.PlotIds})");
			}

			if (where.Length > 0)
			{
				queryBuilder.Append(where);
				countBuilder.Append(where);
			}
			queryBuilder.Append(" ORDER BY GISPlotItem.createdAt desc");
			IPagedList<GISPlotItemDto> pageInfo = this.Context.Database.SqlQueryPagedList<GISPlotItemDto>(model.Page, model.Limit, queryBuilder.ToString(), countBuilder.ToString());

			List<int> userIds = pageInfo.Select(a => a.CreatedBy).ToList().Union(pageInfo.Select(a => a.UpdatedBy).ToList()).ToList();
			List<int> ids = pageInfo.Select(a => a.Id).ToList();
			var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));

			foreach (var item in pageInfo)
			{
				var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
				item.CreatedByName = user != null ? user.NickName : "";

				user = userList.FirstOrDefault(a => a.Id == item.UpdatedBy);
				item.UpdatedByName = user != null ? user.NickName : "";
			}
			return pageInfo;
		}

        public async Task<int> SavePlotItemAsync(GISPlotItem model)
		{
			if (model == null)
			{
				throw new ValidException("参数无效");
			}

			if (string.IsNullOrWhiteSpace(model.Name))
			{
				throw new ValidException("名称不能为空");
			}

            if (model.PlotId == 0)
            {
                throw new ValidException("参数无效");
            }

            if (model.Id == 0)
			{				
				var res = await this.InsertAsync(model);
				return res.Id;
			}
			else
			{
				int res = await this.UpdateAsync(model);
				if (res > 0)
				{
					return model.Id;
				}
				return res;
			}
		}
	}
}
