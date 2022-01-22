using DVS.Application.Services.IPS;
using DVS.Common.Models;
using DVS.Core.Domains.IPS;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.IPS;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.IPS.Api.Controllers.App
{
    /// <summary>
    /// 滚动消息
    /// </summary>
    [Route("/api/ips/app/message")]
    public class IPSMessageController : DvsControllerBase
    {

        private readonly IIPSMessageService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public IPSMessageController(IIPSMessageService service)
        {
            this._service = service;
        }

        /// <summary>
        /// 获取滚动消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPagedList<IPSMessageDto>> GetMessageListAsync(PagedListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this._service.ListMessage(model.Keyword, model.Page, model.Limit, loginuser.AreaId);
        }

        /// <summary>
        /// 滚动消息详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("detail")]
        public async Task<IPSMessageDto> GetMessageDetailAsync(DetailQueryModel model)
        {
            return await this._service.DetailMessage(model.Id);
        }

        /// <summary>
        /// 删除滚动消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<int> DeleteMessageAsync(DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this._service.DeleteMessage(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// 保存滚动消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<int> SaveMessageAsync(IpsMessage model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.Id > 0)
            {
                model.UpdatedBy = loginuser.UserId;
            }
            else {
                model.CreatedBy = loginuser.UserId;
                model.UpdatedBy = loginuser.UserId;
                model.AreaId = loginuser.AreaId;
            }
            return await this._service.SaveMessage(model);
        }

        /// <summary>
        /// 统计滚动消息
        /// </summary>
        /// <returns></returns>
        [HttpPost("statistics")]
        public async Task<IPSStatisticDto> StatisticMessageAsync()
        {
            LoginUser loginuser = this.LoginUser;
            return await this._service.StatisticMessage(loginuser.AreaId);
        }
    }
}
