using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.Cons;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Cons;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Enum;
using DVS.Models.Dtos.Common;
using DVS.Application.Services.Village;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village;
using DVS.Common;
using DVS.Common.RPC;
using DVS.Models.Dtos.RPC;

namespace DVS.Application.Services.Cons
{
    public class ProduceSaleService : ServiceBase<ConsProduceSale>, IProduceSaleService
    {
        readonly IHouseholdCodeService villageHouseholdService;
        readonly IPopulationService villagePopulationService;
        readonly IBasicDepartmentService basicDepartService;
        readonly IBasicCategoryService basicCategory;
        readonly IBasicUserService userService;
        readonly IServiceBase<VillagePopulationAddress> populationAddressService;
        readonly IBasicAreaService basicAreaService;
        private readonly IServiceBase<VillageHouseCodeMember> houseCodeMemberSerivce;

        public ProduceSaleService(DbContext dbContext, IMapper mapper, IBasicDepartmentService _deptservice, IBasicCategoryService _basicCategory, IHouseholdCodeService _villageHouseholdService, IBasicUserService _service, IPopulationService _villagePopulationService, IServiceBase<VillagePopulationAddress> populationAddressService, IBasicAreaService basicAreaService, IServiceBase<VillageHouseCodeMember> houseCodeMemberSerivce)
            : base(dbContext, mapper)
        {
            this.basicDepartService = _deptservice;
            this.basicCategory = _basicCategory;
            this.villagePopulationService = _villagePopulationService;
            this.villageHouseholdService = _villageHouseholdService;
            this.userService = _service;
            this.populationAddressService = populationAddressService;
            this.basicAreaService = basicAreaService;
            this.houseCodeMemberSerivce = houseCodeMemberSerivce;
        }

        public async Task<IPagedList<ListProduceSaleModel>> GetDataList(ProduceSaleListQueryModel model)
        {
            Expression<Func<ConsProduceSale, bool>> filter = a => a.IsDeleted == 0;
            Expression<Func<ConsProduceSale, object>> orderby = a => a.CreatedAt;
            if (!model.Keyword.IsNullOrEmpty())
            {
                filter = filter.And(a => a.Name.ToLower().Contains(model.Keyword));
            }
            if (model.CategoryId != 0)
            {
                // 根据商品类目筛选，取本级及一下所有类目
                var t = await this.GetCategoryIds(model.CategoryId);
                if (t != null && t.Ids.Count > 0)
                {
                    filter = filter.And(a => t.Ids.Contains(a.ProductId));
                }
            }
            DateTime dt = DateTime.Now;
            // Type类型 1售卖中 2已截止
            if (model.Type == 1)
            {
                filter = filter.And(a => a.ExpDate >= DateTime.Parse(dt.ToShortDateString() + " 23:59:59"));
            }
            if (model.Type == 2)
            {
                filter = filter.And(a => a.ExpDate < DateTime.Parse(dt.ToShortDateString() + " 23:59:59"));
            }

            var data = await this.GetPagedListAsync(filter, orderBy: orderby, model.Page, model.Limit, asc: false);

            List<int> productIds = new List<int>();
            foreach (var m in data)
            {
                productIds.Add(m.ProductId);
            }

            List<int> deptIds = data.Select(a => a.CreatedByDeptId).Distinct().ToList();
            List<int> userIds = data.Where(a => a.CreatedBy > 0).Select(a => (int)a.CreatedBy).ToList().Union(data.Where(a => a.UpdatedBy > 0).Select(a => (int)a.UpdatedBy).ToList()).Distinct().ToList();

            var deptList = await this.basicDepartService.GetListAsync(a => deptIds.Contains(a.Id));
            var userList = await this.userService.GetListAsync(a => userIds.Contains(a.Id));
            var categoryList = await this.basicCategory.GetBasicCategoryList(productIds);
            var list = new List<ListProduceSaleModel>();
            foreach (var m in data)
            {
                var dept = deptList.FirstOrDefault(a => a.Id == m.CreatedByDeptId);
                var createuser = userList.FirstOrDefault(a => a.Id == m.CreatedBy);
                var updateuser = userList.FirstOrDefault(a => a.Id == m.UpdatedBy);
                var r = this.mapper.Map<ListProduceSaleModel>(m);
                r.createdByDeptName = dept != null ? dept.Name : "";
                r.UpdatedByName = updateuser != null ? updateuser.NickName : "";
                r.CreatedByName = createuser != null ? createuser.NickName : "";

                var category = categoryList.FirstOrDefault(a => a.Id == m.ProductId);
                r.Name = category != null ? category.CategoryName : r.Name;
                r.ImageUrl = category != null ? category.CategoryPicUrl : "";
                r.CategoryDetail = category != null ? category.CategoryDetail : "";
                r.ImageFiles = category != null ? category.ImageFiles : null;

                if (m.CreatedBy == model.UserId)
                {
                    r.IsSelf = true;
                }
                list.Add(r);
            }

            return new StaticPagedList<ListProduceSaleModel>(list, model.Page, model.Limit, data.TotalItemCount);
        }

