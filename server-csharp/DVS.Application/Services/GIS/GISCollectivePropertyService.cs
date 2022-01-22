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
using System.Collections.Generic;
using DVS.Application.Services.Common;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Enum;
using System.IO;
using System.Data;
using DVS.Common;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using DVS.Models.Dtos.Common;

namespace DVS.Application.Services.GIS
{
	public class GISCollectivePropertyService : ServiceBase<GISCollectiveProperty>, IGISCollectivePropertyService
	{
        readonly IBasicUserService basicUserService;
        readonly IBasicAreaService basicAreaService;
		readonly IBasicDictionaryService basicDictionaryService;
		readonly ISunFileInfoService sunFileInfoService;
		readonly IGISPlotItemService plotitemService;
		readonly IParkAreaService parkAreaService;
		public GISCollectivePropertyService(DbContext dbContext, IMapper mapper, IBasicUserService basicUserService, IBasicAreaService basicAreaService, IBasicDictionaryService basicDictionaryService, ISunFileInfoService sunFileInfoService, IGISPlotItemService plotitemService, IParkAreaService parkAreaService)
			: base(dbContext, mapper)
		{
            this.basicAreaService = basicAreaService;
            this.basicUserService = basicUserService;
			this.basicDictionaryService = basicDictionaryService;
			this.sunFileInfoService = sunFileInfoService;
			this.plotitemService = plotitemService;
			this.parkAreaService = parkAreaService;
		}

		public async Task<int> DeleteCollectivePropertyAsync(int id, int userid)
		{
			var model = await this.GetAsync(a => a.Id == id);
			if (model == null) {
				throw new ValidException("数据不存在");
			}

			model.IsDeleted = 1;
			model.UpdatedBy = userid;

			var ret =  await this.UpdateAsync(model);
			if (ret > 0)
			{
				await this.ExecuteSqlAsync($"update GISPlotItem set isDeleted = 1,updatedBy={userid},isSync=0 where objectId={id} and plottype = " + (int)PlotType.COLLECTIVEPROPERTY);
			}
			return ret;
		}

		public async Task<GISCollectivePropertyDto> DetailCollectivePropertyAsync(int id)
		{
			var model = await this.GetAsync(a => a.Id == id);
			if (model == null)
			{
				throw new ValidException("数据不存在");
			}

			var result = mapper.Map<GISCollectivePropertyDto>(model);
			if (result != null) {
				List<int> userIds = new List<int>();
				userIds.Add(result.CreatedBy);
				userIds.Add(result.UpdatedBy);
				var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
				var area = await this.basicAreaService.GetAsync(a => a.Id == result.AreaId);
				result.AreaName = area != null ? area.Name : "";

				var user = userList.FirstOrDefault(a => a.Id == result.CreatedBy);
				result.CreatedByName = user != null ? user.NickName : "";

				user = userList.FirstOrDefault(a => a.Id == result.UpdatedBy);
				result.UpdatedByName = user != null ? user.NickName : "";

				var dictionarys = await this.basicDictionaryService.GetBasicDictionaryList(7013);
				var dictionary = dictionarys.FirstOrDefault(a => a.Code == result.TypeId);
				result.TypeName = dictionary != null ? dictionary.Name : "";
				result.MediaFiles = await this.sunFileInfoService.GetSunFileInfoList(result.MediaId.ToString());
				result.IconFiles = await this.sunFileInfoService.GetSunFileInfoList(result.IconId.ToString());
			}
			return result;
		}

