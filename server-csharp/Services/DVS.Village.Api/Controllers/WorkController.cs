using DVS.Application.Services;
using DVS.Application.Services.Village;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DVS.Core.Domains.Village;
using X.PagedList;
using DVS.Models.Dtos.Village.Query;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Models.Dtos.Village;
using DVS.Models.Enum;
using DVS.Common;
using DVS.Models.Dtos.Common;
using System.Text.Json;

namespace DVS.Village.Api.Controllers
{
    /// <summary>
    /// 外出务工管理
    /// </summary>
    [Route("/api/village/console")]
    public class WorkController : DvsControllerBase
    {

        private readonly IWorkService workService;
        public WorkController(IWorkService workService) {

            this.workService = workService;
        
        }

        /// <summary>
        /// 外出务工管理列表
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("GetWorkList")]
        [PermissionFilter(ModuleCodes.外出务工管理, PermissionCodes.List)]
        public async Task<IPagedList<VillageWorkDto>> GetWorkList([FromBody]PagePostBody body)
        {
            var data = await this.workService.GetWorkList(body);
            return data;
        }

        /// <summary>
        /// 务工人员详情信息
        /// </summary>
        /// <param name="householdId">户码Id</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        [HttpGet("GetWorkInfoList")]
        [PermissionFilter(ModuleCodes.外出务工管理, PermissionCodes.View)]
        public async Task<List<VillageWorkInfoDto>> GetWorkInfoList(int householdId,int year)
        {
            var data = await this.workService.GetWorkInfoList(householdId, year);
            return data;
        }


        /// <summary>
        /// 某个人口务工情况
        /// </summary>
        /// <param name="populationId">人口Id</param>
        /// <returns></returns>
        [HttpGet("GetUserWorkInfoList")]
        public async Task<List<VillageWorkInfoDto>> GetUserWorkInfoList(int populationId)
        {
            var data = await this.workService.GetWorkInfoList(populationId);
            return data;
        }

        /// <summary>
        /// 务工详情信息
        /// </summary>
        /// <param name="id">务工id</param>
        /// <returns></returns>
        [HttpGet("GetWorkInfoDetail")]
        [PermissionFilter(ModuleCodes.外出务工管理, PermissionCodes.View)]
        public async Task<VillageWorkInfoDto> GetWorkInfoDetail(int id)
        {
            var data = await this.workService.GetWorkInfoDetail(id);
            return data;
        }

        /// <summary>
        /// 登记外出务工信息
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("AddWorkInfo")]
        [PermissionFilter(ModuleCodes.外出务工管理, PermissionCodes.Add)]
        public async Task<bool> AddWorkInfo(SaveWorkBody body)
        {
            // body.AreaId = LoginUser.AreaId;
            body.Id = 0;
            var result = await this.workService.SaveWorkInfo(body);
            if (!result.Flag)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return result.Data;
            }
        }


        /// <summary>
        /// 编辑外出务工信息
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("EditWorkInfo")]
        [PermissionFilter(ModuleCodes.外出务工管理, PermissionCodes.Edit)]
        public async Task<bool> EditWorkInfo(SaveWorkBody body)
        {
            // body.AreaId = LoginUser.AreaId;
            // body.Id = 0;
            if (body.Id <= 0) {
                throw new ValidException("缺少Id");
            }
            var result = await this.workService.SaveWorkInfo(body);
            if (!result.Flag)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return result.Data;
            }
        }


        /// <summary>
        /// 删除外出务工信息
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("DeleteWrokInfo")]
        [PermissionFilter(ModuleCodes.外出务工管理, PermissionCodes.Remove)]
        public async Task<bool> DeleteWrokInfo(DeletePostBody body)
        {
            var result = await this.workService.DeleteWrokInfo(body.Id);
            return result;
        }

        /// <summary>
        /// excel导出外出务工
        /// </summary>
        /// <param name="ids"> 例如1,2,3,4</param>
        /// <param name="areaId"></param>
        /// <param name="year"></param>
        /// <param name="keyword"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        [HttpGet("ExportWork")]
        [PermissionFilter(ModuleCodes.外出务工管理, PermissionCodes.Export)]
        public async Task<FileContentResult> ExportWork(string ids, int areaId = 0, int year = 0, string keyword = "", string orders = "")
        {
            //if (string.IsNullOrWhiteSpace(ids))
            //{

            //    throw new ValidException("缺少必要参数");
            //}
            

            var body = new PagePostBody()
            {
                Page = 1,
                Limit = 10000,
                AreaId = areaId,
                Year = year,
                Keyword= keyword,
                
            };
            if (!string.IsNullOrWhiteSpace(orders)) {
                try {
                    List<OrderBy> orderList = JsonSerializer.Deserialize<List<OrderBy>>(orders, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    if (orderList != null)
                    {
                        body.Orders = orderList;
                    }
                }
                catch{ }
            }
            var res = await this.workService.GetWorkList(body, ids);

            foreach (var item in res)
            {
                item.HouseholdMan = item.HouseholdMan == null ? "" : item.HouseholdMan;
                item.Mobile = item.Mobile == null ? "" : item.Mobile;
                item.Nation = item.UpdatedAt != null ? DateTime.Parse(item.UpdatedAt.ToString()).ToString("yyyy-MM-dd HH:mm:ss") : "";
            }


            Dictionary<string, string> columns = new Dictionary<string, string>();
            // columns.Add("areaName", "区域名称");
            columns.Add("HouseName", "门牌名");
            columns.Add("HouseNumber", "门牌号");
            columns.Add("HouseholdMan", "户主姓名");
            columns.Add("Mobile", "联系方式");
            columns.Add("PeopleCount", "外出务工累计人次");
            columns.Add("Nation", "最后操作时间");



            byte[] filecontent = ExcelExportHelper.ExportExcel(res.ToList(), null, columns, false);
            return File(filecontent, ExcelExportHelper.ExcelContentType, $"外出务工信息{DateTime.Now.ToString("yyyyMMdd")}.xlsx");
        }

    }
}