        public async Task<IPagedList<ListProduceSaleModel>> GetProduceSaleList(ProduceSaleListQueryModel model)
        {
            string sqlWhere = "";
            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                sqlWhere += string.Format(" and cp.Name like '%{0}%' ", model.Keyword);
            }

            if (model.CategoryId != 0)
            {
                // 根据商品类目筛选，取本级及一下所有类目
                var t = await this.GetCategoryIds(model.CategoryId);
                if (t != null && t.Ids.Count > 0)
                {
                    sqlWhere += string.Format(" and cp.ProductId in ({0}) ", string.Join(",",t.Ids));
                }
            }

            DateTime dt = DateTime.Now;
            // Type类型 1售卖中 2已截止
            if (model.Type == 1)
            {
                sqlWhere += string.Format(" and cp.ExpDate >= '{0}'", dt.ToShortDateString()+ " 23:59:59");
            }
            if (model.Type == 2)
            {
                sqlWhere += string.Format(" and cp.ExpDate < '{0}'", dt.ToShortDateString() + " 23:59:59");
            }

            if (model.AreaId > 0)
            {
                sqlWhere += string.Format(" and ((cp.parkAreaId = 0 and cp.publishId in ( select id from VillageHouseholdCode where areaId = {0})) or (cp.parkAreaId > 0 and cp.parkAreaId in ( select id from ParkArea where areaId = {0}))) ", model.AreaId);
            }
            string sql = string.Format(@" SELECT * from ConsProduceSale cp where cp.IsDeleted = 0 {0}",
                sqlWhere);

            var data = this.Context.Database.SqlQueryPagedList<ListProduceSaleModel>(model.Page, model.Limit, sql, "", " order by createdAt desc");

            List<int> productIds = new List<int>();
            foreach (var m in data)
            {
                productIds.Add(m.ProductId);
            }

            List<int> deptIds = data.Select(a => a.CreatedByDeptId).Distinct().ToList();
            List<int> userIds = data.Where(a => a.CreatedBy > 0).Select(a => (int)a.CreatedBy).ToList().Union(data.Where(a => a.UpdatedBy > 0).Select(a => (int)a.UpdatedBy).ToList()).Distinct().ToList();

