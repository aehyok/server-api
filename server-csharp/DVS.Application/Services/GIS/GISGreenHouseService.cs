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
using DVS.Application.Services.Village;
using System.IO;
using System.Data;
using DVS.Common;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using DVS.Models.Dtos.Common;

namespace DVS.Application.Services.GIS
{
    public class GISGreenHouseService : ServiceBase<GISGreenHouse>, IGISGreenHouseService
    {
        readonly IBasicUserService basicUserService;
        readonly IBasicAreaService basicAreaService;
        readonly IBasicDictionaryService basicDictionaryService;
        readonly IGISPlotItemService plotitemService;
        readonly IHouseholdCodeService householdService;
        readonly ISunFileInfoService sunFileInfoService;
        public GISGreenHouseService(DbContext dbContext, IMapper mapper, IBasicUserService basicUserService, IBasicAreaService basicAreaService, IBasicDictionaryService basicDictionaryService, IGISPlotItemService plotitemService, IHouseholdCodeService householdService, ISunFileInfoService sunFileInfoService)
            : base(dbContext, mapper)
        {
            this.basicAreaService = basicAreaService;
            this.basicUserService = basicUserService;
            this.basicDictionaryService = basicDictionaryService;
            this.plotitemService = plotitemService;
            this.householdService = householdService;
            this.sunFileInfoService = sunFileInfoService;
        }

        public async Task<int> DeleteGreenHouseAsync(int id, int userid)
        {
            var greenhouse = await this.GetAsync(a => a.Id == id);
            if (greenhouse == null)
            {
                throw new ValidException("数据不存在");
            }

            greenhouse.IsDeleted = 1;
            greenhouse.UpdatedBy = userid;

            var ret = await this.UpdateAsync(greenhouse);
            if (ret > 0)
            {
                await this.ExecuteSqlAsync($"update GISPlotItem set isDeleted = 1,updatedBy={userid},isSync=0 where objectId={id} and plottype = " + (int)PlotType.GREENHOUSE);
            }
            return ret;
        }

        public async Task<GISGreenHouseDto> DetailGreenHouseAsync(int id)
        {
            var greenhouse = await this.GetAsync(a => a.Id == id);
            if (greenhouse == null)
            {
                throw new ValidException("数据不存在");
            }

            var result = mapper.Map<GISGreenHouseDto>(greenhouse);
            if (result != null)
            {
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

                var dictionarys = await this.basicDictionaryService.GetBasicDictionaryList(7011);
                var dictionary = dictionarys.FirstOrDefault(a => a.Code == result.TypeId);
                result.TypeName = dictionary != null ? dictionary.Name : "";
                var household = await this.householdService.GetHouseholdCodeDetail(result.ObjectId);
                result.Owner = household != null ? household.HouseholdMan : "";
                result.IconFiles = await this.sunFileInfoService.GetSunFileInfoList(result.IconId.ToString());
            }
            return result;
        }

        public async Task<GISImportRes> ImportExcelAsync(Stream fileStream, int category, int userId, int areaId)
        {
            GISImportRes res = new GISImportRes();
            DataSet dataset = Utils.ImportExcel(fileStream, 5, 6);
            int cnt = dataset.Tables.Count;
            if (cnt == 0)
            {
                return res;
            }
            if (areaId == 0)
            {
                throw new ValidException("区域id不能为0");
            }
            int index = 1;
            var dictionarys = await this.basicDictionaryService.GetBasicDictionaryList(7011);
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                string name = row.ItemArray[0].ToString();
                string typeName = row.ItemArray[1].ToString();
                string area = row.ItemArray[2].ToString();
                string phone = row.ItemArray[3].ToString();
                string address = row.ItemArray[4].ToString();
                string remark = row.ItemArray[5].ToString();
                if (name == "" && typeName == "" && area == "" && address == "" && remark == "" && phone == "")
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
                if (area == "") area = "0";
                decimal d_area = 0;
                try
                {
                    d_area = decimal.Parse(area);
                }
                catch
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
                _ = await this.SaveGreenHouseAsync(new GISGreenHouse()
                {
                    Name = name,
                    TypeId = typeId,
                    Address = address,
                    Area = d_area,
                    Unit = "亩",
                    Phone = phone,
                    Remark = remark,
                    Category = category,
                    ObjectId = 0, // 户码id
                    AreaId = areaId,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                });
            }
            return res;
        }

