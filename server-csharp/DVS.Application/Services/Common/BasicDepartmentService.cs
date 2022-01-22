using AutoMapper;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
    public class BasicDepartmentService : ServiceBase<BasicDepartment>, IBasicDepartmentService
    {

        public BasicDepartmentService(DbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {

        }

        public override async Task<IList<BasicDepartment>> GetListAsync(Expression<Func<BasicDepartment, bool>> predicate)
        {
            var departList = await base.GetListAsync(predicate);
            return departList;
        }

        public override async Task<BasicDepartment> GetAsync(Expression<Func<BasicDepartment, bool>> predicate)
        {
            var depart = await base.GetAsync(predicate);
            return depart;
        }
    }
}
