using AutoMapper;
using AutoMapper.QueryableExtensions;
using DVS.Application.Services.Common;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.IPS;
using DVS.Models.Dtos.IPS;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.IPS
{
    public class IPSMessageService : ServiceBase<IpsMessage>, IIPSMessageService
    {
        private readonly IConfiguration configuration;
        readonly IBasicUserService basicUserService;
        public IPSMessageService(DbContext context, IMapper mapper,IConfiguration configuration, IBasicUserService basicUserService
              ) : base(context, mapper)
        {
            this.configuration = configuration;
            this.basicUserService = basicUserService;
        }

        public async Task<int> DeleteMessage(int id, int userid)
        {
            var message = await this.GetAsync(a => a.Id == id);
            message.IsDeleted = 1;
            message.IsSyncToIPS = 0;
            return await this.UpdateAsync(message);
        }

        public async Task<IPSMessageDto> DetailMessage(int id)
        {
            var message = await this.GetAsync(a => a.Id == id);
            var result = mapper.Map<IPSMessageDto>(message);
            if (result != null) {
                var user = await this.basicUserService.GetAsync(a => a.Id == result.CreatedBy);
                if (user != null)
                {
                    result.CreatedByName = user.NickName;
                }
                user = await this.basicUserService.GetAsync(a => a.Id == result.UpdatedBy);
                if (user != null)
                {
                    result.UpdatedByName = user.NickName;
                }
            }
            return result;
        }

        public async Task<IPagedList<IPSMessageDto>> ListMessage(string keyword, int page, int limit, int areaId = 0)
        {
            Expression<Func<IpsMessage, bool>> expression = a => a.IsDeleted == 0;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                expression = expression.And(a => a.Name.ToLower().Contains(keyword.ToLower()));
            }

            if (areaId > 0)
            {
                expression = expression.And(a => a.AreaId == areaId);
            }            
            var result = await this.GetQueryable().Where(expression).OrderByDescending(a => a.CreatedAt).ProjectTo<IPSMessageDto>(mapper.ConfigurationProvider).ToPagedListAsync(page, limit);

            List<int> userIds = result.Select(a => a.CreatedBy).ToList().Union(result.Select(a => a.UpdatedBy).ToList()).Distinct().ToList();
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));

            foreach (var item in result)
            {
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);

                item.CreatedByName = user != null ? user.NickName : "";
                item.UpdatedByName = user != null ? user.NickName : "";
            }
            StaticPagedList<IPSMessageDto> pageList = new StaticPagedList<IPSMessageDto>(result, page, limit, result.TotalItemCount);
            return pageList;
        }

        public async Task<int> SaveMessage(IpsMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Name))
                throw new ValidException("参数无效");

            if (string.IsNullOrWhiteSpace(message.Content))
                throw new ValidException("参数无效");

            if (string.IsNullOrWhiteSpace(message.Devices))
                throw new ValidException("参数无效");

            if (message.Id > 0)
            {
                var data = await this.GetAsync(a => a.Id == message.Id && a.IsDeleted == 0);
                if (data == null) {
                    throw new ValidException("消息不存在");
                }
                data.AuditStatus = 3; // 发布成功, 默认发布成功
                data.DeviceCount = message.Devices.Split(",").ToList().Count();
                data.IsSyncToIPS = 0;
                data.Content = message.Content;
                data.Name = message.Name;
                data.Devices = message.Devices;
                data.UpdatedBy = message.UpdatedBy;
                return await this.UpdateAsync(data);
            }
            else {
                message.AuditStatus = 3; // 发布成功, 默认发布成功
                message.DeviceCount = message.Devices.Split(",").ToList().Count();
                message.AllDay = 1;
                message.EndDate = DateTime.Parse(DateTime.Now.ToShortDateString()+ " 00:00:00");
                message.EndTime = "23:59";
                message.NeverExpire = 1;
                message.StartDate = DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:00");
                message.StartTime = "00:00";
                var result = await this.InsertAsync(message);
                if (result != null) {
                    return result.Id;
                }
                return 0;
            }
            
        }

        public async Task<IPSStatisticDto> StatisticMessage(int areaId)
        {
            var passed = await this.CountAsync(a => a.AreaId == areaId && a.IsDeleted == 0 && a.AuditStatus == 3);
            var passing = await this.CountAsync(a => a.AreaId == areaId && a.IsDeleted == 0 && a.AuditStatus == 2);
            var deny = await this.CountAsync(a => a.AreaId == areaId && a.IsDeleted == 0 && a.AuditStatus == 4);

            return new IPSStatisticDto()
            {
                Passed = passed,
                Passing = passing,
                Deny = deny
            };
        }
    }
}
