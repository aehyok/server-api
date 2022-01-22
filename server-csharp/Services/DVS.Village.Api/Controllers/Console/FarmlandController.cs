using DVS.Application.Services.Village;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Village.Farmland;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Village.Api.Controllers.Console
{
    [Route("api/village/console/farmland/")]
    public class FarmlandController : DvsControllerBase
    {
        [HttpPost("remove")]
        [PermissionFilter(ModuleCodes.土地信息管理, PermissionCodes.Remove)]
        public async Task<bool> Remove([FromServices] IVillageFarmlandService service, [FromBody] RemoveReq req)
        {
            return await service.Remove(req.Ids);
        }
        [HttpPost("list")]
        [PermissionFilter(ModuleCodes.土地信息管理, PermissionCodes.View)]
        public async Task<IPagedList<VillageFarmlandDto>> GetFarmlands([FromServices] IVillageFarmlandService service, [FromBody] GetFarmlandsReq req)
        {
            var farmlands = await service.GetFarmlands(req.AreaId == 0 ? this.LoginUser.AreaId : req.AreaId, req.TypeId, req.Keyword, req.HouseholdId, 1, 1, req.Page, req.Limit, req.OrderBy);
            return farmlands;
        }

        [HttpPost("houseHoldeFarmlandlist")]
        [PermissionFilter(ModuleCodes.土地信息管理, PermissionCodes.View)]
        public async Task<IPagedList<FarmlandAreaSummaryDto>> HouseHoldeFarmlandlist([FromServices] IVillageFarmlandService service, [FromBody] GetFarmlandsReq req)
        {
            var farmlands = await service.GetAreaFarmlands(req.AreaId == 0 ? this.LoginUser.AreaId : req.AreaId, req.Keyword, req.Page, req.Limit, req.OrderBy);
            return farmlands;
        }


        //[HttpPost("save")]
        //public async Task<int> Save([FromServices] IVillageFarmlandService service, [FromBody] VillageFarmland villageFarmland)
        //{
        //    return await service.Save(villageFarmland, this.LoginUser.UserId);
        //}

        [HttpPost("add")]
        [PermissionFilter(ModuleCodes.土地信息管理, PermissionCodes.Add)]
        public async Task<int> Add([FromServices] IVillageFarmlandService service, [FromBody] VillageFarmland villageFarmland)
        {
            if (villageFarmland.Id > 0)
            {
                throw new ValidException("参数无效{id}");
            }
            return await service.Save(villageFarmland, this.LoginUser.UserId);
        }

        [HttpPost("edit")]
        [PermissionFilter(ModuleCodes.土地信息管理, PermissionCodes.Edit)]
        public async Task<int> Edit([FromServices] IVillageFarmlandService service, [FromBody] VillageFarmland villageFarmland)
        {
            if (villageFarmland.Id > 0)
            {
                return await service.Save(villageFarmland, this.LoginUser.UserId);
            }
            else
            {
                throw new ValidException("参数无效{id}");
            }
        }

        [HttpPost("detail")]
        [PermissionFilter(ModuleCodes.土地信息管理, PermissionCodes.View)]
        public async Task<VillageFarmlandDto> Detail([FromServices] IVillageFarmlandService service, [FromBody] DetailReq req)
        {
            return await service.GetDetail(req.Id);
        }

        [HttpGet("export")]
        [PermissionFilter(ModuleCodes.土地信息管理, PermissionCodes.Export)]
        public async Task<FileContentResult> Export([FromServices] IVillageFarmlandService service, [FromQuery] string ids, [FromQuery] int areaId, [FromQuery] string keyword = "")
        {
            List<int> farmlandIds = null;
            if (ids != null)
            {
                farmlandIds = ids.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id => Convert.ToInt32(id)).ToList();
            }
            byte[] farmlandBytes = await service.GetExcelData(farmlandIds, areaId, keyword);
            return File(farmlandBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }


    }
}
