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
using DVS.Common.Models;
using DVS.Models.Dtos.Village.Household;
using DVS.Models.Dtos.Village.Query;
using System.IO;
using DVS.Model.Dtos.Village;
using DVS.Models.Dtos.Village;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using DVS.Common;
using System.Reflection;
using DVS.Models.Dtos.Common;
using DVS.Common.Infrastructures;
using DVS.Models.Enum;
using System.Text.Json;
using DVS.Application.Services.Common;

namespace DVS.Village.Api.Controllers
{
    /// <summary>
    /// 户码管理
    /// </summary>
    [Route("/api/village/console")]
    public class HouseholdCodeController : DvsControllerBase
    {
        //用来获取路径相关
        private IWebHostEnvironment _webHostEnvironment;
        private IHouseholdCodeService service;

        private readonly IBasicDictionaryService basicDictionaryService;

        public HouseholdCodeController(IWebHostEnvironment webHostEnvironment, IHouseholdCodeService service, IBasicDictionaryService basicDictionaryService)
        {

            _webHostEnvironment = webHostEnvironment;
            this.service = service;
            this.basicDictionaryService = basicDictionaryService;
        }
        /// <summary>
        /// 获取户码列表
        /// </summary>
        /// <returns></returns>
        // [AllowAnonymous]
        [HttpPost("GetHouseholdCodeList")]
        [PermissionFilter(ModuleCodes.户码管理, PermissionCodes.List)]
        public async Task<IPagedList<HouseholdCodeDto>> GetHouseholdCodeList([FromBody] PostBody body) //  [FromBody]PostBody body  dynamic body
        {
            // var data = service.GetQueryable().FirstOrDefault(a => a.Id == 1);
            var data = await this.service.GetHouseholdCodeList(body, true);
            return data;
            // return new DvsResult(new { Name = "hello wo" });
        }

        /// <summary>
        /// 获取户码详情
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetHouseholdCodeDetail")]
        [PermissionFilter(ModuleCodes.户码管理, PermissionCodes.View)]
        public async Task<VillageHouseholdCode> GetHouseholdCodeDetail(int id)
        {
            var data = await this.service.GetHouseholdCodeDetail(id);
            return data;
        }
        /// <summary>
        /// 获取户码的模板的示意图
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost("householdcode/template/GetTemplateImage")]
        public async Task<string> GetTemplateImage([FromBody] VillageHouseholdCodeTemplate template)
        {
            template.Id = 0;
            string res = await service.GetTemplateImage(template);
            return res;
        }

