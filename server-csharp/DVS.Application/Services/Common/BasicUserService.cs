using AutoMapper;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Village;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using DVS.Application.Services.Village;
using DVS.Core.Domains.Village;
using DVS.Models.Enum;
using DVS.Common;
using DVS.Models.Dtos.Common;
using DVS.Common.SO;

namespace DVS.Application.Services.Common
{
    public class BasicUserService : ServiceBase<BasicUser>, IBasicUserService
    {

        private readonly IServiceBase<VillagePopulation> populationService;
        private readonly IServiceBase<VillagePopulationAddress> populationAddressService;
        private readonly IBasicAreaService basicAreaService;
        private readonly IServiceBase<VillageHouseholdCode> householdCodeService;
        private readonly IServiceBase<BasicUserLogin> userLoginService;
        private readonly IServiceBase<VillageHouseCodeMember> houseCodeMemberSerivce;
        private readonly IServiceBase<VillageHouseCodeMember> memberService;
        private readonly IServiceBase<BasicRole> basicRoleService;
        public BasicUserService(DbContext dbContext,
            IMapper mapper,
            IServiceBase<VillagePopulation> populationService,
            IServiceBase<VillagePopulationAddress> populationAddressService,
            IBasicAreaService basicAreaService,
            IServiceBase<VillageHouseholdCode> householdCodeService,
            IServiceBase<BasicUserLogin> userLoginService,
            IServiceBase<VillageHouseCodeMember> houseCodeMemberSerivce,
            IServiceBase<VillageHouseCodeMember> memberService,
            IServiceBase<BasicRole> basicRoleService
            )
            : base(dbContext, mapper)
        {
            this.populationService = populationService;
            this.populationAddressService = populationAddressService;
            this.basicAreaService = basicAreaService;
            this.householdCodeService = householdCodeService;
            this.userLoginService = userLoginService;
            this.houseCodeMemberSerivce = houseCodeMemberSerivce;
            this.memberService = memberService;
            this.basicRoleService = basicRoleService;
        }

        public async Task<UserAuthStatusDto> GetUserAuthStatus(int userId)
        {
            var query = from u in this.GetQueryable()
                        join h in this.householdCodeService.GetQueryable() on new { Id = u.HouseholdId, IsDeleted = 0 } equals new { h.Id, h.IsDeleted } into htemp
                        from hh in htemp.DefaultIfEmpty()
                            //join p in this.populationAddressService.GetQueryable() on new { Id = u.PopulationId, IsDeleted = 0 } equals new { p.Id, p.IsDeleted } into ptemp
                            //from pp in ptemp.DefaultIfEmpty()
                        join a in this.basicAreaService.GetQueryable()
                        on u.AreaId equals a.Id into atmp
                        from aa in atmp.DefaultIfEmpty()
                        where u.Id == userId && u.IsDeleted == 0
                        select new UserAuthStatusDto()
                        {
                            UserId = u.Id,
                            DepartmentId = u.DepartmentIds,
                            UserName = u.NickName,
                            AreaId = u.AreaId,
                            AuditStatus = u.IsAuth,
                            PopulationId = u.PopulationId,
                            Mobile = u.Mobile,
                            HouseholdId = u.HouseholdId,
                            HouseName = hh.HouseName,
                            HouseNumber = hh.HouseNumber,
                            AreaName = aa.Name,
                            AreaCode = aa.AreaCode,
                        };
            var user = await query.FirstOrDefaultAsync();

            if (user.AuditStatus == UserAuthAuditStatusEnum.Passed && user.HouseholdId <= 0)
            {
                var houseQuery = from m in this.memberService.GetQueryable()
                                 join h in this.householdCodeService.GetQueryable()
                                 on new { Id = m.HouseholdId, IsDeleted = 0 } equals new { h.Id, h.IsDeleted } into htemp
                                 from hh in htemp.DefaultIfEmpty()
                                 where m.PopulationId == user.PopulationId && m.IsDeleted == 0
                                 select hh;
                var house = await houseQuery.FirstOrDefaultAsync();
                if (house != null)
                {

                    user.HouseholdId = house.Id;
                    user.HouseName = house.HouseName;
                    user.HouseNumber = house.HouseNumber;
                }
            }

            user.Mobile = BasicSO.Decrypt(user.Mobile);
            user.UserName = BasicSO.Decrypt(user.UserName);
            return user;
        }

