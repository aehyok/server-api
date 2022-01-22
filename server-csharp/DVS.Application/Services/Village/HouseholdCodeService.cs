using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Http;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Common;
using DVS.Model.Dtos.Village;
using DVS.Models.Dtos.Village;
using DVS.Models.Dtos.Village.Household;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village.Statistics;
using DVS.Models.Enum;
using LinqKit;
using Lychee.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using X.PagedList;
using DVS.Common.SO;
using DVS.Common;
using System.Data;
using DVS.Models.Dtos.Village.Export;
using DVS.Common.RPC;
using DVS.Models.Dtos.RPC;

namespace DVS.Application.Services.Village
{
    public class HouseholdCodeService : ServiceBase<VillageHouseholdCode>, IHouseholdCodeService
    {
        private readonly IBasicAreaService basicAreaService;
        private readonly IPopulationService populationService;
        private readonly IServiceBase<VillageHouseholdCodeGrenTask> genTaskSerivce;
        private readonly IHouseholdCodeTagService _householdCodeTagService;
        private readonly IBasicDictionaryService _basicDictionaryService;
        private readonly ISunFileInfoService sunFileInfoService;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory factory;
        private readonly int isDeleted = 0;
        private readonly int isHouseholder = 1;
        private readonly IPopulationTagService populationTagService;
        private readonly IServiceBase<VillageHouseName> houseNameService;
        private readonly IServiceBase<VillageHouseholdCodeTemplate> templateSerivce;
        private readonly IServiceBase<VillageHouseCodeMember> memberService;
        public HouseholdCodeService(DbContext dbContext, IMapper mapper, IBasicAreaService basicAreaService,
            IPopulationService populationService,
            IServiceBase<VillageHouseholdCodeGrenTask> genTaskSerivce,
            IServiceBase<VillageHouseholdCodeTemplate> templateSerivce,
            IHouseholdCodeTagService householdCodeTagService,
            ISunFileInfoService sunFileInfoService,
            IBasicDictionaryService basicDictionaryService,
            IPopulationTagService populationTagService,
            IServiceBase<VillageHouseName> houseNameService,
            IConfiguration configuration,
            IServiceBase<VillageHouseCodeMember> memberService,
            IHttpClientFactory factory)
            : base(dbContext, mapper)
        {
            this.factory = factory;
            this.basicAreaService = basicAreaService;
            this.populationService = populationService;
            this.genTaskSerivce = genTaskSerivce;
            this.configuration = configuration;
            this._householdCodeTagService = householdCodeTagService;
            this._basicDictionaryService = basicDictionaryService;
            this.sunFileInfoService = sunFileInfoService;
            this.populationTagService = populationTagService;
            this.houseNameService = houseNameService;
            this.templateSerivce = templateSerivce;
            this.memberService = memberService;

        }

        public async Task<int> GetHouseholdCodeGenStatus(int taskId)
        {
            VillageHouseholdCodeGrenTask task = await genTaskSerivce.GetQueryable().FirstOrDefaultAsync(task => task.Id == taskId);
            if (task == null)
            {
                throw new ValidException("任务不存在");
            }
            return task.Status;
        }

        public async Task<VillageHouseholdCode> GetHouseholdCodeDetail(int id)
        {
            //var data = await this.GetAsync(a => a.Id == id && a.IsDeleted == isDeleted);

            //if (data != null)
            //{
            //    var area = await this.basicAreaService.GetAsync(a => a.Id == data.AreaId && a.IsDeleted == isDeleted);
            //    data.AreaName = area != null ? area.Name : "";

            //    var popu = await this.populationService.GetAsync(a => a.HouseholdId == data.Id && a.IsDeleted == isDeleted&&a.Relationship== householdMan);
            //    data.HouseholdMan = popu != null ? popu.RealName : "";
            //}

            var data = from h in this.GetQueryable()
                       join m in this.memberService.GetQueryable()
                       on new { h.Id, IsHouseholder = 1, IsDeleted = 0 } equals new { Id = m.HouseholdId, m.IsHouseholder, m.IsDeleted } into mtb
                       from mm in mtb.DefaultIfEmpty()
                       join p in this.populationService.GetQueryable()
                       on mm.PopulationId equals p.Id into temp
                       from pp in temp.DefaultIfEmpty()
                       where h.IsDeleted == 0 && h.Id == id
                       select new VillageHouseholdCode()
                       {
                           AreaId = h.AreaId,
                           HouseName = h.HouseName,
                           HouseNumber = h.HouseNumber,
                           Id = h.Id,
                           PeopleCount = h.PeopleCount,
                           CreatedAt = h.CreatedAt,
                           CreatedBy = h.CreatedBy,
                           ImageIds = h.ImageIds,
                           ImageUrls = h.ImageUrls,
                           IsDeleted = h.IsDeleted,
                           Remark = h.Remark,
                           Status = h.Status,
                           Tags = h.Tags,
                           UpdatedAt = h.UpdatedAt,
                           UpdatedBy = h.UpdatedBy,
                           AreaName = "",
                           HouseholdMan = (pp == null ? "" : pp.RealName),
                           Mobile = pp.Mobile,
                           HeadImageUrl = (pp == null ? "" : pp.HeadImageUrl),
                           HouseNameId = h.HouseNameId,
                           // TagNames = "",
                       };

            var result = data.FirstOrDefault();

            if (result != null)
            {
                var area = await this.basicAreaService.GetAsync(a => a.Id == result.AreaId && a.IsDeleted == isDeleted);
                result.AreaName = area != null ? area.Name : "";

                result.TagNames = await this._householdCodeTagService.GetTags(id);
                result.Tags = string.Join(",", result.TagNames.Select(a => a.Id));
                result.ImageUrls = this.sunFileInfoService.ToAbsolutePath(result.ImageUrls);

                result.HouseholdMan = BasicSO.Decrypt(result.HouseholdMan);
                result.HeadImageUrl = this.sunFileInfoService.ToAbsolutePath(result.HeadImageUrl);
                result.Mobile = BasicSO.Decrypt(result.Mobile);
            }

            return result;
        }

