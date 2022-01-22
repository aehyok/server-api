using DVS.Common.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Common.Infrastructures
{
    public class ResourceFilter : IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            try
            {
                await next();
            }
            catch (Exception ex) {
                throw new ValidException(ex.Message);
            }
        }
    }
}
