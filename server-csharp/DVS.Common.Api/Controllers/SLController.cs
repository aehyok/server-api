using DVS.Common.Guideline;
using DVS.Common.Http;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Common.SO;
using DVS.Models.Models.SSO;
using Lychee.Core.Infrastructures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DVS.Common.Api.Controllers
{
    /// <summary>
    /// 盛阳提供的公共服务
    /// </summary>
    [ApiController]
    [DvsResult]
    [DvsException]
    [Route("/api/common/sl/")]
    public class SLController : ControllerBase
    {
        private readonly IHttpClientFactory factory;
        private readonly IConfiguration configuration;

        public SLController(IHttpClientFactory factory,
            IConfiguration configuration,
            ILogger<SLController> logger,
            IServiceProvider serviceProvider)
        {
            ServiceLocator.Current.ResolverFunc = (type) =>
            {
                using var scope = serviceProvider.CreateScope();
                return scope.ServiceProvider.GetService(type);
            };
            this.factory = factory;
            this.configuration = configuration;
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("encrypt")]
        public string Encrypt([FromBody] SOInfo soInfo)
        {
            return BasicSO.Encrypt(soInfo.Content);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("decrypt")]
        public string Decrypt([FromBody] SOInfo soInfo)
        {
            return BasicSO.Decrypt(soInfo.Content);
        }

        /// <summary>
        /// 取数据查询服务端的TOKEN
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="code">CODE</param>
        /// <returns></returns>
        [HttpPost("GetQueryToken")]
        [AllowAnonymous]
        public async Task<object> GetQueryToken(string userName, string code)
        {
            var token = "";
            string confUrl = configuration["QueryServiceUrl"];
            var url = string.IsNullOrEmpty(confUrl) ? "https://localhost:44396" : confUrl;
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            var body = new { };
            try
            {
                HttpResponseMessage response = await factory.PostAsync($"{url}/api/mdquery/CreateClientToken?userName={userName}&code={code}", body, (header) => { });
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    token = jo.GetValue("data").ToString();
                }
                var ret = new
                {
                    token = token,
                    url = url
                };
                return ret;
            }
            catch (Exception ex)
            {
                throw new ValidException(string.Format("[{0}]取查询服务TOKEN发生错误：{1}", userName, ex.Message));
            }
        }

        /// <summary>
        /// 获取指标定义
        /// </summary>
        /// <param name="guideLineId"></param>
        /// <returns></returns>
        [HttpPost("GetGuidelineDefine")]
        [AllowAnonymous]
        public async Task<MD_GuideLine> GetGuidelineDefine(string guideLineId)
        {

            string confUrl = configuration["HOST"];
            var url = string.IsNullOrEmpty(confUrl) ? "https://localhost:44396" : confUrl;
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            var body = new { };
            try
            {
                MD_GuideLine ret = null;
                HttpResponseMessage response = await factory.PostAsync($"{url}/api/mdquery/GetGuidelineDefine?guideLineId={guideLineId}", body, (header) => { });
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = jo.GetValue("data").ToString();
                    ret = JsonConvert.DeserializeObject<MD_GuideLine>(data);
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw new ValidException(string.Format("[{0}]获取指标定义发生错误：{1}", guideLineId, ex.Message));
            }

        }

        /// <summary>
        /// 取指标全部数据  
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetGuidelineData")]
        [AllowAnonymous]
        public async Task<DataTable> GetGuidelineData(QueryGuidelineModel model)
        {

            string confUrl = configuration["HOST"];
            var url = string.IsNullOrEmpty(confUrl) ? "https://localhost:44396" : confUrl;
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            try
            {
                DataTable ret = null;
                HttpResponseMessage response = await factory.PostAsync($"{url}/api/mdquery/GetGuidelineData", model, (header) => { });
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = jo.GetValue("data").ToString();
                    ret = JsonConvert.DeserializeObject<DataTable>(data);
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw new ValidException(string.Format("[{0}]取指标全部数据发生错误：{1}", model.GuideLineId, ex.Message));
            }

        }

        /// <summary>
        /// 分页取指标数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetGuidelineDataPaged")]
        [AllowAnonymous]
        public async Task<object> GetGuidelineDataPaged(QueryGuidelinePageModel model)
        {

            string confUrl = configuration["HOST"];
            var url = string.IsNullOrEmpty(confUrl) ? "https://localhost:44396" : confUrl;
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            try
            {
                var ret = new GuideLinePageModel();
                HttpResponseMessage response = await factory.PostAsync($"{url}/api/mdquery/GetGuidelineDataPaged", model, (header) => { });
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string s = jo.GetValue("data").ToString();
                    ret = JsonConvert.DeserializeObject<GuideLinePageModel>(s);
                    var fff = ret.Docs[0];                  
                    ret.Docs = fff;                   
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw new ValidException(string.Format("[{0}]分页取指标数据发生错误：{1}", model.GuideLineId, ex.Message));
            }

        }


    }
}
