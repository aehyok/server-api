using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Application.Services.Cons;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Cons;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.FFP
{
    public class FFPPublicityManageService : ServiceBase<FFPPublicityManage>, IFFPPublicityManageService
    {

        private readonly IBasicUserService basicUserService;
        private readonly IFFPPublicityHouseholdService householdService;
        private readonly IInfoPublicService infoPublicService;

        public FFPPublicityManageService(DbContext dbContext, IMapper mapper,
            IBasicUserService basicUserService,
            IFFPPublicityHouseholdService householdService,
            IInfoPublicService infoPublicService)
        : base(dbContext, mapper)
        {
            this.basicUserService = basicUserService;
            this.householdService = householdService;
            this.infoPublicService = infoPublicService;
        }

        /// <summary>
        /// 保存公示信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="hd"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> SavePublicityManage(FFPPublicityManage model, PublicityManageAddDto hd, ConsInfoPublic info)
        {
            var ret = 0;
            var status = (hd.Type == 1) ? 3 : 4;    //流程状态 1摸排确认2待评议3公示4待上报乡镇9结束
            string sql = string.Format(@"select   w.id, a.id as areaId, a.`name` as areaName, f.householdId, f.householdType, 
                            h.houseName, h.houseNumber, p.mobile, p.realName as householdMan,
                            ifnull(t1.peopleCount, 0) as peopleCount, p.sex,p.idCard, g.moPaiDate as checkDate 
                            from  FFPWorkflow w
                            left join FFPMoPaiLog g on w.moPaiId = g.id                            
                            left join FFPHouseholdCode f on w.householdId = f.householdId and f.isDeleted = 0
                            left join VillageHouseholdCode h on h.id = w.householdId and h.isDeleted = 0
                            left join  VillageHouseCodeMember m on h.id = m.householdId and m.isHouseholder = 1 and m.isDeleted = 0
                            left join  VillagePopulation p on m.populationId = p.id and p.isDeleted = 0
                            left join  BasicArea a on a.id = h.areaId
                            left join(
                                   select n.householdId, COUNT(n.id) as peopleCount from VillageHouseCodeMember n
                                    where n.isDeleted = 0 and n.householdId > 0  group by n.householdId
                            )t1 on w.householdId = t1.householdId
                            where w.householdId  in ({0}) and w.flowStatus={1}", string.Join(",", hd.Ids), status);
            try
            {
                var data = await this.Context.Database.SqlQueryAsync<PublicityHouseholdList>(sql);
                if (data != null && data.Count > 0)
                {
                    // 评议公示把内容发布到信息公开
                    if (hd.Type == 1)
                    {
                        await infoPublicService.SaveConsInfoPublic(info);
                        model.PublishDate = DateTime.Now;
                        model.Publisher = model.CreatedUser;
                        model.PublishStatus = 1;
                    }
                    var r = await this.InsertAsync(model);
                    if (r != null && r.Id > 0)
                    {
                        foreach (var item in data)
                        {
                            FFPPublicityHousehold h = new FFPPublicityHousehold();
                            h.PublicityManageId = r.Id;
                            h.AreaId = item.AreaId;
                            h.HouseholdId = item.HouseholdId;
                            h.HouseName = item.HouseName;
                            h.HouseNumber = item.HouseNumber;
                            h.HouseholdMan = item.HouseholdMan;
                            h.Sex = 1;
                            h.Relationship = "户主";
                            h.IdCard = item.IdCard;
                            h.PeopleCount = item.PeopleCount;
                            h.Mobile = item.Mobile;
                            h.HouseholdType = item.HouseholdType;
                            h.MonitorObj = 1;
                            h.CheckDate = item.CheckDate;

                            await householdService.InsertAsync(h);
                        }
                    }
                    ret = r.Id;
                }
                else
                {
                    throw new ValidException("数据不存在");
                }
            }
            catch (Exception ex)
            {
                throw new ValidException(string.Format("保存公示信息发生错误:{0}", ex.Message));
            }
            return ret;

        }

    }
}
