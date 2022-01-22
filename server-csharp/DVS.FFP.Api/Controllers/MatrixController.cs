using DVS.Application.Services.FFP;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DVS.FFP.Api.Controllers
{
    /// <summary>
    /// 网格管理
    /// </summary>
    [ApiController]
    [Route("api/ffp")]
    public class MatrixController : DvsControllerBase
    {
        private readonly IFFPMatrixService FFPMatrixService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public MatrixController(IFFPMatrixService service)
        {
            this.FFPMatrixService = service;
        }

        /// <summary>
        /// 查询网格列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/matrix/list")]
        //[PermissionFilter("区域网格管理", 128)]
        public async Task<IEnumerable<FFPMatrixDto>> GetMatrixListAsync(MatrixListQueryModel model)
        {
            return await this.FFPMatrixService.ListMatrix(model.Keyword, model.Inspector, model.Page, model.Limit);
        }

        /// <summary>
        /// 查询网格详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/matrix/detail")]
        //[PermissionFilter("区域网格管理", 8)]
        public async Task<FFPMatrixDto> GetMatrixDetailAsync(DetailQueryModel model)
        {
            return await this.FFPMatrixService.DetailMatrix(model.Id);
        }

        /// <summary>
        /// 删除网格
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/matrix/delete")]
        //[PermissionFilter("区域网格管理", 2)]
        public async Task<int> DeleteMatrixAsync(DetailQueryModel model)
        {
            LoginUser loginUser = this.LoginUser;
            return await this.FFPMatrixService.DeleteMatrix(model.Id, loginUser.UserId);
        }

        /// <summary>
        /// 增加网格
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/matrix/add")]
        //[PermissionFilter("区域网格管理", 1)]
        public async Task<int> AddMatrixAsync(FFPMatrix model)
        {
            LoginUser loginUser = this.LoginUser;
            model.CreatedBy = loginUser.UserId;
            return await this.FFPMatrixService.SaveMatrix(model);
        }

        /// <summary>
        /// 修改网格
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/matrix/edit")]
        //[PermissionFilter("区域网格管理", 4)]
        public async Task<int> EditMatrixAsync(FFPMatrix model)
        {
            LoginUser loginUser = this.LoginUser;
            model.UpdatedBy = loginUser.UserId;
            return await this.FFPMatrixService.SaveMatrix(model);
        }

        /// <summary>
        /// 查询网格所属户码列表
        /// </summary>
        /// <param name="service"></param> 
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/matrix/listmatrixhousehold")]
        //[PermissionFilter("区域网格管理", 128)]
        public async Task<IEnumerable<FFPMatrixHouseholdDto>> GetMatrixHouseholdListAsync([FromServices] IFFPMatrixHouseholdService service, MatrixListQueryModel model)
        {
            return await service.ListMatrixHousehold(model.Keyword, model.AreaId, model.MatrixId, model.Orders, model.Page, model.Limit);
        }


        /// <summary>
        /// 添加户码到网格
        /// </summary>
        /// <param name="service"></param> 
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/matrix/addhousehold")]
        //[PermissionFilter("区域网格管理", 1)]
        public async Task<int> AddMatrixHouseholdAsync([FromServices] IFFPMatrixHouseholdService service, AddQueryModel model)
        {
            LoginUser loginUser = this.LoginUser;
            return await service.SaveMatrixHousehold(model.MatrixId, model.HouseholdIds,loginUser.UserId);
        }

        /// <summary>
        /// 从网格中移除户码
        /// </summary>
        /// <param name="service"></param> 
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/matrix/removehousehold")]
        //[PermissionFilter("区域网格管理", 2)]
        public async Task<int> RemoveMatrixHouseholAsync([FromServices] IFFPMatrixHouseholdService service, RemoveQueryModel model)
        {
            LoginUser loginUser = this.LoginUser;
            return await service.RemoveMatrixHousehold(model.MatrixId, model.HouseholdIds, loginUser.UserId);
        }

        /// <summary>
        /// 查询户码列表
        /// </summary>
        /// <param name="service"></param> 
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/matrix/listhousehold")]
        //[PermissionFilter("区域网格管理", 128)]
        public async Task<IEnumerable<FFPMatrixHouseholdDto>> GetHouseholdListAsync([FromServices] IFFPMatrixHouseholdService service, MatrixListQueryModel model)
        {
            return await service.ListHousehold(model.Keyword, model.AreaId, model.IsUsed, model.Orders, model.Page, model.Limit);
        }

        /// <summary>
        /// 查询花名册
        /// </summary>
        /// <param name="service"></param> 
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/roster/list")]
        //[PermissionFilter("花名册管理", 128)]
        public async Task<IEnumerable<FFPMatrixHouseholdDto>> GetHouseholdByMatrixListAsync([FromServices] IFFPMatrixHouseholdService service, MatrixListQueryModel model)
        {
            return await service.ListHouseholdByMatrix(model.Keyword, model.AreaId, model.MatrixId,model.HouseholdTypes, model.Inspector, model.Orders, model.Page, model.Limit);
        }

        /// <summary>
        /// 统计花名册数量
        /// </summary>
        /// <param name="service"></param> 
        /// <returns></returns>
        [HttpPost("app/roster/statistic")]
        public async Task<FFPHouseholdStatisticDto> StatisticHouseholdAsync([FromServices] IFFPMatrixHouseholdService service)
        {
            LoginUser loginUser = this.LoginUser;
            return await service.StatisticHousehold(loginUser.UserId);
        }

        /// <summary>
        /// 监测对象统计
        /// </summary>
        /// <param name="service"></param> 
        /// <returns></returns>
        [HttpPost("app/roster/statisticmonitor")]
        public async Task<FFPHouseholdStatisticDto> StatisticMonitorHouseholdAsync([FromServices] IFFPMatrixHouseholdService service)
        {
            LoginUser loginUser = this.LoginUser;
            return await service.StatisticMonitorHousehold(loginUser.UserId);
        }

        /// <summary>
        /// 检测对象分类统计
        /// </summary>
        /// <param name="service"></param> 
        /// <returns></returns>
        [HttpPost("app/roster/statisticmonitortype")]
        public async Task<List<FFPHouseholdStatisticDto>> StatisticMonitorHouseholdByTypeAsync([FromServices] IFFPMatrixHouseholdService service)
        {
            LoginUser loginUser = this.LoginUser;
            return await service.StatisticMonitorHouseholdByType(loginUser.UserId);
        }

        /// <summary>
        /// 查询花名册
        /// </summary>
        /// <param name="service"></param> 
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("app/roster/list")]
        public async Task<IEnumerable<FFPMatrixHouseholdDto>> GetHouseholdMatrixListAsync([FromServices] IFFPMatrixHouseholdService service, MatrixListQueryModel model)
        {
            LoginUser loginUser = this.LoginUser;
            return await service.ListHouseholdByMatrix(model.Keyword, 0, 0, model.HouseholdTypes, loginUser.UserId, model.Orders, model.Page, model.Limit, model.HouseholdId);
        }

        /// <summary>
        /// 区域网格导出
        /// </summary>
        /// <param name="inspector"></param>
        /// <param name="keyword"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet("console/matrix/export")]
        //[PermissionFilter(ModuleCodes.区域网格管理, PermissionCodes.Export)]
        public async Task<FileContentResult> ExportAsync(string keyword = "", int inspector = 0, string ids = "")
        {
            byte[] exportBytes = await this.FFPMatrixService.GetExcelData(keyword, inspector, ids);
            return File(exportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }


        /// <summary>
        /// 花名册导出
        /// </summary>
        /// <param name="service"></param>
        /// <param name="matrixId"></param>
        /// <param name="inspector"></param>
        /// <param name="keyword"></param>
        /// <param name="areaId"></param>
        /// <param name="householdTypes"></param>
        /// <param name="houseName"></param>
        /// <param name="houseNumber"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet("console/roster/export")]
        //[PermissionFilter(ModuleCodes.花名册管理, PermissionCodes.Export)]
        public async Task<FileContentResult> RosterExportAsync([FromServices] IFFPMatrixHouseholdService service, int matrixId, int inspector, string keyword, int areaId, string householdTypes, string houseName = "", string houseNumber = "", string ids = "")
        {
            byte[] exportBytes = await service.GetExcelData(matrixId, inspector, keyword, areaId, householdTypes, houseName, houseNumber, ids);
            return File(exportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
