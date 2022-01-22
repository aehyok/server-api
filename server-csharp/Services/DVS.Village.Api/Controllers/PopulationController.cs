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
using Microsoft.AspNetCore.Http;
using System.IO;
using DVS.Common;
using DVS.Models.Dtos.Common;
using System.Text.Json;
using DVS.Models.Dtos.Village.Household;
using DVS.Models.Enum;

namespace DVS.Village.Api.Controllers
{
    /// <summary>
    /// 户籍人口管理
    /// </summary>
    [Route("/api/village/console")]
    public class PopulationController : DvsControllerBase
    {

        private IPopulationService service;
        public PopulationController(IPopulationService service)
        {
            this.service = service;
        }

        /// <summary>
        /// 户籍人口列表
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("GetPopulationList")]
        [PermissionFilter(ModuleCodes.户籍人口, PermissionCodes.List)]
        public async Task<IPagedList<PopulationDto>> GetPopulationList([FromBody] PopulationListBody body)
        {
            var data = await this.service.GetPopulationList(body);
            return data;
        }


        /// <summary>
        /// 微信通知发布范围户籍人口列表获取
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("GetWechatMessagePopulationList")]
        [AllowAnonymous]
        public async Task<List<WechatMessagePopulationDto>> GetWechatMessagePopulationList(WechatMessagePopulationQuery body)
        {
            var data = await this.service.GetPopulationList(body);
            return data;
        }

        /// <summary>
        /// 户籍人口详情
        /// </summary>
        /// <param name="service"></param>
        /// <param name="id"></param>
        /// <param name="idCard"></param>
        /// <param name="isConvert">是否转字典Id成文字，1转，0不转</param>
        ///  <param name="householdId">户码Id</param>
        /// <returns></returns>
        [HttpGet("GetPopulationDetail")]
        [PermissionFilter(ModuleCodes.户籍人口, PermissionCodes.View)]
        public async Task<PopulationDetailDto> GetPopulationDetail(int id, string idCard = "", int isConvert = 0, int householdId = 0)
        {
            var data = await this.service.GetPopulationDetail(id, idCard, isConvert, 0, householdId);
            return data;
        }

        /// <summary>
        /// 户籍人口详情
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="idCard"></param>
        /// <param name="isConvert">是否转字典Id成文字，1转，0不转</param>
        ///  <param name="householdId">户码Id</param>
        /// <returns></returns>
        [HttpGet("GetPopulationDetailByIdCard")]
        [PermissionFilter(ModuleCodes.户籍人口, PermissionCodes.View)]
        public async Task<PopulationDetailDto> GetPopulationDetailByIdCard(string idCard, int areaId, int isConvert = 0, int householdId = 0)
        {
            var data = await this.service.GetPopulationDetail(0, idCard, isConvert, 0, householdId, areaId);
            return data;
        }

        /// <summary>
        /// 添加编辑户籍人口
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        [HttpPost("AddPopulation")]
        [PermissionFilter(ModuleCodes.户籍人口, PermissionCodes.Add)]
        public async Task<bool> AddPopulation(PopulationDetailDto population)
        {
            population.Id = 0;
            var result = await this.service.SavePopulation(population, LoginUser.UserId);
            if (result.Flag == false)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 添加编辑户籍人口
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        [HttpPost("EditPopulation")]
        [PermissionFilter(ModuleCodes.户籍人口, PermissionCodes.Edit)]
        public async Task<bool> EditPopulation(PopulationDetailDto population)
        {
            if (population.Id <= 0)
            {
                throw new ValidException("请输入Id");
            }
            var result = await this.service.SavePopulation(population, LoginUser.UserId);
            if (result.Flag == false)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 设置与户主管理
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("SetHouseholdRelationship")]
        [PermissionFilter(ModuleCodes.户码管理, PermissionCodes.Edit)]
        public async Task<bool> SetHouseholdRelationship(SetRelationshipBody body)
        {
            var result = await this.service.SetHouseholdRelationship(body);
            if (result.Flag == false)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 移除，加入户码
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("SetFromHousehold")]
        [PermissionFilter(ModuleCodes.户码管理, PermissionCodes.Edit)]
        public async Task<bool> SetFromHousehold(SetFromHouseholdBody body)
        {
            var result = await this.service.SetFromHousehold(body);
            if (result.Flag == false)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// excel导入户籍人口
        /// </summary>
        /// <param name="excelFile">导入文件名</param>
        /// <param name="areaId">区域id</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("ImportPopulation")]
        [PermissionFilter(ModuleCodes.户籍人口, PermissionCodes.Import)]
        public async Task<ImportResultDto> ImportPopulation(IFormFile excelFile, int areaId)
        {
            if (areaId <= 0)
            {
                throw new ValidException("缺少必要参数");
            }
            Stream fileStream = excelFile.OpenReadStream();
            var res = await this.service.ImportPopulation(fileStream, areaId);
            if (!res.Flag)
            {
                throw new ValidException(res.Message);
            }
            return res.Data;
            //var res = DVS.Common.Utils.SplitAddress("广东省深圳市南山区西丽镇九祥岭村80号");
            //return 0;
        }
        /// <summary>
        /// excel导入户籍人口，模板包含门牌名和门牌号
        /// </summary>
        /// <param name="excelFile">导入文件名</param>
        /// <param name="areaId">区域id</param>
        /// <returns></returns>
        [HttpPost("ImportPopulationAndHouseName")]
        [PermissionFilter(ModuleCodes.户籍人口, PermissionCodes.Import)]
        public async Task<string> ImportPopulationAndHouseName(IFormFile excelFile, int areaId)
        {
            if (areaId <= 0)
            {
                throw new ValidException("缺少必要参数");
            }
            Stream fileStream = excelFile.OpenReadStream();
            var res = await this.service.ImportPopulationAndHouseName(fileStream, areaId);
            return res;
        }

        /// <summary>
        /// excel导出户籍人口
        /// </summary>
        /// <param name="ids">例如1,2,3,4</param>
        /// <param name="areaId"></param>
        /// <param name="tags"></param>
        /// <param name="keyword"></param>
        /// <param name="orders"></param>
        /// <param name="householdId"></param>
        /// <returns></returns>
        [HttpGet("ExportPopulation")]
        [PermissionFilter(ModuleCodes.户籍人口, PermissionCodes.Export)]
        public async Task<FileContentResult> ExportPopulation(string ids, int areaId = 0, string tags = "", string keyword = "", string orders = "",int householdId = 0)
        {
            //if (string.IsNullOrWhiteSpace(ids))
            //{

            //    throw new ValidException("缺少必要参数");
            //}

            var body = new PopulationListBody()
            {
                Page = 1,
                Limit = 10000,
                Ids = ids,
                AreaId = areaId,
                Tags = tags,
                Keyword = keyword,
                HouseholdId = householdId,
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
            var res = await this.service.GetPopulationList(body);
            Dictionary<string, string> columns = new Dictionary<string, string>();

            foreach (var item in res)
            {
                item.HeadImageUrl = "";
                foreach (var tag in item.TagNames)
                {
                    item.HeadImageUrl += tag.Name + ",";
                }
                item.HeadImageUrl = item.HeadImageUrl.TrimEnd(',');
            }


            // columns.Add("Id", "Id");
            columns.Add("RealName", "姓名");
            columns.Add("Sex", "性别");
            columns.Add("Nation", "民族");
            columns.Add("IdCard", "公民身份证号码");
            columns.Add("Mobile", "联系方式");
            columns.Add("Education", "学历");
            columns.Add("RegisterAddress", "户籍地");
            columns.Add("LiveAddress", "现住地");
            columns.Add("HeadImageUrl", "人口属性");

            //string[] headings = { "社区/村户籍人口信息导入模板", 
            //    "备注： 1、列表标“ *”的为必填项，红色字体为下拉列表选项，列表其他项为文本输入；联系方式为11位手机号码；",
            //    "2、列表中“公民身份号码”输入后获取“出生日期”方法，可以拖拽或双击出生日期D6单元格右下角来填充其他单元格；出生日期格式yyyy-mm-dd，如：1999-01-01；",
            //    "3、为确定数据的有效性，请勿改动列表项及样式；" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(res.ToList(), null, columns, false);
            return File(filecontent, ExcelExportHelper.ExcelContentType, $"户籍人口信息{DateTime.Now.ToString("yyyyMMdd")}.xlsx");
        }

        /// <summary>
        /// 根据户码，获取户主信息
        /// </summary>
        /// <param name="householdId">户码</param>
        /// <param name="isConvert">是否转换字典字段</param>
        /// <returns></returns>
        [HttpGet("GetHouseholdManDetail")]
        [PermissionFilter(ModuleCodes.户籍人口, PermissionCodes.View)]
        public async Task<PopulationDetailDto> GetHouseholdManDetail(int householdId, int isConvert = 0)
        {
            var data = await this.service.GetHouseholdManDetail(householdId, isConvert);
            return data;
        }

        /// <summary>
        /// 获取简约版家庭成员信息
        /// </summary>
        /// <param name="householdId"></param>
        /// <returns></returns>
        [HttpGet("GetMembersSimple")]
        public async Task<List<KeyValueDto>> GetMembersSimple(int householdId) {
            var data = await this.service.GetMembersSimple(householdId);
            return data;
        }

        /// <summary>
        /// 根据户籍id查询户码列表
        /// </summary>
        /// <param name="populationId"></param>
        /// <returns></returns>
        [HttpGet("GetHouseholdList")]
        [PermissionFilter(ModuleCodes.户码管理,PermissionCodes.List)]
        public async Task<List<VillageHouseholdCode>> GetHouseholdList(int populationId)
        {
            var data = await this.service.GetHouseholdList(populationId);
            return data;
        }
    }
}