            var deptList = await this.basicDepartService.GetListAsync(a => deptIds.Contains(a.Id));
            var userList = await this.userService.GetListAsync(a => userIds.Contains(a.Id));
            var categoryList = await this.basicCategory.GetBasicCategoryList(productIds);
            var list = new List<ListProduceSaleModel>();
            foreach (var m in data)
            {
                var dept = deptList.FirstOrDefault(a => a.Id == m.CreatedByDeptId);
                var createuser = userList.FirstOrDefault(a => a.Id == m.CreatedBy);
                var updateuser = userList.FirstOrDefault(a => a.Id == m.UpdatedBy);
                var r = this.mapper.Map<ListProduceSaleModel>(m);
                r.createdByDeptName = dept != null ? dept.Name : "";
                r.UpdatedByName = updateuser != null ? updateuser.NickName : "";
                r.CreatedByName = createuser != null ? createuser.NickName : "";

                var category = categoryList.FirstOrDefault(a => a.Id == m.ProductId);
                r.Name = category != null ? category.CategoryName : r.Name;
                r.ImageUrl = category != null ? category.CategoryPicUrl : "";
                r.CategoryDetail = category != null ? category.CategoryDetail : "";
                r.ImageFiles = category != null ? category.ImageFiles : null;

                if (m.CreatedBy == model.UserId)
                {
                    r.IsSelf = true;
                }
                list.Add(r);
            }

            return new StaticPagedList<ListProduceSaleModel>(list, model.Page, model.Limit, data.TotalItemCount);
        }

        public async Task<DetailProduceSaleModel> GetDetail(int id, PlatFormCode platformcode = PlatFormCode.UNKNOWN,LoginUser loginUser=null)
        {
            var data = await this.GetAsync(a => a.Id == id);
            var ret = mapper.Map<DetailProduceSaleModel>(data);
            if (ret != null)
            {
                var dept = await this.basicDepartService.GetAsync(a => a.Id == data.CreatedByDeptId);
                if (dept != null)
                {
                    ret.createdByDeptName = dept.Name;
                }
                var user = await this.userService.GetAsync(a => a.Id == data.CreatedBy);
                if (user != null)
                {
                    ret.CreatedByName = user.NickName;
                }
                user = await this.userService.GetAsync(a => a.Id == data.UpdatedBy);
                if (user != null)
                {
                    ret.UpdatedByName = user.NickName;
                }
                // 园区发布特殊处理发布者
                if (data.CreatedUserType == 4)
                {
                    var p = new VillagePopulationDto();
                    if (user != null)
                    {
                        p.RealName = user.NickName;
                        p.Mobile = user.Mobile;
                    }
                    ret.PublishInfo = p;
                }
                else
                {
                    var householdCode = await this.villageHouseholdService.GetAsync(a => a.Id == data.PublishId); // 发布农产品以户为单位，因此先查户码
                    if (householdCode != null)
                    {
                        var member = await this.houseCodeMemberSerivce.GetAsync(a => a.HouseholdId == householdCode.Id && a.IsDeleted == 0 && a.IsHouseholder == 1); // 
                        if (member != null)
                        {
                            VillagePopulation publisher = await this.villagePopulationService.GetAsync(a => a.Id == member.PopulationId);
                            var r = this.mapper.Map<VillagePopulationDto>(publisher);
                            if (r != null)
                            {
                                var registerAddress = await this.populationAddressService.GetAsync(a => a.PopulationId == r.Id && a.Type == DVS.Models.Enum.PopulationAddressTypeEnum.户籍地);
                                if (registerAddress != null)
                                {
                                    r.RegisterAddress = registerAddress.Province + registerAddress.City + registerAddress.District + registerAddress.Address;
                                }

                                var liveAddress = await this.populationAddressService.GetAsync(a => a.PopulationId == r.Id && a.Type == DVS.Models.Enum.PopulationAddressTypeEnum.居住地);
                                if (liveAddress != null)
                                {
                                    r.LiveAddress = liveAddress.Province + liveAddress.City + liveAddress.District + liveAddress.Address;
                                }
                                r.RealName = Utils.Decrypt(r.RealName);
                                r.Mobile = Utils.Decrypt(r.Mobile);
                            }
                            ret.PublishInfo = r ?? null;
                        }
                        else {
                            ret.PublishInfo = null;
                        }
                    }
                    else
                    {

                        ret.PublishInfo = null;
                    }
                }

                var category = await this.basicCategory.GetBasicCategory(data.ProductId);
                ret.Name = category != null ? category.CategoryName : ret.Name;
                ret.ImageUrl = category != null ? category.CategoryPicUrl : "";
                ret.CategoryDetail = category != null ? category.CategoryDetail : "";
                ret.ImageFiles = category != null ? category.ImageFiles : null;

                if (platformcode == PlatFormCode.WECHAT)
                {
                    // 微信公众号查看则浏览数加1
                    data.Viewcnt = data.Viewcnt + 1;
                    await this.UpdateAsync(data);
                    if (loginUser != null && loginUser.HouseholdId > 0)
                    {
                        BasicRPC.AllotScore(new IntegralReq()
                        {
                            IntegralAction = IntegralAction.ReadProduceSale,
                            Description = "浏览农产品",
                            HouseholdId = loginUser.HouseholdId,
                            UserId = loginUser.UserId
                        });
                    }
                }
            }
            return ret;
        }


