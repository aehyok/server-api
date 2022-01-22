using AutoMapper;
using AutoMapper.QueryableExtensions;
using DVS.Application.Services.Common;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
    public class FFPMatrixService : ServiceBase<FFPMatrix>, IFFPMatrixService
    {
        private readonly IBasicUserService basicUserService;

        public FFPMatrixService(DbContext dbContext, IMapper mapper, IBasicUserService basicUserService)  : base(dbContext, mapper)
        {
            this.basicUserService = basicUserService;
        }

        public async Task<int> DeleteMatrix(int id, int userid)
        {
            var data = await GetAsync(a => a.Id == id && a.IsDeleted == 0);
            if (data == null)
                throw new ValidException("数据不存在");

            data.IsDeleted = 1;
            data.UpdatedBy = userid;
            var ret = await this.UpdateAsync(data);
            if (ret > 0)
            {
                await this.ExecuteSqlAsync($"update FFPMatrixHousehold set isDeleted = 1,updatedBy={userid} where matrixId={id}");
            }
            return ret;
        }

        public async Task<FFPMatrixDto> DetailMatrix(int id)
        {
            var data = await this.GetAsync(a => a.Id == id && a.IsDeleted == 0);
            if (data == null)
                throw new ValidException("数据不存在");

            var result = mapper.Map<FFPMatrixDto>(data);

            var user = await this.basicUserService.GetAsync(a => a.Id == data.CreatedBy);
            if (user != null)
            {
                result.CreatedByName = user.NickName;
            }
            user = await this.basicUserService.GetAsync(a => a.Id == data.UpdatedBy);
            if (user != null)
            {
                result.UpdatedByName = user.NickName;
            }
            return result;
        }

        public async Task<byte[]> GetExcelData(string keyword, int inspector, string ids)
        {
            IPagedList<FFPMatrixDto> list = await this.ListMatrix(keyword, inspector, 1, 10000, ids);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            package.Workbook.Properties.Title = "区域网格";
            var workSheet = package.Workbook.Worksheets.Add("区域网格");
            // 表头
            workSheet.Cells[1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1].Value = "序号";
            workSheet.Cells[1, 2].Value = "网格名称";
            workSheet.Cells[1, 3].Value = "排序";
            workSheet.Cells[1, 4].Value = "备注";
            workSheet.Cells[1, 5].Value = "户数量";
            workSheet.Cells[1, 6].Value = "网格员";
            workSheet.Cells[1, 7].Value = "网格长";

            List<FFPMatrixDto> items = list.ToList();
            for (int i = 0; i < items.Count; i++)
            {
                int rowIndex = i + 2;
                FFPMatrixDto item = items[i];

                workSheet.Cells[rowIndex, 1].Value = i + 1;
                workSheet.Cells[rowIndex, 2].Value = item.Name;
                workSheet.Cells[rowIndex, 3].Value = item.Sequence;
                workSheet.Cells[rowIndex, 4].Value = item.Remark;
                workSheet.Cells[rowIndex, 5].Value = item.HouseCount;
                workSheet.Cells[rowIndex, 6].Value = item.InspectorName;
                workSheet.Cells[rowIndex, 7].Value = item.InspectorManagerName;
            }
            return await package.GetAsByteArrayAsync();
        }
    
        public async Task<IPagedList<FFPMatrixDto>> ListMatrix(string keyword, int inspector, int page, int limit, string ids = "")
        {
            Expression<Func<FFPMatrix, bool>> expression = a => a.IsDeleted == 0;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                expression = expression.And(a => a.Name.ToLower().Contains(keyword.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(ids)) {
                var ids_list = ids.Split(",");
                expression = expression.And(a => ids_list.Contains(a.Id.ToString()));
            }

            if (inspector > 0)
            {
                Expression<Func<FFPMatrix, bool>> expression_inspector = a => ("," + a.Inspector + ",").Contains("," + inspector.ToString() + ",");
                Expression<Func<FFPMatrix, bool>> expression_inspectorManager = a => ("," + a.InspectorManager + ",").Contains("," + inspector.ToString() + ",");                
                expression = expression.And(expression_inspector.Or(expression_inspectorManager));
            }

            var result = await this.GetQueryable().Where(expression).OrderBy(a => a.Sequence).ProjectTo<FFPMatrixDto>(mapper.ConfigurationProvider).ToPagedListAsync(page, limit);


            foreach (var item in result)
            {
                Expression<Func<BasicUser, bool>> expression_inspector = a => a.IsDeleted == 0;
                Expression<Func<BasicUser, bool>> expression_manager = a => a.IsDeleted == 0;
                if (!string.IsNullOrWhiteSpace(item.Inspector))
                {
                    var ids_inspector = item.Inspector.Split(",");
                    expression_inspector = expression_inspector.And(a => ids_inspector.Contains(a.Id.ToString()));
                    var inspectorlist = await this.basicUserService.GetListAsync(expression_inspector);
                    item.InspectorName = string.Join(",", inspectorlist.Select(a => a.NickName).ToList());
                }

                if (!string.IsNullOrWhiteSpace(item.InspectorManager))
                {
                    var ids_inspectormanager = item.InspectorManager.Split(",");
                    expression_manager = expression_manager.And(a => ids_inspectormanager.Contains(a.Id.ToString()));
                    var inspectormangerlist = await this.basicUserService.GetListAsync(expression_manager);
                    item.InspectorManagerName = string.Join(",", inspectormangerlist.Select(a => a.NickName).ToList());
                }
            }
            StaticPagedList<FFPMatrixDto> pageList = new StaticPagedList<FFPMatrixDto>(result, page, limit, result.TotalItemCount);
            return pageList;
        }

        public async Task<int> SaveMatrix(FFPMatrix matrix)
        {
            if (string.IsNullOrWhiteSpace(matrix.Name))
                throw new ValidException("参数无效");

            Expression<Func<FFPMatrix, bool>> pre = a => a.Name == matrix.Name && a.IsDeleted == 0;
            if (matrix.Id > 0)
            {
                pre = pre.And(a => a.Id != matrix.Id);
            }

            int cnt = this.GetQueryable().Where(pre).Count();
            if (cnt > 0)
            {
                throw new ValidException("名称重复");
            }

            if (matrix.Id > 0)
            {
                var data = await this.GetAsync(a => a.Id == matrix.Id && a.IsDeleted == 0);
                if (data == null)
                {
                    throw new ValidException("网格不存在");
                }
                data.Name = matrix.Name;
                data.Inspector = matrix.Inspector;
                data.InspectorManager = matrix.InspectorManager;
                data.UpdatedBy = matrix.UpdatedBy;
                data.Remark = matrix.Remark;
                data.Sequence = matrix.Sequence;
                return await this.UpdateAsync(data);
            }
            else
            {
                var result = await this.InsertAsync(matrix);
                if (result != null)
                {
                    return result.Id;
                }
                return 0;
            }
        }
    }
}