        public async Task<IPagedList<HouseholdCodeDto>> GetHouseholdCodeList(PostBody body, bool isGetTag = false)
        {
            string sql = string.Format(@"SELECT 
                                            h.id,
                                            h.houseName,
                                            h.houseNumber,
                                            h.status,
                                            h.Remark,
                                            IFNULL(h.houseNameSequence,0) as houseNameSequence,
                                            p.realName as householdMan,
                                            p.mobile,
                                            p.headImageUrl,
                                            ifnull(p.sex,0) as sex,
                                            ifnull(p.id,0) as populationId,
                                            p.relationship,
                                            ifnull(m.isHouseholder,0) as isHouseholder,
                                            a.`name` as areaName,
                                            ifnull(t1.peopleCount,0) as peopleCount,
                                            h.UpdatedAt
                                            from VillageHouseholdCode h
                                            LEFT JOIN VillageHouseCodeMember m on h.id=m.householdId and m.isHouseholder=1 and m.isDeleted=0
                                            LEFT JOIN VillagePopulation p on m.populationId= p.id and p.isDeleted=0
                                            LEFT JOIN BasicArea a on a.id=h.areaId
                                            LEFT JOIN (
                                            SELECT n.householdId,COUNT(n.id) as peopleCount FROM VillageHouseCodeMember n 
                                            WHERE n.isDeleted=0 and n.householdId>0 GROUP BY n.householdId
                                            )t1 on h.id = t1.householdId
                                            WHERE h.isDeleted=0 and h.areaId={0} ", body.AreaId);

            string sqlCount = string.Format(@"SELECT 
                                                h.id
                                                from VillageHouseholdCode h
                                                LEFT JOIN VillageHouseCodeMember m on h.id=m.householdId and m.isHouseholder=1  and m.isDeleted=0
                                                LEFT JOIN VillagePopulation p on m.populationId= p.id and p.isDeleted=0
                                                WHERE h.isDeleted=0  and h.areaId={0} ", body.AreaId);
            if (!string.IsNullOrWhiteSpace(body.HouseName))
            {

                sql += $" and h.houseName='{body.HouseName}'";
                sqlCount += $" and h.houseName='{body.HouseName}'";
            }

            if (body.Status >= 0)
            {
                sql += " and h.Status=" + body.Status;
                sqlCount += " and h.Status=" + body.Status;
            }

            if (!string.IsNullOrWhiteSpace(body.Tags))
            {

                sql += " and concat(',',h.Tags,',') like '%," + body.Tags + ",%'";
                sqlCount += " and concat(',',h.Tags,',') like '%," + body.Tags + ",%'";
            }

            if (!string.IsNullOrWhiteSpace(body.Keyword))
            {
                body.Keyword = body.Keyword.Replace("'", "").Replace("\"", "").Replace("-", "");
                string like = string.Format(" and (h.houseName LIKE '%{0}%' or h.houseNumber LIKE '%{0}%' or p.realName like '%{0}%')", body.Keyword);
                sql += like;
                sqlCount += like;
            }

            if (body.Ids != null && body.Ids.Count > 0)
            {
                sql += " and h.id in (" + string.Join(",", body.Ids) + ")";
                sqlCount += " and h.id in (" + string.Join(",", body.Ids) + ")";
            }
            Console.WriteLine(sql);
            // sql += " order by h.id desc";
            // Console.WriteLine("------00000000000000000-----------------------------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            var data = this.Context.Database.SqlQueryPagedList<HouseholdCodeDto>(body.Page, body.Limit, sql, sqlCount, this.Context.Database.GetOrderBySql(body.Orders));

            if (isGetTag)
            {
                List<int> ids = new List<int>();
                foreach (var item in data)
                {
                    ids.Add(item.Id);
                    // item.TagNames = await this._householdCodeTagService.GetTags(item.Id);
                }

                var tagNames = await this._householdCodeTagService.GetTags(ids);

                foreach (var item in data)
                {
                    item.TagNames = tagNames.Where(a => a.Pid == item.Id).ToList();
                    item.HouseholdMan = BasicSO.Decrypt(item.HouseholdMan);
                    item.Mobile = BasicSO.Decrypt(item.Mobile);
                }

            }
            else
            {
                List<string> dirs = new List<string>();
                foreach (var item in data)
                {
                    dirs.Add(item.Relationship);
                }
                var dirNames = await this._basicDictionaryService.GetBasicDictionaryCodeList(dirs);
                foreach (var item in data)
                {
                    item.Relationship = await this._basicDictionaryService.GetNameByCode(item.Relationship, dirNames);
                    item.HouseholdMan = BasicSO.Decrypt(item.HouseholdMan);
                    item.Mobile = BasicSO.Decrypt(item.Mobile);
                }
            }

            return data;
        }

        public async Task<MessageResult<bool>> SaveHouseholdCode(VillageHouseholdCode householdCode,LoginUser loginUser)
        {
            var result = new MessageResult<bool>("失败了", false, false);
            if (householdCode.AreaId <= 0)
            {
                result.Message = "缺少行政区域Id";
                return result;
            }

            if (householdCode.HouseNameId <= 0)
            {
                result.Message = "请选择门牌名";
                return result;
            }
            if (string.IsNullOrEmpty(householdCode.HouseNumber))
            {
                result.Message = "请输入门牌号";
                return result;
            }

            var houseNameData = await this.houseNameService.GetAsync(a => a.Id == householdCode.HouseNameId && a.IsDeleted == isDeleted);
            if (houseNameData == null)
            {
                if (!string.IsNullOrWhiteSpace(householdCode.HouseName))
                {

                    houseNameData = await this.houseNameService.GetAsync(a => a.HouseName == householdCode.HouseName && a.AreaId == householdCode.AreaId && a.IsDeleted == isDeleted);
                    if (houseNameData == null)
                    {
                        result.Message = "不存在此门牌名";
                        return result;
                    }
                }
                else
                {
                    result.Message = "不存在此门牌名";
                    return result;
                }
            }
            householdCode.HouseNameId = houseNameData.Id;
            householdCode.HouseName = houseNameData.HouseName;
            householdCode.HouseNameSequence = houseNameData.Sequence;



            var household = await this.GetAsync(a => a.AreaId == householdCode.AreaId && a.HouseNameId == householdCode.HouseNameId && a.HouseNumber == householdCode.HouseNumber && a.IsDeleted == isDeleted);
            if (household != null && householdCode.Id != household.Id)
            {
                result.Message = "此门牌名下已经有相同的门牌号，请使用其他门牌号";
                return result;
            }



            if (householdCode.Id > 0)
            {
                var res = await this.GetQueryable().Where(a => a.Id == householdCode.Id && a.AreaId == householdCode.AreaId)
                    .UpdateFromQueryAsync(a => new VillageHouseholdCode()
                    {
                        HouseName = householdCode.HouseName,
                        HouseNumber = householdCode.HouseNumber,
                        Tags = householdCode.Tags,
                        Status = householdCode.Status,
                        Remark = householdCode.Remark,
                        HouseNameId = householdCode.HouseNameId,
                        HouseNameSequence = householdCode.HouseNameSequence,
                        UpdatedBy=loginUser.UserId,
                        IsSync = 0,
                    });
                if (res > 0)
                {

                    await this._householdCodeTagService.SaveTags(householdCode.Id, householdCode.Tags,loginUser.UserId);
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = true;
                    return result;
                }
            }
            else
            {
                householdCode.CreatedBy = loginUser.UserId;
                var res = await this.InsertAsync(householdCode);

                if (res != null)
                {
                    BasicRPC.AllotScore(new IntegralReq()
                    {
                        IntegralAction = IntegralAction.CreateHousehold,
                        Description = "创建户码",
                        HouseholdId = res.Id,
                        UserId = loginUser.UserId
                    }) ;
                    await this._householdCodeTagService.SaveTags(res.Id, householdCode.Tags,loginUser.UserId);
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = true;
                    return result;
                }
            }

            return result;
        }

        public async Task<Stream> SingleQrCode(int id)
        {
            if (id == 0)
            {
                throw new Exception("请选择一个户码");
            }
            string qrCodeStringData = await getSingleQrCodeData(id);
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(qrCodeStringData));
            return stream;
        }

        private async Task<string> getSingleQrCodeData(int id)
        {
            VillageHouseholdCode code = await this.GetQueryable().Where(code => code.Id == id).FirstOrDefaultAsync();
            if (code == null) {
                throw new Exception("户不存在");
            }
            BasicArea basicArea = await basicAreaService.GetQueryable().Where(area => area.Id == code.AreaId).FirstOrDefaultAsync();
            if (basicArea==null) {
                throw new Exception("户不属于任何的区域");
            }
            VillagePopulation population = await populationService.GetHouseholderInfo(code.Id);
            HttpResponseMessage message = await factory.PostAsync(configuration["HouseholdCodeService:Url"] + "/api/qrcode/getqrCode", new HouseholdCodeTaskContent { AreaName = basicArea.Name, HouseholderName = population?.RealName, AreaCode = basicArea.AreaCode, HouseName = code.HouseName, HouseNumber = code.HouseNumber, Id = code.Id, Url = configuration["HouseholdCode:Url"] + "?id=" + code.Id + "&areaId=" + code.AreaId + "&action=1" });
            string qrCodeStringData = await message.Content.ReadAsStringAsync();
            return qrCodeStringData;
        }

        public async Task<string> GetSingleQrCodeImage(int id)
        {
            string qrCodeStringData = await getSingleQrCodeData(id);
            return "data:image/jpg;base64," + qrCodeStringData;
        }



        public async Task<string> GetHouseholdCodeImage(int id, int templateId)
        {
            string qrCodeStringData = await getHouseholdCodeBase64String(id, templateId);
            return "data:image/jpg;base64," + qrCodeStringData;
        }

        private async Task<string> getHouseholdCodeBase64String(int id, int templateId)
        {
            if (id == 0)
            {
                throw new Exception("请选择一个户码");
            }
            VillageHouseholdCode code = await this.GetQueryable().Where(code => code.Id == id).FirstOrDefaultAsync();
            BasicArea basicArea = await basicAreaService.GetQueryable().Where(area => area.Id == code.AreaId).FirstOrDefaultAsync();
            VillageHouseholdCodeTemplateDto templateDto = null;
            if (templateId > 0)
            {
                VillageHouseholdCodeTemplate template = await templateSerivce.GetQueryable().FirstAsync(t => t.Id == templateId);
                templateDto = mapper.Map<VillageHouseholdCodeTemplateDto>(template);
                SunFileInfo background = null;
                if (template != null)
                {
                    background = await sunFileInfoService.GetQueryable().FirstOrDefaultAsync(f => f.Id == template.Background);
                    templateDto.BackgroundInfo = mapper.Map<SunFileInfoDto>(background);
                }
            }
            HttpResponseMessage message = await factory.PostAsync(configuration["HouseholdCodeService:Url"] + "/api/qrcode/gethouseholdcode", new VillageHouseholdCodeGrenTask()
            {
                CodeData = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    AreaCode = basicArea.AreaCode,
                    houseName = code.HouseName,
                    houseNumber = code.HouseNumber,
                    Id = code.Id,
                    Url = configuration["HouseholdCode:Url"] + "?id=" + code.Id + "&areaId=" + code.AreaId + "&action=1",
                }),
                Template = templateDto,
                TaskType = 1
            });

