using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Application.Services.Village;
using DVS.Common;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.GIS;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Dtos.Village.Farmland;
using DVS.Models.Enum;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.GIS
{
    public class GISFarmLandService : ServiceBase<VillageFarmland>, IGISFarmLandService
    {
        readonly IVillageFarmlandService farmlandService;
        readonly IBasicDictionaryService basicDictionaryService;
        public GISFarmLandService(DbContext dbContext, IMapper mapper, IVillageFarmlandService farmlandService, IBasicDictionaryService basicDictionaryService) : base(dbContext, mapper)
        {
            this.farmlandService = farmlandService;
            this.basicDictionaryService = basicDictionaryService;
        }

        public async Task<int> DeleteFarmLandAsync(int id, int userid)
        {
            var model = await this.farmlandService.GetAsync(a => a.Id == id);
            if (model == null)
            {
                throw new ValidException("数据不存在");
            }

            model.IsDeleted = 1;
            model.UpdatedBy = userid;

            var ret = await this.farmlandService.UpdateAsync(model);
            if (ret > 0)
            {
                await this.ExecuteSqlAsync($"update GISPlotItem set isDeleted = 1,updatedBy={userid},isSync=0 where objectId={id} and plottype = " + (int)PlotType.FARMLAND);
            }
            return ret;
        }

        public async Task<VillageFarmlandDto> DetailFarmLandAsync(int id)
        {
            var farmland = await this.farmlandService.GetDetail(id);
            if (farmland == null)
            {
                throw new ValidException("数据不存在");
            }
            return farmland;
        }

        public async Task<GISImportRes> ImportExcelAsync(Stream fileStream, int category, int usefor, int userid, int areaid, int objectId = 0)
        {
            DataSet dataset = null;
            GISImportRes res = new GISImportRes();
            if (category == 1) // 土地管理，户主地块导入
            {
                dataset = Utils.ImportExcel(fileStream, 5, 5);
            }
            else
            { // 园区地块导入
                dataset = Utils.ImportExcel(fileStream, 5, 4);
            }
            int cnt = dataset.Tables.Count;
            if (cnt == 0)
            {
                return res;
            }
            if ((areaid == 0) || (category == 2 && objectId == 0))
            {
                throw new ValidException("区域id或园区id不能为0");
            }
            int index = 1;
            Dictionary<int, string> fail = new Dictionary<int, string>();
            if (category == 1)
            {
                var dictionarys = await this.basicDictionaryService.GetBasicDictionaryList(3010); // 地块类型
                if (usefor == 2)
                {
                    dictionarys = await this.basicDictionaryService.GetBasicDictionaryList(3011); // 规划地块类型
                }
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    string name = row.ItemArray[0].ToString();
                    string typeName = row.ItemArray[1].ToString();
                    string area = row.ItemArray[2].ToString();
                    string address = row.ItemArray[3].ToString();
                    string remark = row.ItemArray[4].ToString();
                    string unit = "亩";
                    if (name == "" && typeName == "" && area == "" && address == "" && remark == "")
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
                    if (typeName == "宅基地")
                    {
                        unit = "M²";
                    }
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
                    var ret = await this.ImportFarmLandAsync(new VillageFarmland()
                    {
                        Name = name,
                        TypeId = typeId,
                        Area = d_area,
                        Unit = unit,
                        Address = address,
                        Remark = remark,
                        Category = category,
                        UseFor = usefor,
                        AreaId = areaid,
                        HouseholdId = objectId,
                    }, userid);

                    if (ret == -1)
                    {
                        res.Fail.Add(new GISImportFailInfo() { Index = index, Message = "数据错误" });
                    }
                    else
                    {
                        res.Ok++;
                    }
                    index++;
                }
            }
            else
            {
                // 园区地块导入，没有地块类型
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    string name = row.ItemArray[0].ToString();
                    string area = row.ItemArray[1].ToString();
                    string unit = "亩";
                    string address = row.ItemArray[2].ToString();
                    string remark = row.ItemArray[3].ToString();

                    if (name == "" && area == "" && unit == "" && address == "" && remark == "")
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
                    var data = new VillageFarmland()
                    {
                        Name = name,
                        Area = d_area,
                        Unit = unit,
                        Address = address,
                        Remark = remark,
                        Category = category,
                        UseFor = usefor,
                        AreaId = areaid,
                    };
                    if (category == 2)
                    { // 园区地块导入
                        data.ParkId = objectId;
                    }
                    var ret = await this.ImportFarmLandAsync(data, userid);
                    if (ret == -1)
                    {
                        res.Fail.Add(new GISImportFailInfo() { Index = index, Message = "数据错误" });
                    }
                    else
                    {
                        res.Ok++;
                    }
                    index++;
                }
            }
            return res;
        }

        public async Task<IPagedList<VillageFarmlandDto>> ListFarmLandAsync(GISListQueryModel model)
        {
            return await this.farmlandService.GetFarmlands(model.AreaId, model.TypeId, model.Keyword, model.ObjectId, model.Category, model.UseFor, model.Page, model.Limit, null, model.Ids);
        }

        public async Task<int> SaveFarmLandAsync(VillageFarmland model, int userId)
        {
            return await this.farmlandService.Save(model, userId);
        }

        public async Task<int> ImportFarmLandAsync(VillageFarmland model, int userId)
        {
            return await this.farmlandService.SaveImport(model, userId);
        }

        public async Task<int> BatchUpdateFarmLandAsync(GISBatchDto model)
        {
            if (model == null)
            {
                throw new ValidException("参数无效");
            }

            var res = 0;
            if (model.TypeId > 0)
            {
                res = await this.GetQueryable().Where(a => model.Ids.Contains(a.Id)).UpdateFromQueryAsync(a => new VillageFarmland()
                {
                    TypeId = model.TypeId,
                    UpdatedBy = model.UpdatedBy,
                    Unit = model.TypeId == 3010101 ? "M²" : "亩"
                });

            }
            else if (!string.IsNullOrWhiteSpace(model.Unit))
            {
                res = await this.GetQueryable().Where(a => model.Ids.Contains(a.Id)).UpdateFromQueryAsync(a => new VillageFarmland()
                {
                    Unit = model.Unit,
                    UpdatedBy = model.UpdatedBy,
                });
            }
            else
            {
                // HouseholdId = 0 ,为村集体
                res = await this.GetQueryable().Where(a => model.Ids.Contains(a.Id)).UpdateFromQueryAsync(a => new VillageFarmland()
                {
                    HouseholdId = model.HouseholdId,
                    UpdatedBy = model.UpdatedBy,
                });
            }
            if (res > 0)
            {
                return res;
            }
            return 0;
        }

        public async Task<byte[]> GetLandExcelData(int areaId, int typeId = 0, string keyword = "", string ids = "", int category = 0, int usefor = 0, int objectId = 0)
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
                UseFor = usefor,
                ObjectId = objectId,
                Ids = ids,
                Page = 1,
                Limit = 10000,
            };
            IPagedList<VillageFarmlandDto> list = await this.ListFarmLandAsync(body);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            package.Workbook.Properties.Title = "土地管理";
            if (category == 1)
            {
                var workSheet = package.Workbook.Worksheets.Add("土地管理");
                // 表头
                if (usefor == 1) // 1 普通用地 ,2 规划用地
                {
                    workSheet.Cells[1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[1, 1].Value = "序号";
                    workSheet.Cells[1, 2].Value = "地块名称";
                    workSheet.Cells[1, 3].Value = "地块所属";
                    workSheet.Cells[1, 4].Value = "地块类型";
                    workSheet.Cells[1, 5].Value = "地块面积";
                    workSheet.Cells[1, 6].Value = "单位";
                    workSheet.Cells[1, 7].Value = "地块位置";
                    workSheet.Cells[1, 8].Value = "备注";
                }
                else
                {
                    workSheet.Cells[1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[1, 1].Value = "序号";
                    workSheet.Cells[1, 2].Value = "地块名称";
                    workSheet.Cells[1, 3].Value = "地块类型";
                    workSheet.Cells[1, 4].Value = "地块面积";
                    workSheet.Cells[1, 5].Value = "单位";
                    workSheet.Cells[1, 6].Value = "地块位置";
                    workSheet.Cells[1, 7].Value = "备注";
                }

                List<VillageFarmlandDto> lands = list.ToList();
                for (int i = 0; i < lands.Count; i++)
                {
                    int rowIndex = i + 2;
                    VillageFarmlandDto land = lands[i];

                    workSheet.Cells[rowIndex, 1].Value = i + 1;
                    workSheet.Cells[rowIndex, 2].Value = land.Name;
                    if (usefor == 1)
                    {
                        workSheet.Cells[rowIndex, 3].Value = land.Householder;
                        workSheet.Cells[rowIndex, 4].Value = land.TypeName;
                        workSheet.Cells[rowIndex, 5].Value = land.Area;
                        workSheet.Cells[rowIndex, 6].Value = land.Unit;
                        workSheet.Cells[rowIndex, 7].Value = land.Address;
                        workSheet.Cells[rowIndex, 8].Value = land.Remark;
                    }
                    else
                    {
                        workSheet.Cells[rowIndex, 3].Value = land.TypeName;
                        workSheet.Cells[rowIndex, 4].Value = land.Area;
                        workSheet.Cells[rowIndex, 5].Value = land.Unit;
                        workSheet.Cells[rowIndex, 6].Value = land.Address;
                        workSheet.Cells[rowIndex, 7].Value = land.Remark;
                    }
                }
            }
            else
            {
                package.Workbook.Properties.Title = "园区企业土地管理";
                var workSheet = package.Workbook.Worksheets.Add("园区企业土地管理");

                // 表头
                workSheet.Cells[1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[1, 1].Value = "序号";
                workSheet.Cells[1, 2].Value = "地块名称";
                workSheet.Cells[1, 3].Value = "地块面积";
                workSheet.Cells[1, 4].Value = "单位";
                workSheet.Cells[1, 5].Value = "地块位置";
                workSheet.Cells[1, 6].Value = "备注";

                List<VillageFarmlandDto> lands = list.ToList();
                for (int i = 0; i < lands.Count; i++)
                {
                    int rowIndex = i + 2;
                    VillageFarmlandDto land = lands[i];

                    workSheet.Cells[rowIndex, 1].Value = i + 1;
                    workSheet.Cells[rowIndex, 2].Value = land.Name;
                    workSheet.Cells[rowIndex, 3].Value = land.Area;
                    workSheet.Cells[rowIndex, 4].Value = land.Unit;
                    workSheet.Cells[rowIndex, 5].Value = land.Address;
                    workSheet.Cells[rowIndex, 6].Value = land.Remark;
                }
            }
            return await package.GetAsByteArrayAsync();
        }
    }
}