        /// <summary>
        /// 增加户码
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        // [AllowAnonymous]
        [HttpPost("AddHouseholdCode")]
        [PermissionFilter(ModuleCodes.户码管理, PermissionCodes.Add)]
        public async Task<bool> AddHouseholdCode(HouseholdCodeBody body)
        {
            var post = new VillageHouseholdCode()
            {
                Id = 0,
                AreaId = body.AreaId,
                HouseNameId = body.HouseNameId,
                HouseNumber = body.HouseNumber,
                Tags = body.Tags,
                Status = body.Status,
                Remark = body.Remark,
                CreatedBy = LoginUser.UserId,
                UpdatedBy = LoginUser.UserId,
            };
            var result = await this.service.SaveHouseholdCode(post,LoginUser);

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
        /// 编辑户码
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        // [AllowAnonymous]
        [HttpPost("EditHouseholdCode")]
        [PermissionFilter(ModuleCodes.户码管理, PermissionCodes.Edit)]
        public async Task<bool> EditHouseholdCode(HouseholdCodeBody body)
        {
            if (body.Id <= 0)
            {
                throw new ValidException("请输入Id");
            }

            var post = new VillageHouseholdCode()
            {
                Id = body.Id,
                AreaId = body.AreaId,
                HouseNameId = body.HouseNameId,
                HouseNumber = body.HouseNumber,
                Tags = body.Tags,
                Status = body.Status,
                Remark = body.Remark,
                UpdatedBy = LoginUser.UserId,
            };
            var result = await this.service.SaveHouseholdCode(post,LoginUser);

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
        /// 请求下载户码
        /// </summary>
        /// <param name="req">区域编码</param>
        /// <returns></returns>
        [HttpPost("downloadHouseholdCode")]
        public async Task<DownloadHouseholdCodeResult> DownloadHouseholdCode([FromBody] DownloadHouseholdCodeRequest req)
        {

            int taskId = await this.service.CreateHouseholdCodeTask(req.AreaCode, req.Ids, req.TemplateId,req.TaskType);
            return new DownloadHouseholdCodeResult { TaskId = taskId };
        }
        /// <summary>
        /// 下载户码压缩文件
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpGet("downloadHouseholdCodeZipfile")]
        public async Task<FileResult> DownloadHouseholdCodeZipfile(int taskId)
        {

            Stream stream = await this.service.DownloadHouseholdCodeZiFile(taskId);
            // 创建文件删除任务
            return File(stream, "application/zip", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}-{taskId}.zip");
        }
        /// <summary>
        /// 查看户码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        [HttpGet("viewHouseholdCode")]
        public async Task<string> ViewHouseholdCode([FromQuery]int id, [FromQuery]int templateId)
        {
            string codeData = await this.service.GetHouseholdCodeImage(id, templateId);
            return codeData;
        }

        /// <summary>
        /// 查看户码的二维码
        /// </summary>
        /// <param name="id">户码的id</param>
        /// <returns></returns>
        [HttpGet("ViewSingleHouseholdQRCode")]
        public async Task<string> ViewSingleHouseholdQRCode([FromQuery] int id)
        {
            string codeData = await this.service.GetSingleQrCodeImage(id);
            return codeData;
        }

        /// <summary>
        /// 查询户码生产状态
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("getHouseholdCodeGenStatus")]
        public async Task<GetHouseholdCodeGenStatusResult> GetHouseholdCodeGenStatus([FromBody] GetHouseholdCodeGenStatusRequest req)
        {
            int status = await this.service.GetHouseholdCodeGenStatus(req.TaskId);
            return new GetHouseholdCodeGenStatusResult() { Status = status };
        }
        /// <summary>
        /// 获取需要生成户码的任务
        /// </summary>
        /// <returns></returns>
        [HttpPost("getHouseholdGenTask")]
        [AllowAnonymous]
        public async Task<VillageHouseholdCodeGrenTask> GetHouseholdGenTaskAsync()
        {
            return await this.service.GetHouseholdGenTaskAsync();
        }

        /// <summary>
        /// 获取需要生成户码的任务
        /// </summary>
        /// <param name="completedInfo"></param>
        /// <returns></returns>
        [HttpPost("notifyHouseholdQRCodeCompleted")]
        [AllowAnonymous]
        public async Task<int> HouseholdQRCodeCompleted([FromBody] HouseholdCodeCompletedReq completedInfo)
        {
            return await this.service.HouseholeCodeGenCompleted(completedInfo);
        }

