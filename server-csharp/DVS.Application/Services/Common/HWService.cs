using DVS.Common;
using DVS.Common.Http;
using DVS.Common.Models;
using DVS.Models.Dtos.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
    public class HWService : IHWService
    {
        private IHttpClientFactory factory;
        private IConfiguration configuration;
        public HWService(IHttpClientFactory factory,IConfiguration configuration)
        {
            this.factory = factory;
            this.configuration = configuration;
        }
        public async Task<HWOcrIDCardRes> ScanIdCard(IFormFile file)
        {
            if (file == null)
            {
                throw new ValidException("请上传身份证的正面照");
            }
            string token = await getHWToken();
            Stream fileSteam = file.OpenReadStream();
            byte[] buffer = new byte[file.Length];
            await fileSteam.ReadAsync(buffer, 0, buffer.Length);
            string imgBase64String = Convert.ToBase64String(buffer);
            // 请求华为扫描身份证接口

            HttpResponseMessage scanResult = await factory.PostAsync(configuration["HuaWei:IDCard:Url"], new { image = imgBase64String }, (header) =>
            {
                header.Add("X-Auth-Token", token);
            });
            string scanResultJson = await scanResult.Content.ReadAsStringAsync();
            HWOcrIDCardResInfo ocrIdCardResult = JsonSerializer.Deserialize<HWOcrIDCardResInfo>(scanResultJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (ocrIdCardResult.Error_Code == "AIS.0104")
            {
                throw new ValidException("未检测到身份证信息");
            }
            else if (ocrIdCardResult.Error_Code == "APIG.0301")
            {
                throw new ValidException("识别接口token无效:" + ocrIdCardResult.Error_Msg);
            }
            else if (ocrIdCardResult.Result == null)
            {
                throw new ValidException("识别失败:" + ocrIdCardResult.Error_Msg);
            }
            return ocrIdCardResult.Result;
        }

        private async Task<string> getHWToken()
        {
            string tokenKey = "hw_token";
            string token = await RedisHelper.GetAsync(tokenKey);
            if (token.IsNullOrWhiteSpace())
            {
                // 获取华为token
                HttpResponseMessage res = await factory.PostAsync(configuration["HuaWei:Token:Url"], new
                {

                    auth = new
                    {
                        identity = new
                        {
                            methods = new List<string>() { "password" },
                            password = new
                            {
                                user = new
                                {
                                    name = configuration["HuaWei:Token:Name"],
                                    password = Utils.DESDecrypt(configuration["HuaWei:Token:Password"]),
                                    domain = new
                                    {
                                        name = configuration["HuaWei:Token:DomainName"]
                                    }
                                }
                            }
                        },
                        scope = new
                        {
                            project = new
                            {
                                name = configuration["HuaWei:Scope:Name"]
                            }
                        }
                    }
                });
                if (res.Headers.Contains("X-Subject-Token"))
                {
                    IEnumerable<string> headers = res.Headers.GetValues("X-Subject-Token");
                    IEnumerator<string> enumerator = headers.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        token = enumerator.Current;
                        // 12小时过期
                        await RedisHelper.SetAsync(tokenKey, token, 1 * 60 * 60 * 12);
                    }
                }
            }
            if (token.IsNullOrWhiteSpace())
            {
                throw new ValidException("识别接口token获取失败");
            }

            return token;
        }
    }
}