        public async Task<int> Save(CreateProduceSaleModel model, PlatFormCode platformcode = PlatFormCode.UNKNOWN)
        {
            if (model == null)
            {
                throw new ValidException("参数无效, model = null");
            }
            if (model.Name == "" || model.ProductId == 0 || model.PublishId == 0)
            {
                throw new ValidException("参数无效, Name = " + model.Name + " ProductId = " + model.ProductId + " PublishId= " + model.PublishId);
            }

            var data = mapper.Map<ConsProduceSale>(model);
            if (model.Id == 0)
            {
                var user = await this.userService.GetAsync(a => a.Id == data.CreatedBy);
                if (platformcode == PlatFormCode.WECHAT)
                {
                    // 根据微信认证用户id查找户籍id，再根据户籍id查找户码id，再根据户码id找户主名称
                    var populationId = user.PopulationId; // 户籍id
                    var population = await this.villagePopulationService.GetAsync(a => a.Id == populationId && a.IsDeleted == 0);
                    if (population == null)
                    {
                        throw new ValidException("微信用户未认证");
                    }

                    // 查找户码
                    var member = await this.houseCodeMemberSerivce.GetAsync(a => a.PopulationId == populationId && a.IsDeleted == 0);
                    if (member == null)
                    {
                        throw new ValidException("微信用户未指定户码");
                    }

                    // 查找户主
                    member = await this.houseCodeMemberSerivce.GetAsync(a => a.HouseholdId == member.HouseholdId && a.IsHouseholder == 1 && a.IsDeleted == 0);
                    if (member == null)
                    {
                        throw new ValidException("户码未设置户主");
                    }

                    // 查找户码
                    var houseHold = await this.villageHouseholdService.GetAsync(a => a.Id == member.HouseholdId && a.IsDeleted == 0);
                    if (houseHold == null)
                    {
                        throw new ValidException("户码不存在");
                    }

                    var name = Utils.Decrypt(population.RealName);
                    var publisher = $"{name}({houseHold.HouseName}{houseHold.HouseNumber})"; // 发布人 = 户主名称(门牌名门牌号)

                    data.PublishId = houseHold.Id; // 户码id
                    data.Publisher = publisher;
                }
                if (data.CreatedByDeptId == 0)
                {
                    if (user != null && !string.IsNullOrEmpty(user.DepartmentIds))
                    {
                        data.CreatedByDeptId = int.Parse(user.DepartmentIds);
                    }
                }

                var house = await this.villageHouseholdService.GetAsync(a => a.Id == data.PublishId);
                var areaName = this.basicAreaService.GetParentAreaString(house.AreaId); // 区域名称包括上级
                data.Address = areaName;
                data = await this.InsertAsync(data);
                //发放积分
                BasicRPC.AllotScore(new IntegralReq()
                {
                    IntegralAction = IntegralAction.CreateProducesale,
                    Description = "发布农产品",
                    HouseholdId = model.PublishId,
                    UserId = data.CreatedBy == null ? 0 : data.CreatedBy.Value
                });
                return data.Id;
            }
            else
            {
                var info = await this.GetAsync(a => a.Id == model.Id);
                if (info == null)
                {
                    throw new Exception("数据不存在");
                }
                info.Name = data.Name;
                info.Number = data.Number;
                info.Publisher = data.Publisher;
                info.PublishId = data.PublishId;
                info.Price = data.Price;
                info.ExpDate = data.ExpDate;
                info.ProductId = model.ProductId;
                var house = await villageHouseholdService.GetAsync(a => a.Id == data.PublishId);
                var areaName = basicAreaService.GetParentAreaString(house.AreaId); // 区域名称包括上级
                info.Address = areaName;

                int res = await this.UpdateAsync(info);
                if (res > 0)
                {
                    return data.Id;
                }
                return res;
            }
        }

