using DVS.Application.Services.Cons;
using DVS.Common.ModelDtos;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons;
using Grpc.Core;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Grpc
{
    public class ServiceguideGrpcService : ServiceGuideApi.ServiceGuideApiBase
    {
        IServiceGuideService serviceGuideService;
        public ServiceguideGrpcService(IServiceGuideService serviceGuideService)
        {
            this.serviceGuideService = serviceGuideService;
        }

        public async override Task<Reply> GetServiceGuideList(Request request, ServerCallContext context)
        {
            try
            {
                PagedListQueryModel req = getRequestParameter<PagedListQueryModel>(request);

                var list = await serviceGuideService.GetDataList(req);
                var pageResultModel = new PagedResultModel<ListServiceGuideModel>();
                pageResultModel.Total = list.TotalItemCount;
                pageResultModel.Page = list.PageNumber;
                pageResultModel.Pages = list.PageCount;
                pageResultModel.Limit = list.PageSize;
                pageResultModel.Docs = list;
                string data = JsonConvert.SerializeObject(pageResultModel);
                return new Reply() { Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) };
            }
            catch
            {
                return new Reply() { Data = ErrorOutPut() };
            }
        }

        public async override Task<Reply> ServiceGuideDetail(Request request, ServerCallContext context)
        {
            try
            {
                IdRequest req = getRequestParameter<IdRequest>(request);
                var detail = await serviceGuideService.GetDetail(req.Id);
                string data = JsonConvert.SerializeObject(detail);
                return new Reply() { Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) };
            }
            catch
            {
                return new Reply() { Data = ErrorOutPut() };
            }
        }

        public async override Task<Reply> ServiceGuideAdd(Request request, ServerCallContext context)
        {
            try
            {
                CreateServiceGuideModel model = getRequestParameter<CreateServiceGuideModel>(request);
                var result = await serviceGuideService.Save(model);
                string data = JsonConvert.SerializeObject(result);
                return new Reply() { Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) };
            }
            catch
            {
                return new Reply() { Data = ErrorOutPut() };
            }
        }

        public async override Task<Reply> ServiceGuideEdit(Request request, ServerCallContext context)
        {
            try
            {
                CreateServiceGuideModel model = getRequestParameter<CreateServiceGuideModel>(request);
                var result = await serviceGuideService.Save(model);
                string data = JsonConvert.SerializeObject(result);
                return new Reply() { Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) };
            }
            catch
            {
                return new Reply() { Data = ErrorOutPut() };
            }
        }

        public async override Task<Reply> ServiceGuideRemove(Request request, ServerCallContext context)
        {
            try
            {
                IdRequest req = getRequestParameter<IdRequest>(request);

                var result = await serviceGuideService.Remove(req.Id);
                string data = JsonConvert.SerializeObject(result);
                return new Reply() { Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) };
            }
            catch
            {
                return new Reply() { Data = ErrorOutPut() };
            }
        }

        private static T getRequestParameter<T>(Request request)
        {
            string parameters = Encoding.UTF8.GetString(Convert.FromBase64String(request.Parameters));
            T req = JsonConvert.DeserializeObject<T>(parameters);
            return req;
        }

        private string ErrorOutPut()
        {
            var resultModel = new ResultModel<object>();
            resultModel.Code = -1;
            resultModel.Message = "参数无效";
            string data = JsonConvert.SerializeObject(resultModel);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
    }
}
