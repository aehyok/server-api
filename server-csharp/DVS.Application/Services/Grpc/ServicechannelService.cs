using DVS.Application.Services.Cons;
using DVS.Common.ModelDtos;
using DVS.Common.Models;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons;
using Grpc.Core;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Grpc
{
    public class ServicechannelGrpcService : ServiceChannelApi.ServiceChannelApiBase
    {
        IServiceChannelService serviceChannelService;
        public ServicechannelGrpcService(IServiceChannelService serviceChannelService)
        {
            this.serviceChannelService = serviceChannelService;
        }

        public async override Task<Reply> GetServiceChannelList(Request request, ServerCallContext context)
        {
            try
            {
                PagedListQueryModel req = getRequestParameter<PagedListQueryModel>(request);
                var list = await serviceChannelService.GetDataList(req);
                var pageResultModel = new PagedResultModel<ListServiceChannelModel>();
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

        public async override Task<Reply> ServiceChannelDetail(Request request, ServerCallContext context)
        {
            try
            {
                IdRequest req = getRequestParameter<IdRequest>(request);
                var detail = await serviceChannelService.GetDetail(req.Id);
                string data = JsonConvert.SerializeObject(detail);
                return new Reply() { Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) };
            }
            catch {
                return new Reply() { Data = ErrorOutPut() };
            }
        }

        public async override Task<Reply> ServiceChannelAdd(Request request, ServerCallContext context)
        {
            try
            {
                CreateServiceChannelModel model = getRequestParameter<CreateServiceChannelModel>(request);
                var result = await serviceChannelService.Save(model);
                string data = JsonConvert.SerializeObject(result);
                return new Reply() { Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) };
            }
            catch
            {
                return new Reply() { Data = ErrorOutPut() };
            }
        }

        public async override Task<Reply> ServiceChannelEdit(Request request, ServerCallContext context)
        {
            try
            {
                CreateServiceChannelModel model = getRequestParameter<CreateServiceChannelModel>(request);
                var result = await serviceChannelService.Save(model);
                string data = JsonConvert.SerializeObject(result);
                return new Reply() { Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) };
            }
            catch
            {
                return new Reply() { Data = ErrorOutPut() };
            }
        }

        public async override Task<Reply> ServiceChannelRemove(Request request, ServerCallContext context)
        {
            try
            {
                IdRequest req = getRequestParameter<IdRequest>(request);

                var result = await serviceChannelService.Remove(req.Id);
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

        private string ErrorOutPut() {
            var resultModel = new ResultModel<object>();
            resultModel.Code = -1;
            resultModel.Message = "参数无效";
            string data = JsonConvert.SerializeObject(resultModel);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
    }
}
