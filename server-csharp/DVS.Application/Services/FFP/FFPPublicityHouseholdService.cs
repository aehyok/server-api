using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Services;
using DVS.Core.Domains.FFP;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Application.Services.FFP
{
    public class FFPPublicityHouseholdService : ServiceBase<FFPPublicityHousehold>, IFFPPublicityHouseholdService
    {

        private readonly IBasicUserService basicUserService;
        public FFPPublicityHouseholdService(DbContext dbContext, IMapper mapper, IBasicUserService basicUserService)
        : base(dbContext, mapper)
        {
            this.basicUserService = basicUserService;
        }
    }
}