        public async Task<VillagePopulationDto> GetPopulationByUserId(int id)
        {
            var data = from u in this.GetQueryable()
                       join p in this.populationService.GetQueryable() on u.PopulationId equals p.Id
                       join m in this.houseCodeMemberSerivce.GetQueryable() on u.PopulationId equals m.PopulationId
                       where u.Id == id && u.IsDeleted == 0 && u.Type == 1 && u.HouseholdId == m.HouseholdId
                       select new VillagePopulationDto()
                       {
                           AreaId = u.AreaId,
                           HouseholdId = u.HouseholdId,
                           Id = p.Id,
                           RealName = Utils.Decrypt(p.RealName),
                           Mobile = Utils.Decrypt(u.Mobile),
                           RegisterAddress = "", // by owen 
                           LiveAddress = "", // by owen
                           IsHouseholder = m.IsHouseholder,
                           HeadImageUrl = p.HeadImageUrl,
                           Relationship = p.Relationship,
                           Nation = p.Nation,
                           Sex = p.Sex,
                           PortraitFileId = u.PortraitFileId
                       };
            var result = data.FirstOrDefault();
            if (result != null)
            {
                var registerAddress = await this.populationAddressService.GetAsync(a => a.PopulationId == result.Id && a.Type == DVS.Models.Enum.PopulationAddressTypeEnum.户籍地);
                if (registerAddress != null)
                {
                    result.RegisterAddress = registerAddress.Province + registerAddress.City + registerAddress.District + registerAddress.Address;
                }

                var liveAddress = await this.populationAddressService.GetAsync(a => a.PopulationId == result.Id && a.Type == DVS.Models.Enum.PopulationAddressTypeEnum.居住地);
                if (liveAddress != null)
                {
                    result.LiveAddress = liveAddress.Province + liveAddress.City + liveAddress.District + liveAddress.Address;
                }

                result.RealName = Utils.Decrypt(result.RealName);
                result.Mobile = Utils.Decrypt(result.Mobile);
                result.LiveAddress = Utils.Decrypt(result.LiveAddress);
                result.RegisterAddress = Utils.Decrypt(result.RegisterAddress);
            }

            return result;
        }


        /// <summary>
        /// 获取相关状态用户数
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="auditStatus"></param>
        /// <returns></returns>
        public async Task<int> GetUserAuthCount(int areaId, UserAuthAuditStatusEnum auditStatus = 0)
        {
            if (areaId <= 0)
            {
                return 0;
            }
            var data = from u in this.GetQueryable()
                       where u.IsDeleted == 0 && u.AreaId == areaId && u.IsAuth == auditStatus
                       select u;
            return await data.CountAsync();
        }

        public override async Task<IList<BasicUser>> GetListAsync(Expression<Func<BasicUser, bool>> predicate)
        {
            var userList = await base.GetListAsync(predicate);
            if (userList != null)
            {
                foreach (var user in userList)
                {
                    user.NickName = Utils.Decrypt(user.NickName);
                    user.Mobile = Utils.Decrypt(user.Mobile);
                    user.Email = Utils.Decrypt(user.Email);
                    user.Address = Utils.Decrypt(user.Address);
                }
            }
            return userList;
        }

        public override async Task<BasicUser> GetAsync(Expression<Func<BasicUser, bool>> predicate)
        {
            var user = await base.GetAsync(predicate);
            if (user != null)
            {
                user.NickName = Utils.Decrypt(user.NickName);
                user.Mobile = Utils.Decrypt(user.Mobile);
                user.Email = Utils.Decrypt(user.Email);
                user.Address = Utils.Decrypt(user.Address);
            }
            return user;
        }

