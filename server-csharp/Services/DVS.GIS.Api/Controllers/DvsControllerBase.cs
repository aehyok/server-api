using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Common;
using Lychee.Core.Infrastructures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace DVS.GIS.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "gis")]
    [DvsResult]
    [DvsException]
    [Route("/api/gis/[controller]")]
    public class DvsControllerBase : ControllerBase, IDVSController
    {
        IBasicUserService basicUserSerivce;
        IServiceBase<VillageHouseCodeMember> householdCodeMemberService;
        public DvsControllerBase( )
        {
            this.householdCodeMemberService = ServiceLocator.Current.GetService<IServiceBase<VillageHouseCodeMember>>();
            this.basicUserSerivce = ServiceLocator.Current.GetService<IBasicUserService>();
        }
        private LoginUser loginUser = null;
        public LoginUser LoginUser
        {
            get
            {
                if (loginUser != null&&loginUser.UserId>0)
                {
                    BasicUserDto userInfo = basicUserSerivce.GetUserInfo(loginUser.UserId).GetAwaiter().GetResult();
                    if (userInfo != null)
                    {
                        var member = householdCodeMemberService.GetQueryable().Where(member => member.HouseholdId == userInfo.HouseholdId).FirstOrDefault();
                        LoginUser loginUserInfo = Mapper.Map<LoginUser>(userInfo);
                        if (member != null)
                        {
                            loginUser.IsHouseholder = member.IsHouseholder;
                        }
                        loginUserInfo.UserId = loginUser.UserId;
                        return loginUserInfo;
                    }
                }
                return loginUser;
            }
           
        }

        protected IMapper Mapper
        {
            get
            {
                return HttpContext.RequestServices.GetRequiredService<IMapper>();
            }
        }
        [NonAction]
        public void SetLoginUser(LoginUser loginUser)
        {
            this.loginUser = loginUser;
        }
    }
}