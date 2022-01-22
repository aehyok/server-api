using AutoMapper;
using DVS.Application.Services.Village;
using DVS.Common.SO;
using DVS.Core.Domains.Village;
using DVS.Models.Const;
using DVS.Models.Dtos.Common;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DVS.Cons.Api.Controllers.Console
{
    /// <summary>
    /// 基础接口
    /// </summary>
    [Route("api/cons/console/system")]
    public class SystemController : DvsControllerBase
    {
        private readonly IMapper mapper;
        // protected readonly ILogger Logger;
        private readonly IPopulationService popuService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="popuService"></param>
        public SystemController(IMapper mapper, IPopulationService popuService)
        {
            this.mapper = mapper;
            this.popuService = popuService;

        }

        /// <summary>
        /// 获取服务的版本
        /// </summary>
        /// <returns></returns>
        [HttpGet("version")]
        public string Version()
        {
            if (System.IO.File.Exists(".version"))
            {
                string versionText = System.IO.File.ReadAllText(".version");
                if (!versionText.IsNullOrWhiteSpace())
                {
                    string[] parts = versionText.Split("=");
                    if (parts.Length > 1)
                    {
                        return parts[1];
                    }
                }
            }
            return "1.0.0.001";
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="service"></param>
        /// <param name="fileName">导入的JSON文件名称 XXXX.json</param>
        /// <returns></returns>
        [HttpPost("ImportData")]
        public async Task<string> ImportData([FromServices] IHouseholdCodeService service, string fileName)
        {
            string jsonfile = Path.Combine(AppContext.BaseDirectory, fileName);
            StreamReader file = System.IO.File.OpenText(jsonfile);
            JsonTextReader reader = new JsonTextReader(file);
            JObject data = (JObject)JToken.ReadFrom(reader);

            var ret = "";
            try
            {
                var jsonObject = JsonConvert.DeserializeObject<ImportModel>(data.ToString());

                var householdList = await  service.GetListAsync(a => a.IsDeleted == 0);

                int i = 0;
                foreach (var d in jsonObject.Data)
                { 
                    var household = householdList.FirstOrDefault(a => a.Remark == d.Address);
                    
                    if (household != null)
                    {
                        var relationship = "0";
                        switch (d.Relationship)
                        {
                            case "户主或本人":
                                relationship = "2";
                                break;
                            case "子女":
                                relationship = "5";
                                break;
                            case "父母":
                                relationship = "4";
                                break;
                            case "配偶":
                                relationship = "3";
                                break;
                        }


                        var indata = new VillagePopulation()
                        {
                            AreaId = household.AreaId,
                            Birthday = Convert.ToDateTime(d.Birthday + " 00:00:00"),
                            IdCard = BasicSO.Encrypt(d.IdCard),
                            RealName = BasicSO.Encrypt(d.RealName),
                            Relationship = relationship,
                            Nation = "39",  
                            Sex = (d.Gender == "男") ? PopulationGender.男 : PopulationGender.女,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now

                        };

                        var res = await popuService.InsertAsync(indata);
                        if (res != null)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            ret = "导入失败" + d.RealName;
                        }

                    }
                }
                ret += "已成功导入" + i + "条记录";
            }
            catch (Exception ex)
            {
                ret = "导入错误!" + ex.Message;
            }

            return ret;

        }
    }

}
