using AutoMapper;
using DVS.Common.Http;
using DVS.Common.ModelDtos;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Village;
using DVS.Models.Dtos.Village.Statistics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DVS.Application.Services.Village
{
    public class VillageSyncSurveyService : ServiceBase<VillageHouseholdCode>, IVillageSyncSurveyService
    {
        private readonly IHttpClientFactory factory;
        private readonly IConfiguration configuration;
        private readonly ILogger<VillageSyncSurveyService> Logger;

        public VillageSyncSurveyService(DbContext dbContext, IMapper mapper, IHttpClientFactory factory, ILogger<VillageSyncSurveyService> logger, IConfiguration configuration)
            : base(dbContext, mapper)
        {
            this.factory = factory;
            this.configuration = configuration;
            this.Logger = logger;
        }
        
        private async Task<ResultModel<VillageHouseholdCode>> PostAsync(string url, Object data)
        {
            string token = await this.GetTokenAsync();
            ResultModel<VillageHouseholdCode> ret = new ResultModel<VillageHouseholdCode>
            {
                Code = 0
            };
            try
            {
                HttpResponseMessage response = await this.factory.PostAsync(url, data, (header) =>
                {
                    header.Add("Authorization", token);
                });
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {                   
                    ret.Message = jo.GetValue("data").ToString();
                    return ret;
                }
                else
                {
                    ret.Code = -1;
                    ret.Message = jo != null ? jo.GetValue("description").ToString() : "";
                    this.Logger.LogInformation(jo != null ? jo.GetValue("description").ToString() : "");
                    return ret;
                }
            }
            catch (Exception ex)
            {
                ret.Code = -1;
                ret.Message = ex.Message;
                this.Logger.LogInformation(ex.Message);
                return ret;
            }
        }
        
        private async Task<string> GetTokenAsync()
        {
            var url = "https://api.sea.utuapp.cn/connect/token";
            if (configuration.GetValue<string>("BIGDATA:TOKEN_URL") != null)
            {
                url = configuration.GetValue<string>("BIGDATA:TOKEN_URL").ToString();
            }
            string clientId = configuration.GetValue<string>("BIGDATA:CLIENT_ID").ToString();
            string secret = configuration.GetValue<string>("BIGDATA:CLIENT_SECRET").ToString();

            FormUrlEncodedContent fromContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("client_id", clientId),
                new KeyValuePair<string,string>("client_secret", secret),
                new KeyValuePair<string,string>("grant_type", "client_credentials")
            });
            fromContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpClient client = factory.CreateClient();
            HttpResponseMessage message = await client.PostAsync(url, fromContent);

            client.Dispose();

            var result = await message.Content.ReadAsStringAsync();
            if (message.StatusCode != System.Net.HttpStatusCode.OK) {
                this.Logger.LogInformation(result);
                return "";
            }
            JObject jo = (JObject)JsonConvert.DeserializeObject(result);
            var token = jo.GetValue("token_type").ToString() + " " + jo.GetValue("access_token").ToString();
            return token;
        }
       
        public async Task<int> SyncVillageSurveyToDVM()
        {
            var year = DateTime.Now.Year;
            string sql = @"select d.id,d.villageId,d.dataYear,d.population,d.tenement,d.area,b.areaCode from DigitalVillageSurvey d, BasicArea b where d.villageId = b.id and d.dataYear = {0} and d.isDeleted = 0 ";
            sql = string.Format(sql, year);
            List<DigitalVillageSurveyDto> pageData;
            try
            {
                pageData = this.Context.Database.SqlQuery<DigitalVillageSurveyDto>(sql);
            }
            catch (Exception ex) {
                this.Logger.LogInformation(ex.Message);
                return 0;
            }
            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Organization/SaveBasicInfoData";
            foreach (var item in pageData) {
                decimal gengDiMJ = 0;
                sql = @"select sum(landarea) as area from DigitalFarmLand where villageid = {0} and isdeleted = 0";
                sql = string.Format(sql, item.VillageId);
                var row = this.Context.Database.SqlQuery<StatisticsFarmlandDto>(sql);
                if (row != null)
                {
                    gengDiMJ = row[0].Area;
                }

                var content = new
                {
                    organizationCode = item.AreaCode,
                    year,
                    zongRenK = item.Population,
                    zongHuS = item.Tenement,
                    liuDongRK = 0,
                    xingZhengMJ = item.Area,
                    gengDiMJ,
                    sourceId = item.AreaCode + "-" + item.Id
                };

                var result = await PostAsync(url, content);
                if (result.Code == -1) {
                    this.Logger.LogInformation(result.Message);
                }
            }
            return 0;
        }
    }
}
