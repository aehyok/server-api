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
    /// 收入来源管理
    /// </summary>
    [Route("/api/village/console")]
    public class IncomeController : DvsControllerBase
    {


        readonly IIncomeService service;
        public IncomeController(IIncomeService service)
        {

            this.service = service;
        }

        /// <summary>
        /// 收入来源管理
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("GetIncomeList")]
        [PermissionFilter(ModuleCodes.收入来源管理, PermissionCodes.List)]
        public async Task<IPagedList<VillageIncomeDto>> GetIncomeList([FromBody] PagePostBody body)
        {
            var data = await this.service.GetIncomeList(body);
            return data;
        }

        /// <summary>
        /// 收入来源详情
        /// </summary>
        /// <param name="householdId">户码Id</param>
        /// <param name="year">年度</param>
        /// <returns></returns>
        [HttpGet("GetIncomeDetail")]
        [PermissionFilter(ModuleCodes.收入来源管理, PermissionCodes.View)]
        public async Task<VillageIncome> GetIncomeDetail(int householdId, int year)
        {
            var data = await this.service.GetIncomeDetail(householdId, year);
            return data;
        }



        /// <summary>
        /// 添加收入来源
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("AddIncome")]
        [PermissionFilter(ModuleCodes.收入来源管理, PermissionCodes.Add)]
        public async Task<int> AddIncome(SaveIncomeBody body)
        {
            body.Id = 0;
            // body.AreaId = LoginUser.AreaId;
            var result = await service.SaveIncome(body);
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
        /// 编辑收入来源
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("EditIncome")]
        [PermissionFilter(ModuleCodes.收入来源管理, PermissionCodes.Edit)]
        public async Task<int> EditIncome(SaveIncomeBody body)
        {
            // body.AreaId = LoginUser.AreaId;

            var result = await service.SaveIncome(body);
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
        /// 删除收入来源
        /// </summary>
        /// <param name="body">id</param>
        /// <returns></returns>
        [HttpPost("DeleteIncome")]
        [PermissionFilter(ModuleCodes.收入来源管理, PermissionCodes.Remove)]
        public async Task<bool> DeleteIncome(DeleteIncomeBody body)
        {
            var res = await this.service.DeleteIncome(body.Id, body.HouseholdId);
            return res;
        }

        /// <summary>
        /// excel导出收入来源
        /// </summary>
        /// <param name="ids">例如1,2,3,4</param>
        /// <param name="areaId"></param>
        /// <param name="year"></param>
        /// <param name="keyword"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        [HttpGet("ExportIncome")]
        [PermissionFilter(ModuleCodes.收入来源管理, PermissionCodes.Export)]
        public async Task<FileContentResult> ExportIncome(string ids, int areaId = 0, int year = 0, string keyword = "", string orders = "")
        {
            //if (string.IsNullOrWhiteSpace(ids))
            //{

            //    throw new ValidException("缺少必要参数");
            //}
            var body = new PagePostBody()
            {
                Page = 1,
                Limit = 10000,
                //  = ids,
                AreaId = areaId,
                Year = year,
                Keyword = keyword,
            };
            if (!string.IsNullOrWhiteSpace(orders))
            {
                try
                {
                    List<OrderBy> orderList = JsonSerializer.Deserialize<List<OrderBy>>(orders, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    if (orderList != null)
                    {
                        body.Orders = orderList;
                    }
                }
                catch { }
            }
            var res = await this.service.GetIncomeList(body, ids);
            Dictionary<string, string> columns = new Dictionary<string, string>();
            // columns.Add("areaName", "区域名称");
            columns.Add("HouseName", "门牌名");
            columns.Add("HouseNumber", "门牌号");
            columns.Add("HouseholdMan", "户主姓名");
            columns.Add("Mobile", "联系方式");
            columns.Add("Year", "更新年份");
            columns.Add("TotalIncome", "家庭收入（单位：元）");



            byte[] filecontent = ExcelExportHelper.ExportExcel(res.ToList(), null, columns, false);
            return File(filecontent, ExcelExportHelper.ExcelContentType, $"收入来源信息{DateTime.Now.ToString("yyyyMMdd")}.xlsx");
        }
    }
}