		public async Task<IPagedList<GISCollectivePropertyDto>> ListCollectivePropertyAsync(GISListQueryModel model)
		{
			Expression<Func<GISCollectiveProperty, bool>> expression = a => a.IsDeleted == 0;
			if (model.Category > 0)
			{
				expression = expression.And(a => a.Category == model.Category);
			}
			if (model.AreaId > 0) {
				expression = expression.And(a => a.AreaId == model.AreaId);
			}
			if (model.ObjectId > 0)
			{
				expression = expression.And(a => a.ObjectId == model.ObjectId);
			}
			if (!string.IsNullOrWhiteSpace(model.Keyword))
			{
				expression = expression.And(a => a.Name.ToLower().Contains(model.Keyword.ToLower()));
			}
			if (!string.IsNullOrWhiteSpace(model.Ids))
			{
				var _ids = model.Ids.Split(",");
				expression = expression.And(a => _ids.Contains(a.Id.ToString()));
			}
			if (model.TypeId > 0)
			{
				expression = expression.And(a => a.TypeId == model.TypeId);
			}
			var result = await this.GetQueryable().Where(expression).OrderByDescending(a => a.CreatedAt).ProjectTo<GISCollectivePropertyDto>(mapper.ConfigurationProvider).ToPagedListAsync(model.Page, model.Limit);

			List<int> userIds = result.Select(a => a.CreatedBy).ToList().Union(result.Select(a => a.UpdatedBy).ToList()).ToList();
			List<int> areaIds = result.Select(a => a.AreaId).Distinct().ToList();
			List<int> fileIds = result.Select(a => a.MediaId).ToList().Union(result.Select(a => a.IconId).ToList()).ToList();
			List<int> parkIds = result.Where(a => a.Category == 2 && a.ObjectId > 0).Select(a => a.ObjectId).Distinct().ToList();

			var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
			var areaList = await this.basicAreaService.GetListAsync(a => areaIds.Contains(a.Id));
			var dictionaryList = await this.basicDictionaryService.GetBasicDictionaryList(7013);
			var sunFileInfos = await this.sunFileInfoService.GetSunFileInfoList(string.Join(",", fileIds));
			var parkList = (model.Category == 2) ? await this.parkAreaService.GetListAsync(a => parkIds != null ? parkIds.Contains(a.Id) : false) : null;

			List<int> ids = result.Select(a => a.Id).Distinct().ToList();
			var list = await this.plotitemService.GetListAsync(a => a.IsDeleted == 0 && ids.Contains(a.ObjectId) && a.PlotType == (int)PlotType.COLLECTIVEPROPERTY);
			foreach (var item in result)
            {
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
				var area = areaList.FirstOrDefault(a => a.Id == item.AreaId);
				item.AreaName = area != null ? area.Name : "";
				item.CreatedByName = user != null ? user.NickName : "";
				item.UpdatedByName = user != null ? user.NickName : "";

				var dictionary = dictionaryList.FirstOrDefault(a => a.Code == item.TypeId);
				item.TypeName = dictionary != null ? dictionary.Name : "";
				item.TypeDto = dictionary == null ? null : this.mapper.Map<BasicDictionaryDto>(dictionary);
				var medias = sunFileInfos.FindAll(a => a.Id == item.MediaId);
				item.MediaFiles = medias;
				item.IsPloted = list.FirstOrDefault(a => a.ObjectId == item.Id && a.PlotType == (int)PlotType.COLLECTIVEPROPERTY) == null ? false : true;
				if (model.Category == 2)
				{
					var park = parkList.FirstOrDefault(a => a.Id == item.ObjectId);
					item.ParkName = park != null ? park.EnterpriseName : "";
				}
				var icons = sunFileInfos.FindAll(a => a.Id == item.IconId);
				item.IconFiles = icons;
			}
            StaticPagedList<GISCollectivePropertyDto> pageList = new StaticPagedList<GISCollectivePropertyDto>(result, model.Page, model.Limit, result.TotalItemCount);
            return pageList;
        }

		public async Task<int> SaveCollectivePropertyAsync(GISCollectiveProperty model)
		{
            if (model == null)
            {
                throw new ValidException("参数无效");
            }
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ValidException("设施名称不能为空");
            }

            Expression<Func<GISCollectiveProperty, bool>> pre = a => a.Name == model.Name && a.IsDeleted == 0;
			if (model.Category > 0)
			{
				pre = pre.And(a => a.Category == model.Category);
			}
			if (model.AreaId > 0)
			{
				pre = pre.And(a => a.AreaId == model.AreaId);
			}
			if (model.ObjectId > 0)
			{
				pre = pre.And(a => a.ObjectId == model.ObjectId);
			}
			if (model.Id > 0)
            {
                pre = pre.And(a => a.Id != model.Id);
            }

            int cnt = this.GetQueryable().Where(pre).Count();
            if (cnt > 0)
            {
                throw new ValidException("设施名称重复");
            }

            if (model.Id == 0)
            {
                var data = await this.InsertAsync(model);
                return data.Id;
            }
            else
            {
                var data = await this.GetAsync(a => a.Id == model.Id);
				if (data != null)
				{
					data.Name = model.Name;
					data.Remark = model.Remark;
					data.Address = model.Address;
					data.MediaId = model.MediaId;
					data.MediaType = model.MediaType;
					data.TypeId = model.TypeId;
				}
				else {
					throw new ValidException("数据无效");
				}
                int res = await this.UpdateAsync(data);
                if (res > 0)
                {
                    return data.Id;
                }
                return res;
            }
        }

