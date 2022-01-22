using AutoMapper;
using AutoMapper.QueryableExtensions;
using DVS.Application.Services.Common;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.IPS;
using DVS.Models.Dtos.IPS;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.IPS
{
    public class IPSScheduleService : ServiceBase<IpsSchedule>, IIPSScheduleService
    {
        private readonly IBasicUserService basicUserService;
        private readonly ISunFileInfoService sunFileInfoService;
        public IPSScheduleService(DbContext context, IMapper mapper, IBasicUserService basicUserService, ISunFileInfoService sunFileInfoService
              ) : base(context, mapper)
        {
            this.basicUserService = basicUserService;
            this.sunFileInfoService = sunFileInfoService;
        }

        public async Task<int> DeleteSchedule(int id, int userid)
        {
            var schedule = await this.GetAsync(a => a.Id == id);
            schedule.IsDeleted = 1;
            schedule.IsSyncToIPS = 0;
            return await this.UpdateAsync(schedule);
        }

        public async Task<IPSScheduleDto> DetailSchedule(int id)
        {
            var message = await this.GetAsync(a => a.Id == id);
            var result = mapper.Map<IPSScheduleDto>(message);
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
                result.ImageFiles = await this.sunFileInfoService.GetSunFileInfoList(result.Images);
                result.VideoFiles = await this.sunFileInfoService.GetSunFileInfoList(result.Videos);
            }
            return result;
        }

        public async Task<IPagedList<IPSScheduleDto>> ListSchedule(string keyword, int page, int limit, int areaId = 0)
        {
            Expression<Func<IpsSchedule, bool>> expression = a => a.IsDeleted == 0;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                expression = expression.And(a => a.Name.ToLower().Contains(keyword.ToLower()));
            }

            if (areaId > 0)
            {
                expression = expression.And(a => a.AreaId == areaId);
            }            
            var result = await this.GetQueryable().Where(expression).OrderByDescending(a => a.CreatedAt).ProjectTo<IPSScheduleDto>(mapper.ConfigurationProvider).ToPagedListAsync(page, limit);

            List<int> userIds = result.Select(a => a.CreatedBy).ToList().Union(result.Select(a => a.UpdatedBy).ToList()).Distinct().ToList();
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));

            foreach (var item in result)
            {
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);

                item.CreatedByName = user != null ? user.NickName : "";
                item.UpdatedByName = user != null ? user.NickName : "";
            }
            StaticPagedList<IPSScheduleDto> pageList = new StaticPagedList<IPSScheduleDto>(result, page, limit, result.TotalItemCount);
            return pageList;
        }

        public async Task<int> SaveSchedule(IpsSchedule schedule)
        {
            if (string.IsNullOrWhiteSpace(schedule.Name))
                throw new ValidException("名称不能为空");

            if (string.IsNullOrWhiteSpace(schedule.Devices))
                throw new ValidException("发布设备不能为空");

            if (!(schedule.Type == 1 || schedule.Type == 2))
                throw new ValidException("内容类型无效");

            if (schedule.Type == 1 && string.IsNullOrWhiteSpace(schedule.Images))
                throw new ValidException("图片不能为空");
            
            if (schedule.Type == 1 && schedule.Showduration == 0)
                throw new ValidException("图片切换间隔不能为空");
            
            if (schedule.Type == 2 && string.IsNullOrWhiteSpace(schedule.Videos))
                throw new ValidException("视频不能为空");

            if (schedule.Id > 0)
            {
                var data = await this.GetAsync(a => a.Id == schedule.Id && a.IsDeleted == 0);
                if (data == null)
                {
                    throw new ValidException("发布内容不存在");
                }
                data.AuditStatus = 3; // 发布成功, 默认发布成功
                data.DeviceCount = schedule.Devices.Split(",").ToList().Count();
                data.IsSyncToIPS = 0;
                data.Devices = schedule.Devices;
                data.Name = schedule.Name;
                data.Type = schedule.Type;
                data.Showduration = schedule.Showduration;
                data.Images = schedule.Images;
                data.Videos = schedule.Videos;
                data.UpdatedBy = schedule.UpdatedBy;
                return await this.UpdateAsync(data);
            }
            else {
                schedule.AuditStatus = 3; // 发布成功, 默认发布成功
                schedule.DeviceCount = schedule.Devices.Split(",").ToList().Count();
                schedule.AllDay = 1;
                schedule.EndDate = DateTime.Parse(DateTime.Now.ToShortDateString()+ " 00:00:00");
                schedule.EndTime = "23:59";
                schedule.NeverExpire = 1;
                schedule.StartDate = DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:00");
                schedule.StartTime = "00:00";

                var result = await this.InsertAsync(schedule);
                if (result != null) {
                    return result.Id;
                }
                return 0;
            }            
        }

        public async Task<IPSStatisticDto> StatisticSchedule(int areaId)
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
