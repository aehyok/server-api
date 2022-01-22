using DVS.Common.ModelDtos;
using DVS.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DVS.Common.Infrastructures
{
    public class DvsExceptionAttribute : TypeFilterAttribute
    {
        public DvsExceptionAttribute()
            : base(typeof(DvsExceptionFilter))
        { }

        public class DvsExceptionFilter : IAsyncExceptionFilter
        {
            private readonly ILogger logger;

            public DvsExceptionFilter(ILogger<DvsExceptionFilter> logger)
            {
                this.logger = logger;
            }

            public async Task OnExceptionAsync(ExceptionContext context)
            {
                var exception = context.Exception;

                if (exception == null)
                {
                    return;
                }
                if (exception is ValidException)
                {
                    ValidException validException = exception as ValidException;
                    var resultModel = new ResultModel<object>();
                    resultModel.Code = validException.ExceptionCode;
                    resultModel.Message = validException.Message;
                    resultModel.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    resultModel.Data = null;
                    context.Result = new DvsResult(resultModel);
                    context.ExceptionHandled = true;
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    var resultModel = new ResultModel<object>();
                    resultModel.Code = (int)HttpStatusCode.InternalServerError;
                    resultModel.Message = exception.Message;
                    resultModel.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    resultModel.Data = exception.StackTrace;
                    this.logger.LogError(exception, exception.Message);
                    context.Result = new DvsResult(resultModel);
                    context.ExceptionHandled = true;
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                await Task.CompletedTask;
            }
        }
    }
}