        /// <summary>
        /// 下载单个户码
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("downloadSingleHouseholdCodeSync")]
        public async Task<FileResult> DownloadSingleHouseholdCodeSync(int id, int templateId)
        {
            Stream stream = await this.service.SingleHouseholdCode(id, templateId);
            return File(stream, "application/vnd.openxmlformats", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}-{id}.jpg");
        }


        /// <summary>
        /// 下载户码的二维码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("downloadSingleQrCodeSync")]
        public async Task<FileResult> DownloadSingleQrCodeSync(int id)
        {
            Stream stream = await this.service.SingleQrCode(id);
            return File(stream, "application/vnd.openxmlformats", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}-{id}.jpg");
        }


        [HttpPost("changeStatus")]
        [PermissionFilter(ModuleCodes.户码管理, PermissionCodes.Edit)]
        public async Task<bool> ChangeStatus(ChangeStatusReq req)
        {
            bool result = await this.service.ChangeStatus(req.Id, req.Status);
            return result;
        }



        /// <summary>
        /// excel导入户码数据
        /// </summary>
        /// <param name="excelFile">导入文件名</param>
        /// <param name="areaId">区域id</param>
        /// <returns></returns>
        [HttpPost("ImportHouseHoldCode")]
        [PermissionFilter(ModuleCodes.户码管理, PermissionCodes.Import)]
        public async Task<ImportResultDto> ImportHouseHoldCode(IFormFile excelFile, int areaId)
        {
            if (areaId <= 0)
            {
                throw new ValidException("缺少必要参数");
            }
            Stream fileStream = excelFile.OpenReadStream();
            var res = await this.service.ImportHouseHoldCode(fileStream, areaId,LoginUser);

            if (!res.Flag) {
                throw new ValidException(res.Message);

            }
            return res.Data;
        }

        /// <summary>
        /// excel导出户籍人口
        /// </summary>
        /// <param name="ids">例如1,2,3,4</param>
        /// <param name="areaId"></param>
        /// <param name="tags"></param>
        /// <param name="keyword"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        [HttpGet("ExportHouseHoldCode")]
        [PermissionFilter(ModuleCodes.户码管理, PermissionCodes.Export)]
        public async Task<FileContentResult> ExportHouseHoldCode(string ids="", int areaId = 0, string tags = "", string keyword = "", string orders = "")
        // public async Task<FileContentResult> ExportHouseHoldCode([System.Web.Http.FromUri] PostBody postBody)
        {
            //if (string.IsNullOrWhiteSpace(ids))
            //{

            //    throw new ValidException("缺少必要参数");
            //}
            var idsArr = ids.Split(',');
            List<int> idList = new List<int>();
            foreach (var id in idsArr)
            {

                int temp;
                if (int.TryParse(id, out temp))
                {
                    idList.Add(temp);
                }

            }

            var postBody = new PostBody()
            {

                AreaId = areaId,
                Page = 1,
                Limit = 10000,
                Ids = idList,
                Tags = tags,
                Keyword = keyword,
            };
            if (!string.IsNullOrWhiteSpace(orders))
            {
                try
                {
                    List<OrderBy> orderList = JsonSerializer.Deserialize<List<OrderBy>>(orders, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    if (orderList != null)
                    {
                        postBody.Orders = orderList;
                    }
                }
                catch { }
            }
            var res = await this.service.GetHouseholdCodeList(postBody, true);
            foreach (var item in res)
            {
                item.Relationship = "";
                foreach (var tag in item.TagNames)
                {
                    item.Relationship += tag.Name + ",";
                }
                item.Relationship = item.Relationship.TrimEnd(',');
                item.Remark = item.Status == 1 ? "正常" : "禁用";

            }


            Dictionary<string, string> columns = new Dictionary<string, string>();
            columns.Add("AreaName", "村/社区");
            columns.Add("HouseName", "门牌名");
            columns.Add("HouseNumber", "门牌号");
            columns.Add("HouseholdMan", "户主");
            columns.Add("PeopleCount", "人员数量");
            columns.Add("Relationship", "门牌标签"); // 借用Relationship 字段
            columns.Add("Remark", "户码状态"); // 借用Remark 字段

            //string[] headings = { "社区/村户户码信息导入模板",
            //    "备注： 1、列表标“ *”的为必填项，列表其他项为文本输入；",
            //    "2、列表中“门牌名”请输入楼栋单元、小组或道路名称，最多8个字；“门牌名”值范围0000～9999；",
            //    "3、为确定数据的有效性，请勿改动列表项及样式；" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(res.ToList(), null, columns, false);
            return File(filecontent, ExcelExportHelper.ExcelContentType, $"户码信息{DateTime.Now.ToString("yyyyMMdd")}.xlsx");
        }



        /// <summary>
        /// 获取门牌名列表
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("GetHouseNamePageLsit")]
        [PermissionFilter(ModuleCodes.门牌名管理, PermissionCodes.List)]
        public async Task<IPagedList<VillageHouseName>> GetHouseNamePageLsit(PageHouseNameBody body)
        {
            var data = await service.GetHouseNamePageLsit(body);
            return data;
        }

        /// <summary>
        /// 添加门牌名
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("AddHouseName")]
        [PermissionFilter(ModuleCodes.门牌名管理, PermissionCodes.Add)]
        public async Task<int> AddHouseName(VillageHouseNameDto body)
        {
            body.Id = 0;
            var data = Mapper.Map<VillageHouseName>(body);
            data.CreatedBy = LoginUser.UserId;
            data.UpdatedBy = LoginUser.UserId;
            var res = await service.SaveHouseName(data);
            if (!res.Flag)
            {
                throw new ValidException(res.Message);
            }
            return res.Data;
        }

        /// <summary>
        /// 编辑门牌名
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("EditHouseName")]
        [PermissionFilter(ModuleCodes.门牌名管理, PermissionCodes.Edit)]
        public async Task<int> EditHouseName(VillageHouseNameDto body)
        {
            if (body.Id <= 0)
            {
                throw new ValidException("缺少Id");
            }

            var data = Mapper.Map<VillageHouseName>(body);
            // data.CreatedBy = LoginUser.UserId;
            data.UpdatedBy = LoginUser.UserId;
            var res = await service.SaveHouseName(data);
            if (!res.Flag)
            {
                throw new ValidException(res.Message);
            }
            return res.Data;
        }

        /// <summary>
        /// 获取门牌名详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetHouseNameDetail")]
        public async Task<VillageHouseNameDto> GetHouseNameDetail(int id)
        {
            var res = await this.service.GetHouseNameDetail(id);
            return res;
        }


        /// <summary>
        /// 删除门牌名
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("DeleteHouseName")]
        [PermissionFilter(ModuleCodes.门牌名管理, PermissionCodes.Remove)]
        public async Task<bool> DeleteHouseName([FromBody] IdRequest req)
        {

            var res = await this.service.DeleteHouseName(req.Id, LoginUser.UserId);
            return res;
        }

        [HttpPost("householdcode/template/remove")]
        [PermissionFilter(ModuleCodes.门牌模板管理, PermissionCodes.Remove)]
        public async Task<bool> RemoveTemplate([FromServices] IHouseholdCodeService service, [FromBody] IdRequest req)
        {
            bool result = await service.RemoveTemplate(req.Id);
            return result;
        }

        [HttpPost("householdcode/template/add")]
        [PermissionFilter(ModuleCodes.门牌模板管理, PermissionCodes.Add)]
        public async Task<int> AddTemplate([FromServices] IHouseholdCodeService service, [FromBody] VillageHouseholdCodeTemplateDto template)
        {
            if (template.Id > 0)
            {
                throw new ValidException("无效的参数{id}");
            }
            var id = await service.SaveTemplate(template);
            return id;
        }

        [HttpPost("householdcode/template/edit")]
        [PermissionFilter(ModuleCodes.门牌模板管理, PermissionCodes.Edit)]
        public async Task<int> ModifyTemplate([FromServices] IHouseholdCodeService service, [FromBody] VillageHouseholdCodeTemplateDto template)
        {
            if (template.Id <= 0)
            {
                throw new ValidException("无效的参数{id}");
            }
            var id = await service.SaveTemplate(template);
            return id;
        }

        [HttpPost("householdcode/template/list")]
        public async Task<List<VillageHouseholdCodeTemplateDto>> TemplateList([FromServices] IHouseholdCodeService service, [FromBody] TemplateListReq req)
        {
            if (req.AreaId <= 0)
            {
                throw new ValidException("无效的参数{areaId}");
            }
            var templates = await service.TemplateList(req.AreaId);
            return templates;
        }

        [HttpPost("householdcode/template/detail")]
        public async Task<VillageHouseholdCodeTemplateDto> TemplateDetail([FromServices] IHouseholdCodeService service, [FromBody] IdRequest req)
        {
            if (req.Id <= 0)
            {
                throw new ValidException("无效的参数{areaId}");
            }
            var template = await service.TemplateDetail(req.Id);
            return template;
        }

        /// <summary>
        /// 村门牌名列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetHouseNameList")]

        public async Task<List<HouseNameDto>> GetHouseNameList(int areaId)
        {
            if (areaId <= 0)
            {
                areaId = LoginUser.AreaId;
            }
            var data = await this.service.GetHouseNameList(areaId);
            return data;
        }

        /// <summary>
        /// 村门牌号列表
        /// </summary>
        /// <param name="areaId">行政区域Id</param>
        /// <param name="houseNameId">门牌名</param>
        /// <returns></returns>
        [HttpGet("GetHouseNumberList")]
        public async Task<IEnumerable<HouseNumberDto>> GetHouseNumberList(int areaId, int houseNameId)
        {
            var data = await this.service.GetHouseNumberList(areaId, houseNameId);
            return data;
        }


        /// <summary>
        /// 门牌标签管理列表
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        // [AllowAnonymous]
        [HttpPost("GetDictionaryHouseTagPageListAsync")]
        public async Task<IPagedList<BasicDictionaryHouseTagDto>> GetDictionaryHouseTagPageListAsync(HouseTagQueryModel body)
        {
            var data = await this.basicDictionaryService.GetDictionaryHouseTagPageListAsync(body);
            return data;
        }

        /// <summary>
        /// 保存门牌标签
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("SaveDictionaryHouseTagAsync")]
        public async Task<bool> SaveDictionaryHouseTagAsync(BasicDictionaryHouseTagBody body)
        {
            int updatedBy = LoginUser.UserId;
            var res = await this.basicDictionaryService.SaveDictionaryHouseTagAsync(body, updatedBy);
            return res;
        }
    }
}
