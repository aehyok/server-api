using DVS.Common.Http;
using DVS.Common.Services.SSO;
using DVS.Models.Models.SSO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DVS.Application.Services.SSO
{
    public class SSOService:ISSOService 
    {
        private  HttpClient client;
        private  IConfiguration configuration;
        private  IHttpClientFactory factory;
        public SSOService(IHttpClientFactory factory, IConfiguration configuration)
        {
            this.factory = factory;
        }

        public async Task<T> GetSSOInfoByToken<T>(string token) where T:LoginUserInfo
        {
            HttpResponseMessage message = await factory.PostAsync(configuration["SSO:SSOServiceUrl"], new { token });
            LoginUserInfo loginUserInfo = JsonSerializer.Deserialize<LoginUserInfo>(await message.Content.ReadAsStringAsync());
            RedisHelper.Set(token, JsonSerializer.Serialize(loginUserInfo), loginUserInfo.TokenValidSeconds);
            return loginUserInfo as T;
        }

        
    }
}
