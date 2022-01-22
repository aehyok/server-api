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
using DVS.Common;
using System.Data;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DVS.Application.Services.GIS
{
    public class GISCameraService : ServiceBase<GISCamera>, IGISCameraService
    {
        readonly IBasicUserService basicUserService;
        readonly IBasicAreaService basicAreaService;
        readonly IGISPlotItemService plotitemService;
        readonly IParkAreaService parkAreaService;
        readonly ISunFileInfoService sunFileInfoService;
        public GISCameraService(DbContext dbContext, IMapper mapper, IBasicUserService basicUserService, IBasicAreaService basicAreaService, IGISPlotItemService plotitemService, IParkAreaService parkAreaService, ISunFileInfoService sunFileInfoService)
            : base(dbContext, mapper)
        {
            this.basicAreaService = basicAreaService;
            this.basicUserService = basicUserService;
            this.plotitemService = plotitemService;
            this.parkAreaService = parkAreaService;
            this.sunFileInfoService = sunFileInfoService;
        }

        public async Task<int> DeleteCameraAsync(int id, int userid)
        {
            var camera = await this.GetAsync(a => a.Id == id);
            if (camera == null)
            {
                throw new ValidException("数据不存在");
            }

            camera.IsDeleted = 1;
            camera.UpdatedBy = userid;

            var ret = await this.UpdateAsync(camera);
            if (ret > 0)
            {
                await this.ExecuteSqlAsync($"update GISPlotItem set isDeleted = 1,updatedBy={userid},isSync=0 where objectId={id} and plottype = " + (int)PlotType.CAMERA);
            }
            return ret;
        }

        public async Task<GISCameraDto> DetailCameraAsync(int id)
        {
            var camera = await this.GetAsync(a => a.Id == id);
            if (camera == null)
            {
                throw new ValidException("数据不存在");
            }

            var result = mapper.Map<GISCameraDto>(camera);
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

                result.IconFiles = await this.sunFileInfoService.GetSunFileInfoList(result.IconId.ToString());
            }
            return result;
        }

        public async Task<IPagedList<GISCameraDto>> ListCameraAsync(GISListQueryModel model)
        {

            Expression<Func<GISCamera, bool>> expression = a => a.IsDeleted == 0;
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

            if(!string.IsNullOrWhiteSpace(model.Ids))
            {
                var _ids = model.Ids.Split(",");
                expression = expression.And(a => _ids.Contains(a.Id.ToString()));
            }

            var result = await this.GetQueryable().Where(expression).OrderByDescending(a => a.CreatedAt).ProjectTo<GISCameraDto>(mapper.ConfigurationProvider).ToPagedListAsync(model.Page, model.Limit);

            List<int> userIds = result.Select(a => a.CreatedBy).ToList().Union(result.Select(a => a.UpdatedBy).ToList()).ToList();
            List<int> areaIds = result.Select(a => a.AreaId).Distinct().ToList();
            List<int> parkIds = result.Where(a => a.Category == 2 && a.ObjectId > 0).Select(a => a.ObjectId).Distinct().ToList();
            List<int> fileIds = result.Select(a => a.IconId).Distinct().ToList();

            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            var areaList = await this.basicAreaService.GetListAsync(a => areaIds.Contains(a.Id));
            var parkList = (model.Category == 2) ? await this.parkAreaService.GetListAsync(a => parkIds != null ? parkIds.Contains(a.Id) : false) : null;

            List<int> ids = result.Select(a => a.Id).Distinct().ToList();
            var list = await this.plotitemService.GetListAsync(a => a.IsDeleted == 0 && ids.Contains(a.ObjectId) && a.PlotType == (int)PlotType.CAMERA);

            var sunFileInfos = await this.sunFileInfoService.GetSunFileInfoList(string.Join(",", fileIds));
            foreach (var item in result)
            {
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
                var area = areaList.FirstOrDefault(a => a.Id == item.AreaId);
                item.AreaName = area != null ? area.Name : "";
                item.CreatedByName = user != null ? user.NickName : "";
                item.UpdatedByName = user != null ? user.NickName : "";
                var icons = sunFileInfos.FindAll(a => a.Id == item.IconId);
                item.IconFiles = icons;
                item.IsPloted = list.FirstOrDefault(a => a.ObjectId == item.Id && a.PlotType == (int)PlotType.CAMERA) == null ? false : true;
                if (model.Category == 2) {
                    var park = parkList.FirstOrDefault(a => a.Id == item.ObjectId);
                    item.ParkName = park != null ? park.EnterpriseName : "";
                }
            }
            StaticPagedList<GISCameraDto> pageList = new StaticPagedList<GISCameraDto>(result, model.Page, model.Limit, result.TotalItemCount);
            return pageList;
        }

        public async Task<int> SaveCameraAsync(GISCamera model)
        {
            if (model == null)
            {
                throw new ValidException("参数无效");
            }
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ValidException("摄像头名称不能为空");
            }

            Expression<Func<GISCamera, bool>> pre = a => a.Name == model.Name && a.IsDeleted == 0;
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
                throw new ValidException("摄像头名称重复");
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
                    data.StreamUrl = model.StreamUrl;
                    data.Url = model.Url;
                    data.Address = model.Address;
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

        public async Task<GISImportRes> ImportExcelAsync(Stream fileStream, int category, int userId, int areaId, int objectId)
        {
            GISImportRes res = new GISImportRes();
            if (areaId == 0)
            {
                throw new ValidException("区域id不能为0");
            }
            DataSet dataset = Utils.ImportExcel(fileStream, 5, 5);
            int cnt = dataset.Tables.Count;
            if (cnt == 0)
            {
                return res;
            }
            int index = 1;
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                string name = row.ItemArray[0].ToString();
                string address = row.ItemArray[1].ToString();
                string streamUrl = row.ItemArray[2].ToString();
                string url = row.ItemArray[3].ToString();
                string remark = row.ItemArray[4].ToString();
                if (name == "" && address == "" && remark == "" && streamUrl == "" && url == "")
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
                _ = await this.SaveCameraAsync(new GISCamera()
                {
                    Name = name,
                    Address = address,
                    StreamUrl = streamUrl,
                    Url = url,
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
            IPagedList<GISCameraDto> list = await this.ListCameraAsync(body);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            package.Workbook.Properties.Title = "摄像头";
            var workSheet = package.Workbook.Worksheets.Add("摄像头");
            // 表头
            workSheet.Cells[1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1].Value = "序号";
            workSheet.Cells[1, 2].Value = "摄像头名称";
            workSheet.Cells[1, 3].Value = "摄像头流地址";
            workSheet.Cells[1, 4].Value = "直播流地址";
            workSheet.Cells[1, 5].Value = "摄像头位置";
            workSheet.Cells[1, 6].Value = "备注";

            List<GISCameraDto> items = list.ToList();
            for (int i = 0; i < items.Count; i++)
            {
                int rowIndex = i + 2;
                GISCameraDto item = items[i];

                workSheet.Cells[rowIndex, 1].Value = i + 1;
                workSheet.Cells[rowIndex, 2].Value = item.Name;
                workSheet.Cells[rowIndex, 3].Value = item.StreamUrl;
                workSheet.Cells[rowIndex, 4].Value = item.Url;
                workSheet.Cells[rowIndex, 5].Value = item.Address;
                workSheet.Cells[rowIndex, 6].Value = item.Remark;
            }
            return await package.GetAsByteArrayAsync();
        }
    }
}