		public async Task<int> BatchUpdateCollectivePropertyAsync(GISBatchDto model)
		{
			if (model == null)
			{
				throw new ValidException("参数无效");
			}

			var res = await this.GetQueryable().Where(a => model.Ids.Contains(a.Id)).UpdateFromQueryAsync(a => new GISCollectiveProperty()
			{
				TypeId = model.TypeId,
				UpdatedBy = model.UpdatedBy
			});
			if (res > 0)
			{
				return res;
			}
			return 0;
		}

		public async Task<GISImportRes> ImportExcelAsync(Stream fileStream, int category, int userId, int areaId, int objectId)
		{
			GISImportRes res = new GISImportRes();
			DataSet dataset = Utils.ImportExcel(fileStream, 5, 4);
			int cnt = dataset.Tables.Count;
			if (cnt == 0)
			{
				return res;
			}
			int index = 1;
			var dictionarys = await this.basicDictionaryService.GetBasicDictionaryList(7013);
			foreach (DataRow row in dataset.Tables[0].Rows)
			{
				string name = row.ItemArray[0].ToString();
				string typeName = row.ItemArray[1].ToString();
				string address = row.ItemArray[2].ToString();
				string remark = row.ItemArray[3].ToString();
				if (name == "" && address == "" && remark == "" && typeName == "")
				{
					// 整行都没有数据
					index++;
					continue;
				}
				if (name == "")
				{
					res.Fail.Add(new GISImportFailInfo() { Index = index, Message = "数据错误" });
					index++;
					continue;
				}
				var dictionary = dictionarys.FirstOrDefault(a => a.Name == typeName);
				var typeId = dictionary != null ? dictionary.Code : 0;
				if (typeId == 0)
				{
					res.Fail.Add(new GISImportFailInfo() { Index = index, Message = "数据错误" });
					index++;
					continue;
				}
				_ = await this.SaveCollectivePropertyAsync(new GISCollectiveProperty()
				{
					Name = name,
					TypeId = typeId,
					Address = address,
					Remark = remark,
					Category = category,
					ObjectId = objectId,
					AreaId = areaId,
					CreatedBy = userId,
					UpdatedBy = userId,
				});
			}
			return res;
		}

		public async Task<byte[]> GetExcelData(int areaId, int typeId = 0, string keyword = "", string ids = "", int category = 0, int objectId = 0)
		{
			if (areaId == 0)
			{
				throw new ValidException("参数无效");
			}
			GISListQueryModel body = new GISListQueryModel()
			{
				AreaId = areaId,
				TypeId = typeId,
				Keyword = keyword,
				Category = category,
				ObjectId = objectId,
				Ids = ids,
				Page = 1,
				Limit = 10000,
			};
			IPagedList<GISCollectivePropertyDto> list = await this.ListCollectivePropertyAsync(body);

			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var package = new ExcelPackage();

			package.Workbook.Properties.Title = "公共设施";
			var workSheet = package.Workbook.Worksheets.Add("公共设施");
			// 表头
			workSheet.Cells[1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			workSheet.Cells[1, 1].Value = "序号";
			workSheet.Cells[1, 2].Value = "设施名称";
			workSheet.Cells[1, 3].Value = "设施类型";
			workSheet.Cells[1, 4].Value = "设施位置";
			workSheet.Cells[1, 5].Value = "图片视频";
			workSheet.Cells[1, 6].Value = "备注";

			List<GISCollectivePropertyDto> items = list.ToList();
			for (int i = 0; i < items.Count; i++)
			{
				int rowIndex = i + 2;
				GISCollectivePropertyDto item = items[i];

				workSheet.Cells[rowIndex, 1].Value = i + 1;
				workSheet.Cells[rowIndex, 2].Value = item.Name;
				workSheet.Cells[rowIndex, 3].Value = item.TypeName;
				workSheet.Cells[rowIndex, 4].Value = item.Address;
                workSheet.Cells[rowIndex, 5].Value = item.MediaFiles.Count > 0 ? "有" : "--";
                workSheet.Cells[rowIndex, 6].Value = item.Remark;
			}
			return await package.GetAsByteArrayAsync();
		}
	}
}
