using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.Cons;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using LinqKit;
using DVS.Common;
using DVS.Models.Enum;
using DVS.Common.RPC;
using DVS.Models.Dtos.RPC;
using DVS.Common.Models;

namespace DVS.Application.Services.Cons
{
    public class ServiceGuideService : ServiceBase<ConsServiceGuide>, IServiceGuideService
    {
        readonly ISunFileInfoService fileService;
        readonly IBasicUserService basicUserService;
        public ServiceGuideService(DbContext dbContext, IMapper mapper, ISunFileInfoService sunfileservice, IBasicUserService _basicUserService)
            : base(dbContext, mapper)
        {
            this.fileService = sunfileservice;
            this.basicUserService = _basicUserService;
        }


        public async Task<IPagedList<ListServiceGuideModel>> GetDataList(PagedListQueryModel model)
        {
            Expression<Func<ConsServiceGuide, bool>> filter = a => a.IsDeleted == 0;
            Expression<Func<ConsServiceGuide, object>> orderby = a => a.CreatedAt;

            if (!model.Keyword.IsNullOrEmpty())
            {
                filter = filter.And(a => a.Title.ToLower().Contains(model.Keyword));
            }
            var data = await this.GetPagedListAsync(filter, orderBy: orderby, model.Page, model.Limit, asc: false);
            // var data = await this.GetPagedListAsync(filter, model.Page, model.Limit);

            List<int> userIds = new List<int>();
            string imageIds = "";
            foreach (var item in data)
            {
                int userid = (int)item.CreatedBy;
                if (!(userIds.IndexOf(userid) >= 0))
                {
                    userIds.Add(userid);
                }

                userid = (int)item.UpdatedBy;
                if (!(userIds.IndexOf(userid) >= 0))
                {
                    userIds.Add(userid);
                }

                if (!string.IsNullOrEmpty(item.Icon))
                {
                    imageIds = imageIds + "," + item.Icon;
                }

                if (!string.IsNullOrEmpty(item.StepImages))
                {
                    imageIds = imageIds + "," + item.StepImages;
                }

            }
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            var fileList = await this.fileService.GetSunFileInfoList(imageIds);
            var list = new List<ListServiceGuideModel>();
            foreach (var m in data)
            {
                var r = this.mapper.Map<ListServiceGuideModel>(m);
                var user = userList.FirstOrDefault(a => a.Id == m.CreatedBy);
                r.CreatedByName = user != null ? user.NickName : "";

                user = userList.FirstOrDefault(a => a.Id == m.UpdatedBy);
                r.UpdatedByName = user != null ? user.NickName : "";

                var files = fileList.FindAll(a => ("," + m.Icon + ",").Contains("," + a.Id + ","));
                r.IconFiles = files;

                files = fileList.FindAll(a => ("," + m.StepImages + ",").Contains("," + a.Id + ","));
                r.StepImageFiles = files;

                list.Add(r);
            }

            return new StaticPagedList<ListServiceGuideModel>(list, model.Page, model.Limit, data.TotalItemCount);
        }


        public async Task<DetailServiceGuideModel> GetDetail(int id,PlatFormCode platFormCode=PlatFormCode.WECHAT,LoginUser loginUser=null)
        {
            var data = await this.GetAsync(a => a.Id == id);
            var ret = mapper.Map<DetailServiceGuideModel>(data);
            var files = await this.fileService.GetSunFileInfoList(data.Icon);
            ret.IconFiles = files;

            files = await this.fileService.GetSunFileInfoList(data.StepImages);
            ret.StepImageFiles = files;

            var cUser = await this.basicUserService.GetAsync(a => a.Id == data.CreatedBy);
            if (cUser != null)
            {
                ret.CreatedByName = cUser.NickName;
            }
            var uUser = await this.basicUserService.GetAsync(a => a.Id == data.UpdatedBy);
            if (uUser != null)
            {
                ret.UpdatedByName = uUser.NickName;
            }
            if (!string.IsNullOrEmpty(ret.Url))
            {
                var qrCode = Utils.GetQRCodeImage(ret.Url);
                if (!string.IsNullOrEmpty(qrCode))
                {
                    ret.QrCode = "data:image/jpg;base64," + qrCode;
                }
            }


            if (platFormCode == PlatFormCode.WECHAT)
            {
                BasicRPC.AllotScore(new IntegralReq()
                {
                    IntegralAction = IntegralAction.ServiceChannel,
                    Description = "浏览便民查询",
                    HouseholdId = loginUser.HouseholdId,
                    UserId = loginUser.UserId
                });
            }

            return ret;
        }


        public async Task<int> Save(CreateServiceGuideModel model)
        {
            var data = mapper.Map<ConsServiceGuide>(model);
            if (model.Id == 0)
            {
                data = await this.InsertAsync(data);
                return data.Id;
            }
            else
            {
                var info = await this.GetAsync(a => a.Id == model.Id);
                if (info == null)
                {
                    throw new Exception("数据不存在");
                }
                info.Title = data.Title;
                info.Condition = data.Condition;
                info.Material = data.Material;
                info.Description = data.Description;
                info.Step = data.Step;
                info.StepImages = data.StepImages;
                info.Icon = data.Icon;
                info.Url = data.Url;
                info.UpdatedBy = data.UpdatedBy;
                int res = await this.UpdateAsync(info);
                if (res > 0)
                {
                    return data.Id;
                }
                return res;
            }
        }

        public async Task<int> Remove(int id)
        {
            return await this.GetQueryable().Where(a => a.Id == id).UpdateFromQueryAsync(a => new ConsServiceGuide
            {
                IsDeleted = 1
            });
        }

    }
}