        public async Task<IPagedList<GISGreenHouseDto>> ListGreenHouseAsync(GISListQueryModel model)
        {
            Expression<Func<GISGreenHouse, bool>> expression = a => a.IsDeleted == 0;
            if (model.Category > 0)
            {
                expression = expression.And(a => a.Category == model.Category);
            }
            if (model.AreaId > 0)
            {
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
            var result = await this.GetQueryable().Where(expression).OrderByDescending(a => a.CreatedAt).ProjectTo<GISGreenHouseDto>(mapper.ConfigurationProvider).ToPagedListAsync(model.Page, model.Limit);

            List<int> userIds = result.Select(a => a.CreatedBy).ToList().Union(result.Select(a => a.UpdatedBy).ToList()).ToList();
            List<int> areaIds = result.Select(a => a.AreaId).Distinct().ToList();
            List<int> fileIds = result.Select(a => a.IconId).Distinct().ToList();

            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            var areaList = await this.basicAreaService.GetListAsync(a => areaIds.Contains(a.Id));
            var dictionaryList = await this.basicDictionaryService.GetBasicDictionaryList(7011);
            var sunFileInfos = await this.sunFileInfoService.GetSunFileInfoList(string.Join(",", fileIds));

            List<int> ids = result.Select(a => a.Id).Distinct().ToList();
            var list = await this.plotitemService.GetListAsync(a => a.IsDeleted == 0 && ids.Contains(a.ObjectId) && a.PlotType == (int)PlotType.GREENHOUSE);
            foreach (var item in result)
            {
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
                var area = areaList.FirstOrDefault(a => a.Id == item.AreaId);
                var dictionary = dictionaryList.FirstOrDefault(a => a.Code == item.TypeId);
                item.AreaName = area != null ? area.Name : "";
                item.TypeName = dictionary != null ? dictionary.Name : "";
                item.TypeDto = dictionary == null ? null : this.mapper.Map<BasicDictionaryDto>(dictionary);
                item.CreatedByName = user != null ? user.NickName : "";
                item.UpdatedByName = user != null ? user.NickName : "";
                item.IsPloted = list.FirstOrDefault(a => a.ObjectId == item.Id && a.PlotType == (int)PlotType.GREENHOUSE) == null ? false : true;
                var household = await this.householdService.GetHouseholdCodeDetail(item.ObjectId);
                item.Owner = household != null ? household.HouseholdMan : (item.ObjectId == 0 ? "村集体" : "");
                var icons = sunFileInfos.FindAll(a => a.Id == item.IconId);
                item.IconFiles = icons;
            }
            StaticPagedList<GISGreenHouseDto> pageList = new StaticPagedList<GISGreenHouseDto>(result, model.Page, model.Limit, result.TotalItemCount);
            return pageList;
        }

        public async Task<int> SaveGreenHouseAsync(GISGreenHouse model)
        {
            if (model == null)
            {
                throw new ValidException("参数无效");
            }
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ValidException("大棚名称不能为空");
            }

            Expression<Func<GISGreenHouse, bool>> pre = a => a.Name == model.Name && a.IsDeleted == 0;
            if (model.Category > 0)
            {
                pre = pre.And(a => a.Category == model.Category);
            }
            if (model.AreaId > 0)
            {
                pre = pre.And(a => a.AreaId == model.AreaId);
            }
            if (model.Id > 0)
            {
                pre = pre.And(a => a.Id != model.Id);
            }

            int cnt = this.GetQueryable().Where(pre).Count();
            if (cnt > 0)
            {
                throw new ValidException("大棚名称重复");
            }

            if (model.Id == 0)
            {
                model.UpdatedBy = model.CreatedBy;
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
                    data.TypeId = model.TypeId;
                    data.Area = model.Area;
                    data.Unit = model.Unit;
                    data.ObjectId = model.ObjectId;
                    //data.Owner = model.Owner;
                    data.Phone = model.Phone;
                }
                else
                {
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

        public async Task<int> BatchUpdateGreenHouseAsync(GISBatchDto model)
        {
            if (model == null)
            {
                throw new ValidException("参数无效");
            }

            var res = 0;
            if (model.TypeId > 0)
            {
                res = await this.GetQueryable().Where(a => model.Ids.Contains(a.Id)).UpdateFromQueryAsync(a => new GISGreenHouse()
                {
                    TypeId = model.TypeId,
                    UpdatedBy = model.UpdatedBy
                });
            }
            else
            {
                // 户码ObjectId=0 ，则为村集体
                res = await this.GetQueryable().Where(a => model.Ids.Contains(a.Id)).UpdateFromQueryAsync(a => new GISGreenHouse()
                {
                    ObjectId = model.ObjectId,
                    UpdatedBy = model.UpdatedBy
                });
            }
            if (res > 0)
            {
                return res;
            }
            return 0;
        }

        public async Task<byte[]> GetExcelData(int areaId, int typeId = 0, string keyword = "", string ids = "", int category = 0)
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
                Ids = ids,
                Page = 1,
                Limit = 10000,
            };
            IPagedList<GISGreenHouseDto> list = await this.ListGreenHouseAsync(body);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            package.Workbook.Properties.Title = "大棚管理";
            var workSheet = package.Workbook.Worksheets.Add("大棚管理");
            // 表头
            workSheet.Cells[1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1].Value = "序号";
            workSheet.Cells[1, 2].Value = "大棚名称";
            workSheet.Cells[1, 3].Value = "大棚所属";
            workSheet.Cells[1, 4].Value = "大棚类型";
            workSheet.Cells[1, 5].Value = "大棚面积";
            workSheet.Cells[1, 6].Value = "单位";
            workSheet.Cells[1, 7].Value = "大棚位置";
            workSheet.Cells[1, 8].Value = "联系人电话";
            workSheet.Cells[1, 9].Value = "备注";

            List<GISGreenHouseDto> items = list.ToList();
            for (int i = 0; i < items.Count; i++)
            {
                int rowIndex = i + 2;
                GISGreenHouseDto item = items[i];

                workSheet.Cells[rowIndex, 1].Value = i + 1;
                workSheet.Cells[rowIndex, 2].Value = item.Name;
                workSheet.Cells[rowIndex, 3].Value = item.Owner;
                workSheet.Cells[rowIndex, 4].Value = item.TypeName;
                workSheet.Cells[rowIndex, 5].Value = item.Area;
                workSheet.Cells[rowIndex, 6].Value = item.Unit;
                workSheet.Cells[rowIndex, 7].Value = item.Address;
                workSheet.Cells[rowIndex, 8].Value = item.Phone;
                workSheet.Cells[rowIndex, 9].Value = item.Remark;
            }
            return await package.GetAsByteArrayAsync();
        }
    }
}
