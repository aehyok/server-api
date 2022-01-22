using DVS.Common.Infrastructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.SMS
{
    public interface ISendSMSService
    {
        MessageResult<object> Send(List<string> mobiles, string template);
        /// <summary>
        /// 发送用户申请认证验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        Task<MessageResult<object>> SendUserAuthCode(string mobile);

        Task<MessageResult<object>> SendWarning(string mobile, string Title, string Content,string extendCotent = "");
    }
}
