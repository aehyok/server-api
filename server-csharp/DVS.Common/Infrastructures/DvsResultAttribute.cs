using DVS.Common.ModelDtos;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Common.Infrastructures
{
    public class DvsResultAttribute : TypeFilterAttribute
    {
        public DvsResultAttribute()
            : base(typeof(DvsResultFilter))
        {
        }

        public class DvsResultFilter : IAsyncResultFilter
        {
            private readonly ILogger logger;

            public DvsResultFilter(ILogger<DvsResultFilter> logger)
            {
                this.logger = logger;
            }

            public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
            {
                this.logger.LogInformation("执行 VisualizationResultFilter");
                if (!context.ModelState.IsValid)
                {
                    var resultModel = new ResultModel<object>();
                    resultModel.Code = 200;
                    var errorInfo = (await context.ModelState.Values.ToListAsync()).FirstOrDefault(err => err.Errors != null && err.Errors.Count > 0);
                    resultModel.Message = (errorInfo != null && errorInfo.Errors != null && errorInfo.Errors.Count > 0) ? errorInfo.Errors[0].ErrorMessage : "参数错误";
                    resultModel.Data = null;
                    resultModel.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    context.Result = new DvsResult(resultModel);
                }
                if (context.Result is ObjectResult result)
                {
                    if (!(result.Value is DvsResult))
                    {
                        if (result.Value is IPagedList pagedList)
                        {
                            var resultModel = new ResultModel<object>();
                            var pageResultModel = new PagedResultModel<object>();

                            pageResultModel.Total = pagedList.TotalItemCount;
                            pageResultModel.Page = pagedList.PageNumber;
                            pageResultModel.Pages = pagedList.PageCount;
                            pageResultModel.Limit = pagedList.PageSize;
                            pageResultModel.Docs = result.Value as IEnumerable<object>;
                            resultModel.Code = context.HttpContext.Response.StatusCode;
                            resultModel.Message = "Success";
                            resultModel.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            resultModel.Data = pageResultModel;

                            context.Result = new DvsResult(resultModel);
                        }
                        else if (result.Value is ValidationProblemDetails detail)
                        {
                            var resultModel = new ResultModel<object>();
                            resultModel.Code = (int)CustomExceptionCode.ParamValidError;
                            resultModel.Message = "参数错误";
                            resultModel.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            resultModel.Data = detail.Errors;
                            context.Result = new DvsResult(resultModel);
                        }
                        else
                        {
                            var resultModel = new ResultModel<object>();
                            resultModel.Code = context.HttpContext.Response.StatusCode;
                            resultModel.Message = "Success";
                            resultModel.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            resultModel.Data = result.Value;
                            context.Result = new DvsResult(resultModel);
                        }
                    }
                }
                else if (context.Result is StatusCodeResult statusCodeResult)
                {
                    var resultModel = new ResultModel<object>();
                    resultModel.Code = statusCodeResult.StatusCode;
                    resultModel.Message = statusCodeResult.StatusCode == 200 ? "Success" : "请求发生错误";
                    resultModel.Data = statusCodeResult.StatusCode == 200;
                    resultModel.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    context.Result = new DvsResult(resultModel);
                }
                else
                {
                    await next();
                }

                await next();
            }
        }
    }
}