            string qrCodeStringData = await message.Content.ReadAsStringAsync();
            return qrCodeStringData;
        }
        public async Task<Stream> SingleHouseholdCode(int id, int templateId)
        {
            string qrCodeStringData = await getHouseholdCodeBase64String(id, templateId);
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(qrCodeStringData));
            return stream;
        }

        public async Task<int> CreateHouseholdCodeTask(int areaCode, List<int> ids, int templateId, int taskType = 1)
        {
            if (ids == null || ids.Count == 0)
            {
                throw new ValidException("请选择要下载的户码");
            }
            BasicArea basicArea = await basicAreaService.GetQueryable().Where(area => area.Id == areaCode).FirstOrDefaultAsync();
            if (basicArea == null)
            {
                throw new ValidException($"没找到编码为{areaCode}行政区域");
            }

            var codes = await this.GetQueryable().Where(householdCode => ids.Contains(householdCode.Id)).Select(code => new HouseholdCodeTaskContent { HouseName = code.HouseName, AreaName = basicArea.Name, HouseholderName = "", HouseNumber = code.HouseNumber, AreaCode = basicArea.AreaCode, Id = code.Id, Url = configuration["HouseholdCode:Url"] + "?id=" + code.Id + "&areaId=" + code.AreaId + "&action=1" }).ToListAsync();
            if (codes == null)
            {
                throw new ValidException($"没找到户码信息，可能已经被删除");
            }
            foreach (var item in codes)
            {
                var population = await populationService.GetHouseholderInfo(item.Id);
                item.HouseholderName = population?.RealName;
            }
            VillageHouseholdCodeGrenTask task = await genTaskSerivce.InsertAsync(new VillageHouseholdCodeGrenTask() { Status = 0, CodeData = JsonSerializer.Serialize(codes), TemplateId = templateId, TaskType = taskType });

            return task.Id;
        }
        #region 跟QRCGenService交互

        public async Task<VillageHouseholdCodeGrenTask> GetHouseholdGenTaskAsync()
        {
            VillageHouseholdCodeGrenTask task = await genTaskSerivce.GetQueryable().Where(task => task.Status == 0).FirstOrDefaultAsync();
            if (task == null)
            {
                return null;
            }
            VillageHouseholdCodeTemplate template = await templateSerivce.GetQueryable().FirstOrDefaultAsync(a => a.Id == task.TemplateId);
            if (template != null)
            {
                VillageHouseholdCodeTemplateDto templateDto = mapper.Map<VillageHouseholdCodeTemplateDto>(template);
                SunFileInfo background = await sunFileInfoService.GetQueryable().FirstOrDefaultAsync(f => f.Id == templateDto.Background);
                templateDto.BackgroundInfo = mapper.Map<SunFileInfoDto>(background);
                task.Template = templateDto;
            }
            return task;
        }

        public async Task<int> HouseholeCodeGenCompleted(HouseholdCodeCompletedReq req)
        {
            VillageHouseholdCodeGrenTask task = await genTaskSerivce.GetAsync(req.TaskId);
            task.Status = (int)HouseholeCodeGentTaskStatus.Completed;
            task.ZipFileId = req.fileId;
            int res = await genTaskSerivce.UpdateAsync(task);
            return res;
        }
        #endregion 跟QRCGenService交互


        public async Task<Stream> DownloadHouseholdCodeZiFile(int taskId)
        {
            VillageHouseholdCodeGrenTask task = await genTaskSerivce.GetQueryable().Where(t => t.Id == taskId && t.Status == (int)HouseholeCodeGentTaskStatus.Completed).FirstOrDefaultAsync();
            if (task == null)
            {
                throw new ValidException("文件未生成，请稍后");
            }
            SunFileInfo sunFileInfo = await sunFileInfoService.GetQueryable().Where(f => f.Id == task.ZipFileId).FirstOrDefaultAsync();
            if (sunFileInfo != null)
            {
                SunFileInfoDto sunFileInfoDto = mapper.Map<SunFileInfoDto>(sunFileInfo);
                Stream stream = await factory.GetStreamAsync(sunFileInfoDto.Url);
                return stream;
            }
            else
            {
                throw new ValidException("找不到生成文件");
            }
        }
        public async Task<List<HouseNameDto>> GetHouseNameList(int areaId)
        {
            var data = from v in this.houseNameService.GetQueryable().Where(a => a.IsDeleted == 0 && a.AreaId == areaId)
                       orderby v.Sequence
                       select new HouseNameDto()
                       {
                           Name = v.HouseName,
                           Value = v.Id,
                       };
            return data.ToList();
        }

        public async Task<MessageResult<bool>> SetHouseholdImage(SetHouseholdImageBody body)
        {
            var result = new MessageResult<bool>("失败", false, false);

            if (body.HouseholdId <= 0)
            {
                result.Message = "缺少必要参数";
                return result;
            }

            if (string.IsNullOrWhiteSpace(body.ImageIds))
            {
                result.Message = "请传入图片Ids";
                return result;
            }
            body.ImageUrls = await this.sunFileInfoService.GetSunFileRelativeUrls(body.ImageIds);
            var res = await this.GetQueryable().Where(a => a.Id == body.HouseholdId).UpdateFromQueryAsync(a => new VillageHouseholdCode()
            {
                ImageIds = body.ImageIds,
                ImageUrls = body.ImageUrls,
                IsSync = 0,
            });
            if (res > 0)
            {
                result.Message = "成功";
                result.Flag = true;
                result.Data = true;
                return result;
            }
            return result;
        }

        public async Task<IEnumerable<StatisticsCommonDto>> GetVillageHonor(int areaId)
        {
            var areaIds = await basicAreaService.FindChildrenAreaIds(areaId);
            // 门牌标签 2010
            var query = from h in this.GetQueryable()
                        join t in this._householdCodeTagService.GetQueryable() on h.Id equals t.HouseholdId
                        join d in this._basicDictionaryService.GetQueryable() on t.TagId equals d.Code
                        where areaIds.Contains(h.AreaId) && h.IsDeleted == 0
                        group d by d.Name into g
                        select new StatisticsCommonDto
                        {
                            Name = g.Key,
                            Value = g.Count()
                        };

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<HouseNumberDto>> GetHouseNumberList(int areaId, int houseNameId)
        {
            if (areaId <= 0 || houseNameId <= 0)
            {
                return new List<HouseNumberDto>();
            }

            var sql = string.Format(@"SELECT `v`.`Id` AS `HouseholdId`, `v`.`HouseNumber`,`v`.`HouseName`,`v1`.`RealName` AS `HouseholdMan`
                        FROM `VillageHouseholdCode` AS `v`
                        LEFT JOIN `VillageHouseCodeMember` AS `v0` ON `v`.`Id` = `v0`.`HouseholdId` and `v0`.`IsHouseholder`=1 and `v0`.IsDeleted=0
                        LEFT JOIN `VillagePopulation` AS `v1` ON `v0`.`PopulationId` = `v1`.`Id`
                        WHERE 1=1   
                        and `v`.`IsDeleted`=0
						and `v`.`status`=1
                        and `v`.`HouseNameId` = {0}
                        and `v`.`AreaId` = {1} 
                        ORDER BY `v`.houseNumber ASC", houseNameId, areaId);
            var data = await this.Context.Database.SqlQueryAsync<HouseNumberDto>(sql);
            foreach (var item in data)
            {
                item.HouseholdMan = BasicSO.Decrypt(item.HouseholdMan);
            }
            return data;
        }


        public async Task<List<HouseNumberDto>> GetHouseNumberListByIdCard(int areaId, string idCard)
        {
            idCard = BasicSO.Encrypt(idCard);
            //var count = this.populationService.GetQueryable().Where(a => a.AreaId == areaId && a.IdCard == idCard && a.IsDeleted == isDeleted).Count();
            //if (count < 1)
            //{
            //    return null;
            //}
            var data = from p in this.populationService.GetQueryable()
                       join m in this.memberService.GetQueryable() on new { PopulationId = p.Id, IsDeleted = 0 } equals new { m.PopulationId, m.IsDeleted }
                       join h in this.GetQueryable() on new { Id = m.HouseholdId, IsDeleted = 0, Status = 1 } equals new { h.Id, h.IsDeleted, h.Status }
                       where p.AreaId == areaId && p.IdCard == idCard && p.IsDeleted == 0
                       orderby h.HouseNumber
                       // && (m.IsDeleted is null || m.IsDeleted==1)
                       select new HouseNumberDto()
                       {
                           HouseholdId = h.Id,
                           HouseNumber = h.HouseNumber,
                           HouseName = h.HouseName,
                       };
            //var data = from o in this.GetQueryable().Where(a => a.AreaId == areaId && a.HouseName == houseName && a.IsDeleted == isDeleted)
            //           select new HouseNumberDto()
            //           {
            //               HouseholdId = o.Id,
            //               HouseNumber = o.HouseNumber
            //           };
            return await data.ToListAsync();
        }


        public async Task<HouseholderAndHouseNumberDto> GethouseholderAndHouseNumber(int householdId)
        {
            var list = from h in this.GetQueryable()
                       join m in this.memberService.GetQueryable() on new { h.Id, IsHouseholder = 1, IsDeleted = 0 } equals new { Id = m.HouseholdId, m.IsHouseholder, m.IsDeleted } into mtemp
                       from mm in mtemp.DefaultIfEmpty()
                       join p in this.populationService.GetQueryable() on mm.PopulationId equals p.Id into temp
                       from pp in temp.DefaultIfEmpty()
                       where h.IsDeleted == 0 && h.Id == householdId
                       select new HouseholderAndHouseNumberDto()
                       {
                           HouseholdId = h.Id,
                           AreaId = h.AreaId,
                           HeadImageUrl = pp.HeadImageUrl,
                           HouseName = h.HouseName,
                           HouseNumber = h.HouseNumber,
                           ImageUrls = h.ImageUrls,
                           IsHouseholder = mm.IsHouseholder,
                           Mobile = pp.Mobile,
                           PopulationId = pp.Id,
                           RealName = pp.RealName,
                           Relationship = pp.Relationship,
                           Sex = pp.Sex,
                           IdCard = pp.IdCard,
                           IsDeleted = h.IsDeleted,
                           HouseNameId = h.HouseNameId
                           // Address = "",
                           // AreaName = "",
                           // HouseholdTagNames=[],
                           // PopulationTagNames=[],
                       };

            var data = await list.FirstOrDefaultAsync();
            if (data != null)
            {

                data.AreaName = await this.basicAreaService.GetAreaName(data.AreaId);
                data.ImageUrls = this.sunFileInfoService.ToAbsolutePath(data.ImageUrls);
                data.HeadImageUrl = this.sunFileInfoService.ToAbsolutePath(data.HeadImageUrl);
                data.HouseholdTagNames = await this._householdCodeTagService.GetTags(data.HouseholdId);
                data.PopulationTagNames = await this.populationTagService.GetTags(data.PopulationId);
                data.Address = this.basicAreaService.FindParentAreaString(data.AreaId, false);

                data.Mobile = BasicSO.Decrypt(data.Mobile);
                data.RealName = BasicSO.Decrypt(data.RealName);
                data.IdCard = BasicSO.Decrypt(data.IdCard);
            }
            return data;
        }

        /// <summary>
        /// 户码数和人数统计
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public async Task<HouseholdStatisticsDto> HouseholdStatistics(int areaId)
        {

            // 计算户数
            var household = from h in this.GetQueryable() where h.IsDeleted == 0 && h.AreaId == areaId select h;
            var householdCount = await household.CountAsync();

            // 计算人数
            var population = from p in this.populationService.GetQueryable() where p.IsDeleted == 0 && p.AreaId == areaId select p;
            var populationCount = await population.CountAsync();

            return new HouseholdStatisticsDto()
            {

                HouseholdCount = householdCount,
                PopulationCount = populationCount,
                AreaId = areaId,
            };

        }

        public async Task<bool> ChangeStatus(int id, int status)
        {
            int result = await this.GetQueryable().Where(h => h.Id == id).UpdateFromQueryAsync(a => new VillageHouseholdCode { Status = status, IsSync = 0 });
            return result > 0;
        }

        public async Task<MessageResult<ImportResultDto>> ImportHouseHoldCode(Stream fileStream, int areaId,LoginUser loginUser)
        {
            MessageResult<ImportResultDto> result = new MessageResult<ImportResultDto>();
            DataSet dataset = Utils.ImportExcel(fileStream, 5, 3);
            int cnt = dataset.Tables.Count;
            if (cnt == 0)
            {
                result.Message = "无任何数据可导入";
                return result;
            }
            // int successCount = 0;
            // int failCount = 0;
            ImportResultDto importResult = new ImportResultDto();
            importResult.Errors = new List<ImportResultMessage>();
            // Dictionary<long,string> errors = new Dictionary<long, string>();
            var areas = await this.houseNameService.GetListAsync(a => a.AreaId == areaId && a.IsDeleted == 0);
            int i = 5;
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                i += 1;
                string houseName = row.ItemArray[0].ToString().Trim();
                string houseNumber = row.ItemArray[1].ToString().Trim();
                string remark = row.ItemArray[2].ToString().Trim();

                if (string.IsNullOrWhiteSpace(houseName) && string.IsNullOrWhiteSpace(houseNumber))
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(houseName) || string.IsNullOrWhiteSpace(houseNumber))
                {
                    importResult.FailCount += 1;
                    importResult.Errors.Add(new ImportResultMessage() { Row = i, Description = "数据错误" });
                    continue;
                }
                var area = areas.FirstOrDefault(a => a.HouseName == houseName);
                var data = new VillageHouseholdCode()
                {

                    AreaId = areaId,
                    CreatedAt = DateTime.Now,
                    HouseName = houseName,
                    HouseNumber = houseNumber,
                    Remark = remark,
                    Status = 1,
                    IsDeleted = 0,
                    HouseNameId = area != null ? area.Id : 0
                };

                // 重复导入时，更新而不是新增重复的记录
                var housecode = await this.GetAsync(a => a.HouseName == data.HouseName && a.HouseNumber == data.HouseNumber && a.AreaId == data.AreaId && a.IsDeleted == 0);
                if (housecode != null)
                {
                    data.Id = housecode.Id;
                    data.Tags = housecode.Tags;
                    data.Status = housecode.Status;
                }
                var res = await this.SaveHouseholdCode(data,loginUser);
                if (res.Flag)
                {
                    importResult.SuccessCount += 1;
                }
                else
                {
                    importResult.Errors.Add(new ImportResultMessage() { Row = i, Description = "数据错误" });
                    importResult.FailCount += 1;
                }
            }

            result.Flag = true;
            result.Data = importResult;
            return result;
        }

        public async Task<List<ExportHouseHoldCodeDto>> ExportHouseHoldCode(string ids)
        {
            string sql = string.Format(@" SELECT h.houseName,h.houseNumber,h.remark FROM VillageHouseholdCode h  WHERE h.IsDeleted=0 and h.id in({0})", ids.Replace("-", "").Replace("delete", "").Replace("drop", "").Replace("update", ""));
            var data = this.Context.Database.SqlQuery<ExportHouseHoldCodeDto>(sql);
            return data;
        }



        /// <summary>
        /// 获取门牌名列表
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<IPagedList<VillageHouseName>> GetHouseNamePageLsit(PageHouseNameBody body)
        {

            Expression<Func<VillageHouseName, bool>> filter = PredicateBuilder.New<VillageHouseName>(true);
            filter = filter.And(x => x.AreaId == body.AreaId && x.IsDeleted == isDeleted);

            if (!string.IsNullOrWhiteSpace(body.Keyword))
            {
                filter = filter.And(x => x.HouseName.Contains(body.Keyword));
            }
            var data = await this.houseNameService.GetPagedListAsync(filter, new List<Expression<Func<VillageHouseName, int>>>() { a => a.Sequence, a => a.Id }, body.Page, body.Limit, true);
            return data;
        }

        /// <summary>
        /// 添加编辑门牌名
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>

        public async Task<MessageResult<int>> SaveHouseName(VillageHouseName body)
        {

            var result = new MessageResult<int>("失败", false, body.Id);

            if (string.IsNullOrWhiteSpace(body.HouseName))
            {

                result.Message = "请输入名称";
                return result;
            }

            if (body.AreaId <= 0)
            {
                result.Message = "请选择行政区域";
                return result;
            }

            var data = await this.houseNameService.GetAsync(a => a.AreaId == body.AreaId && a.HouseName == body.HouseName && a.IsDeleted == 0);

            if (data != null && data.Id != body.Id)
            {
                result.Message = "已经存在相同的名称";
                return result;
            }



            if (body.Id > 0)
            {
                body.UpdatedAt = DateTime.Now;
                // var res = await this.houseNameService.UpdateAsync(body);


                var res = this.houseNameService.GetQueryable().Where(a => a.Id == body.Id && a.IsDeleted == 0).UpdateFromQuery(a => new VillageHouseName()
                {
                    HouseName = body.HouseName,
                    Sequence = body.Sequence,
                    Remark = body.Remark,
                    AreaId = body.AreaId,
                    UpdatedAt = body.UpdatedAt,
                    UpdatedBy = body.UpdatedBy,
                });
                if (res <= 0)
                {
                    result.Message = "失败";
                    return result;
                }
                await this.Context.Database.ExecuteSqlRawAsync(" update VillageHouseholdCode set HouseName={0},HouseNameSequence={1},isSync = 0 where HouseNameId={2}", body.HouseName, body.Sequence, body.Id);
            }
            else
            {
                var res = await this.houseNameService.InsertAsync(body);
                if (res == null)
                {
                    result.Message = "失败";
                    return result;
                }
                result.Data = res.Id;
            }

            result.Message = "成功";
            result.Flag = true;
            return result;
        }

        /// <summary>
        /// 获取门牌名详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<VillageHouseNameDto> GetHouseNameDetail(int id)
        {
            var res = await this.houseNameService.GetAsync(id);
            return mapper.Map<VillageHouseNameDto>(res);
        }


        /// <summary>
        /// 删除门牌名
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedBy">操作人Id</param>
        /// <returns></returns>

        public async Task<bool> DeleteHouseName(int id, int updatedBy = 0)
        {
            var householdCode = await this.GetQueryable().Where(code => code.HouseNameId == id && code.IsDeleted == 0).FirstOrDefaultAsync();
            if (householdCode != null)
            {
                throw new ValidException("门牌名正在使用，不允许删除");
            }
            var res = this.houseNameService.GetQueryable().Where(a => a.Id == id && a.IsDeleted == 0).UpdateFromQuery(a => new VillageHouseName()
            {
                IsDeleted = 1,
                UpdatedAt = DateTime.Now,
                UpdatedBy = updatedBy,
            });
            return res > 0;
        }

        public async Task<List<VillageHouseholdCodeTemplateDto>> TemplateList(int areaId)
        {
            if (areaId <= 0)
            {
                throw new ValidException("请指定行政区域");
            }
            var templates = await templateSerivce.GetQueryable().Where(t => t.AreaId == areaId && t.IsDeleted == 0).OrderByDescending(t => t.Id).ToListAsync();
            List<VillageHouseholdCodeTemplateDto> templateDots = mapper.Map<List<VillageHouseholdCodeTemplateDto>>(templates);
            if (templateDots != null)
            {
                foreach (var templateDto in templateDots)
                {
                    SunFileInfo background = await sunFileInfoService.GetQueryable().FirstOrDefaultAsync(f => f.Id == templateDto.Background);
                    templateDto.BackgroundInfo = mapper.Map<SunFileInfoDto>(background);
                    templateDto.TemplateImage = await GetTemplateImage(mapper.Map<VillageHouseholdCodeTemplate>(templateDto));
                }
            }
            return templateDots;
        }


        public async Task<bool> RemoveTemplate(int id)
        {
            if (id <= 0)
            {
                throw new ValidException("无效的编码");
            }

            int count = await this.templateSerivce.GetQueryable().Where(t => t.Id == id).UpdateFromQueryAsync(a => new VillageHouseholdCodeTemplate() { IsDeleted = 1 });
            return count > 0;
        }

        public async Task<VillageHouseholdCodeTemplateDto> TemplateDetail(int id)
        {
            if (id <= 0)
            {
                throw new ValidException("无效的编码");
            }
            VillageHouseholdCodeTemplate template = await this.templateSerivce.GetAsync(id);
            if (template == null)
            {
                return null;
            }
            VillageHouseholdCodeTemplateDto templateDto = mapper.Map<VillageHouseholdCodeTemplateDto>(template);
            if (templateDto != null)
            {
                SunFileInfo background = await sunFileInfoService.GetQueryable().FirstOrDefaultAsync(f => f.Id == template.Background);
                templateDto.BackgroundInfo = mapper.Map<SunFileInfoDto>(background);
            }
            return templateDto;
        }

        public async Task<int> SaveTemplate(VillageHouseholdCodeTemplateDto templateDto)
        {
            VillageHouseholdCodeTemplate template = mapper.Map<VillageHouseholdCodeTemplate>(templateDto);
            if (template.QrCodeNoShow == 1)
            {
                if (template.QrCodeNoXaxis <= 0)
                {
                    throw new ValidException("请填写编号的横坐标");
                }
                if (template.QrCodeNoYaxis <= 0)
                {
                    throw new ValidException("请填写编号的纵标");
                }
                if (template.QrCodeNoFontSize <= 0)
                {
                    throw new ValidException("请填写编号的字号");
                }
            }
            if (template == null)
            {
                throw new ValidException("参数错误");
            }

            if (template.Id > 0)
            {
                var exitTemplate = await this.templateSerivce.GetQueryable().FirstOrDefaultAsync(t => t.Name == template.Name && t.AreaId == template.AreaId && t.Id != template.Id);
                if (exitTemplate != null)
                {
                    throw new ValidException("模板名称已存在");
                }
                await this.templateSerivce.UpdateAsync(template);
                return template.Id;
            }
            else
            {
                var exitTemplate = await this.templateSerivce.GetQueryable().FirstOrDefaultAsync(t => t.Name == template.Name && t.AreaId == template.AreaId);
                if (exitTemplate != null)
                {
                    throw new ValidException("模板名称已存在");
                }
                var result = await this.templateSerivce.InsertAsync(template);
                return result.Id;
            }
        }

        public async Task<string> GetTemplateImage(VillageHouseholdCodeTemplate template)
        {
            if (template.Id > 0)
            {
                template = await templateSerivce.GetQueryable().FirstAsync(t => t.Id == template.Id);
            }
            if (template.Background <= 0)
            {
                throw new ValidException("请上传背景图");
            }
            SunFileInfo background = null;
            VillageHouseholdCodeTemplateDto templateDto = mapper.Map<VillageHouseholdCodeTemplateDto>(template);
            if (template != null)
            {
                background = await sunFileInfoService.GetQueryable().FirstOrDefaultAsync(f => f.Id == template.Background);
                templateDto.BackgroundInfo = mapper.Map<SunFileInfoDto>(background);
            }

            HttpResponseMessage message = await factory.PostAsync(configuration["HouseholdCodeService:Url"] + "/api/qrcode/getHouseholdCode", new
            {
                CodeData = JsonSerializer.Serialize(new
                {
                    areaCode = 610403001888,
                    houseName = "门牌名称显示区域",
                    houseNumber = "0008",
                    url = configuration["HouseholdCode:Url"] + "?id=" + 1 + "&areaId=610403001888" + "&action=1",
                    tepmlateImage = true
                }),
                Template = templateDto,
            });

            string qrCodeStringData = await message.Content.ReadAsStringAsync();
            return "data:image/jpg;base64," + qrCodeStringData;
        }



        public async Task<HouseholderDto> GetHouseholderInfo(int householdId)
        {

            string sql = string.Format(@"SELECT 
                                        h.houseName,
                                        h.houseNumber,
                                        a.`name` as areaName,
                                        p.realName,
                                        h.areaId,
                                        m.householdId,
                                        p.mobile,
                                        m.populationId,
                                        '' as fullName
                                        FROM VillageHouseholdCode h
                                        LEFT JOIN BasicArea a on h.areaId = a.id and a.IsDeleted=0
                                        LEFT JOIN VillageHouseCodeMember m on h.id= m.householdId  and m.IsDeleted=0 and m.isHouseholder=1
                                        LEFT JOIN VillagePopulation p on m.populationId = p.id and p.IsDeleted=0
                                        WHERE h.id={0} and h.IsDeleted=0", householdId);

            var data = this.Context.Database.SqlQuery<HouseholderDto>(sql).FirstOrDefault();
            if (data != null)
            {
                data.RealName = BasicSO.Decrypt(data.RealName);
                data.Mobile = BasicSO.Decrypt(data.Mobile);

                data.FullName = data.AreaName + data.HouseName + data.HouseNumber + data.RealName;
            }
            return data;
        }


        public async Task<int> GetHouseholdMemberCount(int householdId)
        {
            var query = from m in this.memberService.GetQueryable() where m.IsDeleted == 0 && m.HouseholdId == householdId select m;
            var count = await query.CountAsync();
            return count;
        }
    }
}