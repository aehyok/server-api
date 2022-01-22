using DVS.Common.Infrastructures;
using DVS.Common.RPC;
using DVS.Models.Const;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.SMS
{

    /// <summary>
    /// 发送短信
    /// </summary>
    public class SendSMSService : ISendSMSService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<SendSMSService> logger;
        public SendSMSService(IConfiguration configuration, ILogger<SendSMSService> logger)
        {

            this.configuration = configuration;
            this.logger = logger;
        }

        public MessageResult<object> Send(List<string> mobiles, string template)
        {
            var result = new MessageResult<object>("发送失败");
            try
            {
                var res = BasicRPC.SendSMS(mobiles, template);
                if (res != null && res.Code == 0)
                {
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = true;
                    return result;
                }
                if (res != null && res.Code != 0)
                {
                    result.Message = res.Message;
                    result.Data = res.Data;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<MessageResult<object>> SendUserAuthCode(string mobile)
        {
            var result = new MessageResult<object>("发送失败");
            string tpl= this.configuration["SMSTPL:UserAuthCodeTpl"];
            if (string.IsNullOrWhiteSpace(tpl)) {
                tpl = "您的验证码为：{0}，10分钟之内有效。";
            }

            string key = $"{ConstCommon.USER_AUTH_SMS_CODE_REDIS}_{mobile}";

            Random rand = new Random();
            var code = rand.Next(100001, 999999);


            var redis = await RedisHelper.SetAsync(key, code, 600);
            if (!redis) {

                result.Message = "发送失败。10001";
                return result;
            }
            var res = this.Send(new List<string>() { mobile }, string.Format(tpl, code));
            if (!res.Flag) {
                await RedisHelper.DelAsync(key);
            }
            return res;
        }

        /// <summary>
        /// 发送报警短信
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="Title"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public async Task<MessageResult<object>> SendWarning(string mobile,string Title,string Content, string extendCotent = "")
        {
            var result = new MessageResult<object>("发送失败");
            string tpl = this.configuration["SMSTPL:WarningTPL"];
            if (string.IsNullOrWhiteSpace(tpl))
            {
                tpl = "【告警通知】{0}：{1} 可登录数字乡村APP，点击告警信息查看详情。";
            }

            var res = this.Send(new List<string>() { mobile }, string.Format(tpl+ extendCotent, Title, Content));
            this.logger.LogInformation("SendWarning info " + res.Message);
            if (!res.Flag)
            {
                this.logger.LogError("SendWarning error ", res.Message);
            }
            return res;
        }
    }
}
