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

namespace DVS.Cons.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "cons")]
    [DvsResult]
    [DvsException]
    [Route("/api/cons/[controller]")]
    public class DvsControllerBase : ControllerBase, IDVSController
    {
        IBasicUserService basicUserSerivce;
        IServiceBase<VillageHouseCodeMember> householdCodeMemberService;
        public DvsControllerBase( )
        {
            this.basicUserSerivce = ServiceLocator.Current.GetService<IBasicUserService>();
            this.householdCodeMemberService = ServiceLocator.Current.GetService<IServiceBase<VillageHouseCodeMember>>();

        }
        private LoginUser loginUser = null;
        public LoginUser LoginUser
        {
            get
            {
                if (loginUser != null&&loginUser.UserId>0)
                {
                    BasicUserDto userInfo = basicUserSerivce.GetUserInfo(loginUser.UserId).GetAwaiter().GetResult();
                    var member = householdCodeMemberService.GetQueryable().Where(member => member.HouseholdId == userInfo.HouseholdId).FirstOrDefault();
                    if (userInfo != null)
                    {
                        LoginUser loginUserInfo = Mapper.Map<LoginUser>(userInfo);
                        if (member != null) {
                            loginUserInfo.IsHouseholder = member.IsHouseholder;
                        }
                        loginUserInfo.UserId = loginUser.UserId;
                        return loginUserInfo;
                    }
                }
                return loginUser;
            }
            set { loginUser = value; }
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