        public async Task<BasicUserDto> GetUserInfo(int id)
        {
            var data = await this.GetAsync(id);
            var result = mapper.Map<BasicUserDto>(data);
            if (result != null)
            {
                var registerAddress = await this.populationAddressService.GetAsync(a => a.PopulationId == data.PopulationId && a.Type == DVS.Models.Enum.PopulationAddressTypeEnum.户籍地);
                if (registerAddress != null)
                {
                    result.RegisterAddress = registerAddress.Province + registerAddress.City + registerAddress.District + registerAddress.Address;
                }

                var liveAddress = await this.populationAddressService.GetAsync(a => a.PopulationId == data.PopulationId && a.Type == DVS.Models.Enum.PopulationAddressTypeEnum.居住地);
                if (liveAddress != null)
                {
                    result.LiveAddress = liveAddress.Province + liveAddress.City + liveAddress.District + liveAddress.Address;
                }

                result.NickName = Utils.Decrypt(result.NickName);
                result.Mobile = Utils.Decrypt(result.Mobile);
                result.LiveAddress = Utils.Decrypt(result.LiveAddress);
                result.RegisterAddress = Utils.Decrypt(result.RegisterAddress);

                result.AreaName = await this.basicAreaService.GetAreaName(data.AreaId);

                if (data.RoleIds != "")
                {
                    var ids = data.RoleIds.Split(",").ToList();
                    var list = await this.basicRoleService.GetListAsync(a => ids.Contains(a.Id.ToString()) && a.Type == data.Type);
                    result.DataAcces = "self"; // 最小权限，查看本人
                    foreach (var item in list) {
                        if (item.DataAccess == "group" && result.DataAcces != "all") {
                            result.DataAcces = item.DataAccess;
                        }
                        if (item.DataAccess == "all")
                        {
                            result.DataAcces = item.DataAccess;
                        }
                    }
                }
                result.AreaIds = await this.basicAreaService.FindChildrenAreaIds(result.AreaId);

            }

            return result;
        }

        /// <summary>
        /// 根据userid查找户主信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<VillagePopulationDto> GetPopulationMasterByUserId(int userid)
        {
            var data = from u in this.GetQueryable()
                       join p in this.populationService.GetQueryable() on u.PopulationId equals p.Id
                       join m in this.houseCodeMemberSerivce.GetQueryable() on p.Id equals m.PopulationId
                       join e in this.householdCodeService.GetQueryable() on u.HouseholdId equals e.Id
                       where u.Id == userid && m.IsHouseholder == 1 && u.HouseholdId == m.HouseholdId
                       select new VillagePopulationDto()
                       {
                           AreaId = u.AreaId,
                           HouseholdId = u.HouseholdId,
                           Id = p.Id,
                           RealName = p.RealName,
                           Mobile = u.Mobile,
                           RegisterAddress = "",
                           LiveAddress = "",
                           IsHouseholder = m.IsHouseholder,
                           HouseName = e.HouseName,
                           HouseNumber = e.HouseNumber,
                       };
            var result = data.FirstOrDefault();
            if (result != null)
            {
                var registerAddress = await this.populationAddressService.GetAsync(a => a.PopulationId == result.Id && a.Type == DVS.Models.Enum.PopulationAddressTypeEnum.户籍地);
                if (registerAddress != null)
                {
                    result.RegisterAddress = registerAddress.Province + registerAddress.City + registerAddress.District + registerAddress.Address;
                }

                var liveAddress = await this.populationAddressService.GetAsync(a => a.PopulationId == result.Id && a.Type == DVS.Models.Enum.PopulationAddressTypeEnum.居住地);
                if (liveAddress != null)
                {
                    result.LiveAddress = liveAddress.Province + liveAddress.City + liveAddress.District + liveAddress.Address;
                }

                result.RealName = Utils.Decrypt(result.RealName);
                result.Mobile = Utils.Decrypt(result.Mobile);
                result.LiveAddress = Utils.Decrypt(result.LiveAddress);
                result.RegisterAddress = Utils.Decrypt(result.RegisterAddress);
            }

            return result;
        }

       
        public async Task<string> updateUserPushId(int userId, string pushId, string manufacturer)
        {
            if (userId <= 0) {
                return "";
            }

            var data = await this.userLoginService.GetAsync(a => a.UserId == userId);

            BasicUserLogin userLogin = new BasicUserLogin()
            {
                UserId = userId,
                PushId = pushId,
                Manufacturer = manufacturer,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            if (string.IsNullOrWhiteSpace(pushId))
            {
                userLogin.PushId = Guid.NewGuid().ToString().Replace("-", "");
            }


            if (data != null)
            {
                data.UserId = userId;
                data.PushId = userLogin.PushId;
                data.Manufacturer = manufacturer;
                var res = await this.userLoginService.UpdateAsync(data);
                return res > 0 ? data.PushId : "";
            }
            else
            {
                var res = await this.userLoginService.InsertAsync(userLogin);
                return res != null ? res.PushId : "";
            }
        }
    }
}
