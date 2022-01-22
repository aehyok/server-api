using DVS.Common.Services;
using DVS.Core.Domains.FFP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.FFP
{
    public interface IFFPAutoNumberService : IServiceBase<FFPAutoNumber>
    {


        Task<string> GetWorkflowAutoNumber(string prefix = "T");
       

    }
}