        public async Task<IPagedList<ListProduceSaleModel>> MyDataList(ProduceSaleListQueryModel model, PlatFormCode platformcode = PlatFormCode.UNKNOWN)
        {
            Expression<Func<ConsProduceSale, bool>> filter = a => a.IsDeleted == 0;
            if (platformcode == PlatFormCode.APP) // app
            {
                if (model.PublishId != 0)
                {
                    // 从app的乡村治理--选择户主--农产品菜单时使用，PublishId就是该户主的id
                    filter = filter.And(a => a.PublishId == model.PublishId);
                }
                else
                {
                    filter = filter.And(a => a.CreatedBy == model.UserId);
                }
            }
            if (platformcode == PlatFormCode.WECHAT) // wechat, 只查询当事人的记录
            {
                filter = filter.And(a => a.CreatedBy == model.UserId);
            }

            if (!model.Keyword.IsNullOrEmpty())
            {
                filter = filter.And(a => a.Name.ToLower().Contains(model.Keyword));
            }
            DateTime dt = DateTime.Now;
            if (model.Type == 1)
            {
                filter = filter.And(a => a.ExpDate >= dt);
            }
            if (model.Type == 2)
            {
                filter = filter.And(a => a.ExpDate < dt);
            }

            Expression<Func<ConsProduceSale, object>> orderby = a => a.CreatedAt;

            var data = await this.GetPagedListAsync(filter, orderBy: orderby, model.Page, model.Limit, asc: false);

            List<int> productIds = new List<int>();
            foreach (var m in data)
            {
                productIds.Add(m.ProductId);
            }

            var categoryList = await this.basicCategory.GetListAsync(a => productIds.Contains(a.Id));
            var list = new List<ListProduceSaleModel>();
            foreach (var m in data)
            {
                var r = this.mapper.Map<ListProduceSaleModel>(m);
                var category = categoryList.FirstOrDefault(a => a.Id == m.ProductId);
                r.Name = category != null ? category.CategoryName : r.Name;
                r.ImageUrl = category != null ? category.CategoryPicUrl : "";
                r.CategoryDetail = category != null ? category.CategoryDetail : "";
                list.Add(r);
            }

            return new StaticPagedList<ListProduceSaleModel>(list, model.Page, model.Limit, data.TotalItemCount);
        }

