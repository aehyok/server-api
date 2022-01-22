using DVS.Common.Http;
using DVS.Models.Dtos.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
    public class PushResult
    {

        /// <summary>
        /// 错误码 0正常，-1或其他不正常
        /// </summary>
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

    }
    public class PushService : IPushService
    {
        private readonly IHttpClientFactory factory;
        private readonly IConfiguration configuration;
        private readonly ILogger<PushService> logger;
        public PushService(IHttpClientFactory factory, IConfiguration configuration, ILogger<PushService> logger)
        {
            this.factory = factory;
            this.configuration = configuration;
            this.logger = logger;
        }



        public async Task<bool> PushMessage(PushMessageDto message)
        {

            // appId=xxx&timestamp=1608882850537&sign=xxxx

            string appId = configuration["Push:AppId"];
            string secret = configuration["Push:Secret"];
            string url = configuration["Push:Url"];
            string timestamp = DVS.Common.Utils.GetTimeStamp();
            string sign = DVS.Common.Utils.MD5("appId=" + appId + "&secret=" + secret + "&timestamp=" + timestamp);

            var body = new
            {
                aliasList = message.AliasList,
                title = message.Title,
                content = message.Content,
                extras = message.Extras,
                timeToLive = message.TimeToLive,
                huaweiPushIds = message.HuaweiPushIds,
                debugUserIds = message.DebugUserIds,
            };
            HttpResponseMessage scanResult = await factory.PostAsync($"{url}?appId={appId}&timestamp={timestamp}&sign={sign}", body, (header) => { });

            if (scanResult.StatusCode != System.Net.HttpStatusCode.OK) {
                this.logger.LogError("PushMessage error StatusCode " + scanResult.StatusCode.ToString());
                return false;
            }
         
            string scanResultJson = await scanResult.Content.ReadAsStringAsync();
            PushResult result = JsonSerializer.Deserialize<PushResult>(scanResultJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
          
            if (result != null && result.Code == 0)
            {
                return true;
            }
            else
            {
                this.logger.LogError("PushMessage error " + scanResultJson);
            }
            return false;
        }
    }
}
