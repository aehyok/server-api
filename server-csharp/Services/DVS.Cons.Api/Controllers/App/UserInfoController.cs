using System.Threading.Tasks;
using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Models;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village;
using Microsoft.AspNetCore.Mvc;

namespace DVS.Cons.Api.Controllers.App
{
    [Route("/api/cons/app/basicuser")]
    public class UserInfoController : DvsControllerBase
    {
        private readonly IMapper mapper;
        private readonly IBasicUserService dataService;

        public UserInfoController(IMapper mapper, IBasicUserService service)
        {
            this.mapper = mapper;
            this.dataService = service;
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <returns></returns>
        [HttpPost("getUserInfo")]
        public async Task<BasicUserDto> GetUserInfoAsync()
        {
            LoginUser loginuser = this.LoginUser;
            return await this.dataService.GetUserInfo(loginuser.UserId);
        }
    }


}