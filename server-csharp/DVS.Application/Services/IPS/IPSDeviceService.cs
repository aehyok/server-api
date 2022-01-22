using AutoMapper;
using DVS.Common.Services;
using DVS.Core.Domains.IPS;
using DVS.Models.Dtos.IPS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqKit;
using DVS.Application.Services.Common;

namespace DVS.Application.Services.IPS
{
    public class IPSDeviceService : ServiceBase<IpsDevice>, IIPSDeviceService
    {
        readonly IIPSCompanyService companyService;
        readonly IBasicAreaService basicAreaService;
        public IPSDeviceService(DbContext context, IMapper mapper, IIPSCompanyService companyService, IBasicAreaService basicAreaService
              ) : base(context, mapper)
        {
            this.companyService = companyService;
            this.basicAreaService = basicAreaService;
        }

        public async Task<int> SaveDevice(IpsDevice data)
        {
            var model = await this.GetAsync(a => a.DeviceId == data.DeviceId);
            if (model == null)
            {
                var res = await this.InsertAsync(data);

                if (res != null)
                {
                    return res.Id;
                }
                return 0;
            }
            else
            {
                int res = await this.GetQueryable().Where(a => a.DeviceId == model.DeviceId).UpdateFromQueryAsync(a => new IpsDevice()
                {
                    DeviceId = data.DeviceId,
                    DeviceNo = data.DeviceNo,
                    DeviceName = data.DeviceName,
                    Category = data.Category,
                    Remark = data.Remark,
                    Status = data.Status,
                    CompanyId = data.CompanyId,
                });
                if (res > 0)
                    return data.Id;

                return 0;
            }
        }

        public async Task<IPSDeviceTreeDto> List(int areaId)
        {
            var companyId = "";
            var area = await this.basicAreaService.GetAsync(a => a.Id == areaId);
            if (area != null) {
                companyId = area.IpsCompanyId;
            }
            var parentNode = new IPSDeviceTreeDto();
            Expression<Func<IpsCompany, bool>> expression = a => a.CompanyId == companyId && a.Status == 1 && a.IsDeleted == 0;
            var company = await this.companyService.GetAsync(expression);
            if (company == null)
                return parentNode;

            //parentNode.Id = company.Id;
            parentNode.CompanyId = company.CompanyId;
            parentNode.CompanyName = company.CompanyName;
            parentNode.CompanyNo = company.CompanyNo;
            parentNode.ParentId = company.ParentId;
            parentNode.Category = company.Category;
            if (company.Category == 2)
            { //门店
                var devices = await this.GetListAsync(a => a.CompanyId == company.CompanyId && a.IsDeleted == 0 && a.Category == 0 && a.Status == 1);
                if (devices != null)
                    parentNode.Devices = mapper.Map<List<IPSDeviceDto>>(devices);
            }
            await FindChildrenNodesAsync(parentNode, company.CompanyId);
            return parentNode;
        }

        private async Task FindChildrenNodesAsync(IPSDeviceTreeDto parentNode, string parentId)
        {
            var companys = await this.companyService.GetListAsync(a => a.ParentId == parentId && a.Status == 1 && a.IsDeleted == 0);
            if (companys == null)
                return;

            foreach (var company in companys)
            {
                var newnode = new IPSDeviceTreeDto();
                //newnode.Id = company.Id;
                newnode.CompanyId = company.CompanyId;
                newnode.CompanyName = company.CompanyName;
                newnode.CompanyNo = company.CompanyNo;
                newnode.ParentId = company.ParentId;
                newnode.Category = company.Category;
                if (company.Category == 2)
                { //门店
                    var devices = await this.GetListAsync(a => a.CompanyId == company.CompanyId && a.IsDeleted == 0 && a.Category == 0 && a.Status == 1);
                    if (devices != null)
                        newnode.Devices = mapper.Map<List<IPSDeviceDto>>(devices);
                }
                parentNode.Children.Add(newnode);
                await FindChildrenNodesAsync(newnode, newnode.CompanyId);
            }
        }
    }
}
