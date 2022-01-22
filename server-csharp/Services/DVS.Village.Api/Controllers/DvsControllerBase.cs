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
using System.Linq;

namespace DVS.Village.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "village")]
    [DvsResult]
    [DvsException]
    [Route("village/[controller]")]
    public class DvsControllerBase : Controller, IDVSController
    {
        IBasicUserService basicUserSerivce;
        IServiceBase<VillageHouseCodeMember> householdCodeMemberService;
        public DvsControllerBase( )
        {
            this.basicUserSerivce  = ServiceLocator.Current.GetService<IBasicUserService>();
            this.householdCodeMemberService = ServiceLocator.Current.GetService<IServiceBase<VillageHouseCodeMember>>();
        }
        private LoginUser loginUser = null;
        public LoginUser LoginUser
        {
            get
            {
                if (loginUser != null&&loginUser.UserId!=0)
                {
                    BasicUserDto userInfo = basicUserSerivce.GetUserInfo(loginUser.UserId).GetAwaiter().GetResult();
                    if (userInfo != null)
                    {
                       var member =  householdCodeMemberService.GetQueryable().Where(member => member.HouseholdId == userInfo.HouseholdId).FirstOrDefault();
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