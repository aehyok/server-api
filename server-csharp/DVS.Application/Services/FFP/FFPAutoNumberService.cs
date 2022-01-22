using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.FFP;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace DVS.Application.Services.FFP
{
   

    public class FFPAutoNumberService : ServiceBase<FFPAutoNumber>, IFFPAutoNumberService
    {

       
        public FFPAutoNumberService(DbContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
        {
           
        }

        public async Task<string> GetWorkflowAutoNumber(string prefix = "T")
        {
            var data = new FFPAutoNumber() { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, UpdatedBy = 0, CreatedBy = 0, IsDeleted = 0 };
            var res = await this.InsertAsync(data);
            if (res != null) {
                return prefix + res.Id.ToString();
            }
            return "";
        }
    }
}
