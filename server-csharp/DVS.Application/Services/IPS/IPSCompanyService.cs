using AutoMapper;
using DVS.Common.Services;
using DVS.Core.Domains.IPS;
using DVS.Models.Dtos.IPS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DVS.Application.Services.IPS
{
    public class IPSCompanyService : ServiceBase<IpsCompany>, IIPSCompanyService
    {
        private readonly IConfiguration configuration;
        public IPSCompanyService(DbContext context, IMapper mapper,IConfiguration configuration
              ) : base(context, mapper)
        {
            this.configuration = configuration;
        }

        public async Task<int> SaveCompany(IpsCompany data)
        {
            var model = await this.GetAsync(a => a.CompanyId == data.CompanyId);
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
                int res = await this.GetQueryable().Where(a => a.CompanyId == model.CompanyId).UpdateFromQueryAsync(a => new IpsCompany()
                {
                    CompanyNo = data.CompanyNo,
                    TopCompanyId = data.CompanyNo,
                    Category = data.Category,
                    CompanyName = data.CompanyName,
                    ShortName = data.ShortName,
                    Remark = data.Remark,
                    Status = data.Status,
                    ParentId = data.ParentId,
                });
                if (res > 0)
                    return data.Id;

                return 0;
            }
        }

        public async Task<IPSCompanyTreeDto> GetCompanyNodesTree(string companyId = "")
        {
            if (string.IsNullOrWhiteSpace(companyId))
                companyId = configuration.GetValue<string>("IPS:CompanyId");

            var parentNode = new IPSCompanyTreeDto();
            Expression<Func<IpsCompany, bool>> expression = a => a.CompanyId == companyId && a.Status == 1 && a.IsDeleted == 0;
            var company = await this.GetAsync(expression);
            if (company == null)
                return parentNode;

            parentNode.CompanyId = company.CompanyId;
            parentNode.CompanyNo = company.CompanyNo;
            parentNode.CompanyName = company.CompanyName;
            parentNode.ParentId = company.ParentId;
            parentNode.Category = company.Category;
            await FindChildrenNodesAsync(parentNode, company.CompanyId);
            return parentNode;
        }

        private async Task FindChildrenNodesAsync(IPSCompanyTreeDto parentNode, string parentId)
        {
            var companys = await this.GetListAsync(a => a.ParentId == parentId && a.Status == 1 && a.IsDeleted == 0);
            if (companys == null)
                return;

            foreach (var company in companys)
            {
                var newnode = new IPSCompanyTreeDto();
                newnode.CompanyId = company.CompanyId;
                newnode.CompanyName = company.CompanyName;
                newnode.CompanyNo = company.CompanyNo;
                newnode.ParentId = company.ParentId;
                newnode.Category = company.Category;
                parentNode.Children.Add(newnode);
                await FindChildrenNodesAsync(newnode, newnode.CompanyId);
            }
        }
    }
}