        public async Task<IPagedList<ListProduceSaleModel>> GetDataByAreaIdList(ProduceSaleListQueryModel model)
        {
            // 查询区域下的所有户码id
            var publishlist = await this.villageHouseholdService.GetListAsync(a => a.AreaId == model.AreaId && a.IsDeleted == 0);
            if (model.AreaId == 0)
            {
                publishlist = await this.villageHouseholdService.GetListAsync(a => a.IsDeleted == 0);
            }
            List<int> publishIds = new List<int>();
            foreach (var item in publishlist)
            {
                publishIds.Add(item.Id);
            }
            Expression<Func<ConsProduceSale, bool>> filter = a => a.IsDeleted == 0 && publishIds.Contains(a.PublishId);

            DateTime dt = DateTime.Now;
            filter = filter.And(a => a.ExpDate >= dt);

            Expression<Func<ConsProduceSale, object>> orderby = a => a.CreatedAt;

            var data = await this.GetPagedListAsync(filter, orderBy: orderby, model.Page, model.Limit, asc: false);

            List<int> productIds = new List<int>();
            foreach (var m in data)
            {
                productIds.Add(m.ProductId);
            }

            var categoryList = await this.basicCategory.GetListAsync(a => productIds.Contains(a.Id));
            var list = new List<ListProduceSaleModel>();
            foreach (var m in data)
            {
                var r = this.mapper.Map<ListProduceSaleModel>(m);
                var category = categoryList.FirstOrDefault(a => a.Id == m.ProductId);
                r.Name = category != null ? category.CategoryName : r.Name;
                r.ImageUrl = category != null ? category.CategoryPicUrl : "";
                r.CategoryDetail = category != null ? category.CategoryDetail : "";

                list.Add(r);
            }

            return new StaticPagedList<ListProduceSaleModel>(list, model.Page, model.Limit, data.TotalItemCount);
        }

        public async Task<int> MyPublishCount(int userId, PlatFormCode platformcode = PlatFormCode.UNKNOWN)
        {
            if (platformcode == PlatFormCode.APP) // app
            {
                return await this.GetQueryable().Where(a => a.CreatedBy == userId && a.IsDeleted == 0).CountAsync();
            }
            if (platformcode == PlatFormCode.WECHAT)
            { // wechat
                return await this.GetQueryable().Where(a => a.CreatedBy == userId && a.IsDeleted == 0).CountAsync();
            }
            return 0;
        }


        public async Task<List<PublisherModel>> GetPublisher(PostBody model)
        {
            var publishlist = await this.villageHouseholdService.GetHouseholdCodeList(model);
            List<PublisherModel> publisher = new List<PublisherModel>();
            foreach (var item in publishlist)
            {
                var name = item.HouseholdMan;
                name = $"{name}({item.HouseName}{item.HouseNumber})";
                var p = new PublisherModel(); // 户码id
                p.ID = item.Id;
                p.Name = name;
                publisher.Add(p);
            }

            return publisher;
        }


        private void CategoryChildren(IList<BasicCategory> basicList, List<BasicCategoryModel> children, List<int> Ids)
        {
            foreach (var parent in children)
            {
                parent.Children = mapper.Map<List<BasicCategoryModel>>(basicList.Where(a => a.ParentId == parent.Id).OrderBy(a => a.Sequence).ToList());
                if (parent.Children != null && parent.Children.Count > 0)
                {
                    List<int> ChildrenIds = new List<int>();
                    ChildrenIds.Add(parent.Id);
                    foreach (var item in parent.Children)
                    {
                        Ids.Add(item.Id);
                        ChildrenIds.Add(item.Id);
                    }
                    parent.Ids = ChildrenIds;
                    this.CategoryChildren(basicList, parent.Children, Ids);
                }
            }
        }
        public async Task<BasicCategoryModel> GetCategoryIds(int id)
        {
            var category = await this.basicCategory.GetAsync(a => a.Id == id && a.IsDeleted == 0);

            var tree = new BasicCategoryModel();

            var list = await this.basicCategory.GetListAsync(a => a.IsDeleted == 0);

            var parent = list.FirstOrDefault(a => a.Id == id);
            if (parent != null)
            {
                tree.Id = parent.Id;
                tree.ParentId = parent.ParentId;
                tree.Sequence = parent.Sequence;
                tree.CategoryName = parent.CategoryName;
                List<int> Ids = new List<int>();
                Ids.Add(parent.Id);
                var clist = mapper.Map<List<BasicCategoryModel>>(list.Where(a => a.ParentId == id).OrderBy(a => a.Sequence).ToList());
                tree.Children = clist;
                foreach (var item in clist)
                {
                    Ids.Add(item.Id);
                    item.Ids.Add(item.Id);
                }
                this.CategoryChildren(list, clist, Ids);
                tree.Ids = Ids;

            }
            return tree;
        }
    }
}
