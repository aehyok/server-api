using AutoMapper;
using AutoMapper.QueryableExtensions;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Cons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;
using LinqKit;
using DVS.Core.Domains.Common;
using DVS.Application.Services.Common;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons;
using DVS.Models.Enum;
using DVS.Common.RPC;
using DVS.Models.Dtos.RPC;

namespace DVS.Application.Services.Cons
{
    public class InfoPublicService : ServiceBase<ConsInfoPublic>, IInfoPublicService
    {
        readonly IBasicUserService basicUserService;
        readonly IBasicDepartmentService basicDepartService;
        readonly ISunFileInfoService fileService;
        readonly IBasicAreaService basicAreaService;

        public InfoPublicService(DbContext dbContext, IMapper mapper, ISunFileInfoService sunfileservice, IBasicUserService basicUserService, IBasicDepartmentService basicDepartService, IBasicAreaService basicAreaService)
            : base(dbContext, mapper)
        {
            this.basicUserService = basicUserService;
            this.basicDepartService = basicDepartService;
            this.fileService = sunfileservice;
            this.basicAreaService = basicAreaService;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<int> DeleteConsInfoPublic(int id, int userid)
        {
            ConsInfoPublic entity = await this.GetAsync(a => a.Id == id);
            if (entity == null)
            {
                throw new ValidException("数据不存在");
            }
            entity.IsDeleted = 1;
            entity.UpdatedBy = userid;
            return await this.UpdateAsync(entity);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keyword"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="userid"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IPagedList<ConsInfoPublicDto>> ListConsInfoPublic(int type, string keyword, string startdate, string enddate, int userid, int page, int limit, int areaId = 0, PlatFormCode platformcode = PlatFormCode.UNKNOWN)
        {
            if (!(type== (int)InfoPublicType.ALL || type == (int)InfoPublicType.THREE_AFFAIRS || type == (int)InfoPublicType.PARTY_BUILDING || type == (int)InfoPublicType.SPIRITUAL_CIVILIZATION || type == (int)InfoPublicType.CONVENIENT_SERVICE))
                throw new ValidException("参数无效");

            if (!(page > 0 || limit > 0))
                throw new ValidException("参数无效");

            var sqlWhere = "";
            Expression<Func<ConsInfoPublic, bool>> expression = a => a.IsDeleted == 0;
            if (type != 0)
            {
                expression = expression.And(a => a.Type == type);
                sqlWhere += " and type = " + type;
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                expression = expression.And(a => a.MessageName.ToLower().Contains(keyword.ToLower()));
                sqlWhere += " and messageName  like '%" + keyword + "%'";
            }

            if (!string.IsNullOrWhiteSpace(startdate) && !string.IsNullOrWhiteSpace(enddate))
            {
                expression = expression.And(a => a.CreatedAt >= Convert.ToDateTime(startdate + " 00:00:00") && a.CreatedAt <= Convert.ToDateTime(enddate + " 23:59:59"));
                sqlWhere += " and createdAt >= '" + startdate + " 00:00:00" + "' and createdAt <= '"+ enddate + " 23:59:59"+"'";
            }

            if (areaId > 0)
            {
                expression = expression.And(a => a.AreaId == areaId);
                sqlWhere += " and areaId = " + areaId;
            }
            var basicuser = await this.basicUserService.GetUserInfo(userid);
            if (basicuser == null || (basicuser.Type == 1 && basicuser.IsAuth != UserAuthAuditStatusEnum.Passed))
            {
                // 游客只能看政务部门发的内容, 未认证村民：信息公开仅展示所有政务账号发布的信息公开内容
                expression = expression.And(a => a.CreatedByDeptId > 0);
                sqlWhere += " and createdByDeptId > 0";
            }
            else
            {
                if (basicuser.Type == 3)
                {
                    // 政务用户根据角色来查看数据
                    var dataaccess = basicuser.DataAcces;
                    dataaccess = "all"; // 暂时不处理政务用户权限
                    if (dataaccess == "self")
                    {
                        expression = expression.And(a => a.CreatedBy == userid);
                    }
                    else if (dataaccess == "group")
                    {
                        expression = expression.And(a => a.CreatedByDeptId == int.Parse(basicuser.DepartmentIds));
                    }
                }
                else if (basicuser.Type == 1) 
                {
                    //公众号认证村民：1、信息公开展示当前用户openid所属区域以及对应的同一层级政务账户发布的信息公开内容；
                    //          2、若切换户时存在不同村的情况，则展示当前用户openid所属区域（切换后的）以及对应的同一层级政务账户发布的信息公开内容；
                    expression = expression.And(a => a.AreaId == basicuser.AreaId);
                    sqlWhere += " and areaId = " + basicuser.AreaId;
                }
                else
                {
                    // 查看本级区域以及下级区域
                    var tree = await this.basicAreaService.GetBasicAreaTree(basicuser.AreaId);
                    expression = expression.And(a => tree.Ids.Contains(a.AreaId));
                    sqlWhere += " and areaId in (" + string.Join(',', tree.Ids) + ")";
                }
            }

            StaticPagedList<ConsInfoPublicDto> result = null;
            var dt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");
            if (platformcode == PlatFormCode.WECHAT || platformcode == PlatFormCode.APP)
            {
                // 党建宣传、精神文明、便民信息，微信、APP、园区APP显示顶置内容
                var dtstr = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                var sql = "select * from ((select *, now() as orderbydate,updatedAt as orderby from ConsInfoPublic where isDeleted = 0 " + sqlWhere + " and pinTopExpireAt >= '"+ dtstr + "' order by updatedAt desc) ";
                sql += " union all (select *,createdAt as orderbydate,createdAt as orderby from ConsInfoPublic where IsDeleted = 0 " + sqlWhere + " and pinTopExpireAt <'" + dtstr + "' order by createdAt desc)) a order by orderbydate desc, orderby desc  ";
                var pagedata = this.Context.Database.SqlQueryPagedList<ConsInfoPublicDto>(page, limit, sql, "");

                result = new StaticPagedList<ConsInfoPublicDto>(pagedata, page, limit, pagedata.TotalItemCount);
            }
            else
            {
                var result_all = await this.GetQueryable().Where(expression).OrderByDescending(a => a.CreatedAt).ProjectTo<ConsInfoPublicDto>(mapper.ConfigurationProvider).ToPagedListAsync(page, limit);

                result = new StaticPagedList<ConsInfoPublicDto>(result_all, page, limit, result_all.TotalItemCount);
            }

            List<int> deptIds = result.Select(a => a.CreatedByDeptId).Distinct().ToList();
            List<int> userIds = result.Select(a => a.CreatedBy).ToList().Union(result.Select(a => a.UpdatedBy).ToList()).Distinct().ToList();
            List<string> imagelist = result.Where(a => a.Images.IsNullOrWhiteSpace() == false).Select(a => a.Images).Distinct().ToList();
            string imageIds = string.Join(",", imagelist);

            var deptList = await this.basicDepartService.GetListAsync(a => deptIds.Contains(a.Id));
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            var fileList = await this.fileService.GetSunFileInfoList(imageIds);

            foreach (var item in result)
            {
                var dept = deptList.FirstOrDefault(a => a.Id == item.CreatedByDeptId);
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
                var fileInfo = fileList.FindAll(a => (","+item.Images+",").Contains("," + a.Id + ","));

                item.CreatedByDeptName = dept != null ? dept.Name : await this.basicAreaService.GetAreaName(item.AreaId);
                item.CreatedByName = user != null ? user.NickName : "";
                item.UpdatedByName = user != null ? user.NickName : "";
                item.ImageFiles = fileInfo != null ? fileInfo : new List<SunFileInfoDto>();
                if (item.PinTopExpireAt < dt && (platformcode == PlatFormCode.WECHAT || platformcode == PlatFormCode.APP))
                {
                    item.PinTopExpireAt = Convert.ToDateTime("0001-01-01 00:00:00");
                }
            }
            StaticPagedList<ConsInfoPublicDto> pageList = new StaticPagedList<ConsInfoPublicDto>(result, page, limit, result.TotalItemCount);
            return pageList;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="types"></param>
        /// <param name="keyword"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="userid"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IPagedList<ConsInfoPublicDto>> ListInfoPublic(List<int> types, string keyword, string startdate, string enddate, int userid, int page, int limit, int areaId = 0, PlatFormCode platformcode = PlatFormCode.UNKNOWN)
        {
            if (!(page > 0 || limit > 0))
                throw new ValidException("参数无效");

            var sqlWhere = "";
            Expression<Func<ConsInfoPublic, bool>> expression = a => a.IsDeleted == 0;
            if (types.Count != 0)
            {
                expression = expression.And(a => types.Contains(a.Type));
                sqlWhere += " and type in ( " + string.Join(',', types) + ")";
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                expression = expression.And(a => a.MessageName.ToLower().Contains(keyword.ToLower()));
                sqlWhere += " and messageName  like '%" + keyword + "%'";
            }

            if (!string.IsNullOrWhiteSpace(startdate) && !string.IsNullOrWhiteSpace(enddate))
            {
                expression = expression.And(a => a.CreatedAt >= Convert.ToDateTime(startdate + " 00:00:00") && a.CreatedAt <= Convert.ToDateTime(enddate + " 23:59:59"));
                sqlWhere += " and createdAt >= '" + startdate + " 00:00:00" + "' and createdAt <= '" + enddate + " 23:59:59" + "'";
            }

            if (areaId > 0)
            {
                expression = expression.And(a => a.AreaId == areaId);
                sqlWhere += " and areaId = " + areaId;
            }
            var basicuser = await this.basicUserService.GetUserInfo(userid);
            if (basicuser == null || (basicuser.Type == 1 && basicuser.IsAuth != UserAuthAuditStatusEnum.Passed))
            {
                // 游客只能看政务部门发的内容, 未认证村民：信息公开仅展示所有政务账号发布的信息公开内容
                expression = expression.And(a => a.CreatedByDeptId > 0);
                sqlWhere += " and createdByDeptId > 0";
            }
            else
            {
                if (basicuser.Type == 3)
                {
                    // 政务用户根据角色来查看数据
                    var dataaccess = basicuser.DataAcces;
                    dataaccess = "all"; // 暂时不处理政务用户权限
                    if (dataaccess == "self")
                    {
                        expression = expression.And(a => a.CreatedBy == userid);
                    }
                    else if (dataaccess == "group")
                    {
                        expression = expression.And(a => a.CreatedByDeptId == int.Parse(basicuser.DepartmentIds));
                    }
                }
                else if (basicuser.Type == 1)
                {
                    //公众号认证村民：1、信息公开展示当前用户openid所属区域以及对应的同一层级政务账户发布的信息公开内容；
                    //          2、若切换户时存在不同村的情况，则展示当前用户openid所属区域（切换后的）以及对应的同一层级政务账户发布的信息公开内容；
                    expression = expression.And(a => a.AreaId == basicuser.AreaId);
                    sqlWhere += " and areaId = " + basicuser.AreaId;
                }
                else
                {
                    // 查看本级区域以及下级区域
                    var tree = await this.basicAreaService.GetBasicAreaTree(basicuser.AreaId);
                    expression = expression.And(a => tree.Ids.Contains(a.AreaId));
                    sqlWhere += " and areaId in (" + string.Join(',', tree.Ids) + ")";
                }
            }

            StaticPagedList<ConsInfoPublicDto> result = null;
            var dt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");
            if (platformcode == PlatFormCode.WECHAT)
            {
                // 党建宣传、精神文明、便民信息，微信显示顶置内容
                var dtstr = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                var sql = "select * from ((select *, now() as orderbydate,updatedAt as orderby from ConsInfoPublic where isDeleted = 0 " + sqlWhere + " and pinTopExpireAt >= '" + dtstr + "' order by updatedAt desc) ";
                sql += " union all (select *,createdAt as orderbydate,createdAt as orderby from ConsInfoPublic where IsDeleted = 0 " + sqlWhere + " and pinTopExpireAt <'" + dtstr + "' order by createdAt desc)) a order by orderbydate desc, orderby desc  ";
                var pagedata = this.Context.Database.SqlQueryPagedList<ConsInfoPublicDto>(page, limit, sql, "");

                result = new StaticPagedList<ConsInfoPublicDto>(pagedata, page, limit, pagedata.TotalItemCount);
            }
            else
            {
                var result_all = await this.GetQueryable().Where(expression).OrderByDescending(a => a.CreatedAt).ProjectTo<ConsInfoPublicDto>(mapper.ConfigurationProvider).ToPagedListAsync(page, limit);

                result = new StaticPagedList<ConsInfoPublicDto>(result_all, page, limit, result_all.TotalItemCount);
            }

            List<int> deptIds = result.Select(a => a.CreatedByDeptId).Distinct().ToList();
            List<int> userIds = result.Select(a => a.CreatedBy).ToList().Union(result.Select(a => a.UpdatedBy).ToList()).Distinct().ToList();
            List<string> imagelist = result.Where(a => a.Images.IsNullOrWhiteSpace() == false).Select(a => a.Images).Distinct().ToList();
            string imageIds = string.Join(",", imagelist);

            var deptList = await this.basicDepartService.GetListAsync(a => deptIds.Contains(a.Id));
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            var fileList = await this.fileService.GetSunFileInfoList(imageIds);

            foreach (var item in result)
            {
                var dept = deptList.FirstOrDefault(a => a.Id == item.CreatedByDeptId);
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
                var fileInfo = fileList.FindAll(a => ("," + item.Images + ",").Contains("," + a.Id + ","));

                item.CreatedByDeptName = dept != null ? dept.Name : await this.basicAreaService.GetAreaName(item.AreaId);
                item.CreatedByName = user != null ? user.NickName : "";
                item.UpdatedByName = user != null ? user.NickName : "";
                item.ImageFiles = fileInfo != null ? fileInfo : new List<SunFileInfoDto>();
                if (item.PinTopExpireAt < dt) {
                    item.PinTopExpireAt = Convert.ToDateTime("0001-01-01 00:00:00");
                }
            }
            StaticPagedList<ConsInfoPublicDto> pageList = new StaticPagedList<ConsInfoPublicDto>(result, page, limit, result.TotalItemCount);
            return pageList;
        }

        /// <summary>
        /// 公开信息详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="platformcode"></param>
        /// platformcode: 1 console, 2 app ,3 wechat
        /// <returns></returns>
        public async Task<ConsInfoPublicDto> DetailConsInfoPublic(int id, PlatFormCode platformcode = PlatFormCode.UNKNOWN, LoginUser loginUser=null)
        {
            var data = await GetAsync(a => a.Id == id);
            var result = mapper.Map<ConsInfoPublicDto>(data);
            if (data != null) {
                var dept = await this.basicDepartService.GetAsync(a => a.Id == data.CreatedByDeptId);
                if (dept != null)
                {
                    result.CreatedByDeptName = dept.Name;
                }
                else {
                    var areaName = await this.basicAreaService.GetAreaName(data.AreaId);
                    result.CreatedByDeptName = areaName;
                }
                var user = await this.basicUserService.GetAsync(a => a.Id == data.CreatedBy);
                if (user != null)
                {
                    result.CreatedByName = user.NickName;
                }
                user = await this.basicUserService.GetAsync(a => a.Id == data.UpdatedBy);
                if (user != null)
                {
                    result.UpdatedByName = user.NickName;
                }
                if (platformcode == PlatFormCode.WECHAT)
                {
                    // 微信公众号查看则浏览数加1
                    data.Viewcnt++;
                    await this.UpdateAsync(data);

                    BasicRPC.AllotScore(new IntegralReq()
                    {
                        IntegralAction = IntegralAction.ReadInfoPublic,
                        Description = "浏览信息公开",
                        HouseholdId = loginUser.HouseholdId,
                        UserId = loginUser.UserId
                    });
                }

                var files = await this.fileService.GetSunFileInfoList(data.Images);
                result.ImageFiles = files;
            }
            
            return result;
        }

        /// <summary>
        /// 保存公开信息
        /// </summary>
        /// <param name="consinfopublic"></param>
        /// <returns></returns>
        public async Task<int> SaveConsInfoPublic(ConsInfoPublic consinfopublic)
        {
            if (consinfopublic == null)
            {
                throw new ValidException("参数无效");
            }
            if (consinfopublic.MessageName == "" || consinfopublic.MessageText == "")
            {
                throw new ValidException("参数无效");
            }

            if (consinfopublic.Id == 0 && consinfopublic.AreaId == 0)
            {
                throw new ValidException("参数无效");
            }

            Expression<Func<ConsInfoPublic, bool>> pre = a => a.MessageName == consinfopublic.MessageName && a.IsDeleted == 0 && a.Type == consinfopublic.Type && a.AreaId == consinfopublic.AreaId;
            if (consinfopublic.Id > 0)
            {
                pre = pre.And(a => a.Id != consinfopublic.Id);
            }

            int cnt = this.GetQueryable().Where(pre).Count();
            if (cnt > 0)
            {
                throw new ValidException("标题名称重复");
            }

            if (consinfopublic.Id == 0)
            {
                consinfopublic.UpdatedBy = consinfopublic.CreatedBy;
                var user = await this.basicUserService.GetAsync(a => a.Id == consinfopublic.CreatedBy);
                if (user != null)
                {
                    if (string.IsNullOrEmpty(user.DepartmentIds))
                    {
                        consinfopublic.CreatedByDeptId = 0;
                    }
                    else
                    {
                        consinfopublic.CreatedByDeptId = int.Parse(user.DepartmentIds);
                    }
                    //consinfopublic.AreaId = user.AreaId;
                }

                var data = await this.InsertAsync(consinfopublic);
                return data.Id;
            }
            else
            {
                var data = await this.GetAsync(a => a.Id == consinfopublic.Id);
                if (data != null) {
                    data.MessageName = consinfopublic.MessageName;
                    data.MessageText = consinfopublic.MessageText;
                    data.UpdatedBy = consinfopublic.UpdatedBy;
                    data.Images = consinfopublic.Images;
                    data.Type = consinfopublic.Type;
                    data.PinTopExpireAt = consinfopublic.PinTopExpireAt;
                }
                int res = await this.UpdateAsync(data);
                if (res > 0)
                {
                    return data.Id;
                }
                return res;
            }
        }

    }
}