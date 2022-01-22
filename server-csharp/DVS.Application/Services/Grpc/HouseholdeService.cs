using DVS.Application.Services.Common;
using DVS.Application.Services.Village;
using DVS.Common.ModelDtos;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village;
using DVS.Models.Dtos.Village.Query;
using Grpc.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Grpc
{
    public class HouseholdeGrpcService : HouseholdApi.HouseholdApiBase
    {
        IHouseholdCodeService householdCodeService;
        IPopulationService populationService;
        ISunFileInfoService sunfileService;
        public HouseholdeGrpcService(IHouseholdCodeService householdCodeService, IPopulationService populationService, ISunFileInfoService sunFileService)
        {
            this.householdCodeService = householdCodeService;
            this.populationService = populationService;
            this.sunfileService = sunFileService;
        }

        public async override Task<Reply> GetHouseholdList(Request request, ServerCallContext context)
        {
            PostBody req = getRequestParameter<PostBody>(request);

            var householdInfos = await householdCodeService.GetHouseholdCodeList(req);
            var pageResultModel = new PagedResultModel<HouseholdCodeDto>();
            pageResultModel.Total = householdInfos.TotalItemCount;
            pageResultModel.Page = householdInfos.PageNumber;
            pageResultModel.Pages = householdInfos.PageCount;
            pageResultModel.Limit = householdInfos.PageSize;
            pageResultModel.Docs = householdInfos;
            string data = JsonConvert.SerializeObject(pageResultModel);
            return new Reply() { Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) };
        }

        public async override Task<Reply> GetHouseholdInfo(Request request, ServerCallContext context)
        {
            IdRequest req = getRequestParameter<IdRequest>(request);
            VillageHouseholdCode villageHousehold = await householdCodeService.GetHouseholdCodeDetail(req.Id);
            string data = JsonConvert.SerializeObject(new
            {
                Id = villageHousehold.Id,
                AreaId = villageHousehold.AreaId,
                HouseName = villageHousehold.HouseName,
                HouseNumber = villageHousehold.HouseNumber,
                HouseholdMan = villageHousehold.HouseholdMan,
                Mobile = villageHousehold.Mobile
            });
            return new Reply() { Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) };
        }



        public async override Task<Reply> GetHouseholdMembers(Request request, ServerCallContext context)
        {
            IdRequest idRequest = getRequestParameter<IdRequest>(request);
            var householdInfo = await householdCodeService.GetHouseholdCodeDetail(idRequest.Id);
            IEnumerable<PopulationDto> members = (await populationService.GetPopulationList(new Models.Dtos.Village.Query.PopulationListBody() { HouseholdId = householdInfo.Id, AreaId = householdInfo.AreaId, Page = 1, Limit = 1000 }));
            List<PopulationDto> memberInfos = new List<PopulationDto>(members);
            foreach (PopulationDto memberInfo in memberInfos)
            {
                string headImageUrl = sunfileService.ToAbsolutePath(memberInfo.HeadImageUrl);
                memberInfo.HeadImageUrl = headImageUrl;
            }
            string data = JsonConvert.SerializeObject(memberInfos);
            return new Reply() { Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) };
        }

        private static T getRequestParameter<T>(Request request)
        {
            string parameters = Encoding.UTF8.GetString(Convert.FromBase64String(request.Parameters));
            T req = JsonConvert.DeserializeObject<T>(parameters);
            return req;
        }
    }
}
