using AutoMapper;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;

namespace DVS.Application.Services.Common
{
    public class ParkAreaService : ServiceBase<ParkArea>, IParkAreaService
    {

        public ParkAreaService(DbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {

        }
    }
}
