using AutoMapper;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using DVS.Models.Dtos.Common;
using X.PagedList;
using DVS.Common.SO;

namespace DVS.Application.Services.Common
{
    public class BasicDictionaryService : ServiceBase<BasicDictionary>, IBasicDictionaryService
    {

        private readonly IServiceBase<BasicUser> basicUserService;
        private readonly ISunFileInfoService sunFileInfoService;
        public BasicDictionaryService(DbContext dbContext, IMapper mapper, IServiceBase<BasicUser> basicUserService, ISunFileInfoService sunFileInfoService)
            : base(dbContext, mapper)
        {
            this.basicUserService = basicUserService;
            this.sunFileInfoService = sunFileInfoService;
        }

        public async Task<IEnumerable<BasicDictionary>> GetBasicDictionaryList(int typecode)
        {
            if (typecode <= 0)
            {
                var list = this.GetQueryable().Where(a => a.IsDeleted == 0 && a.Status == 1).OrderBy(a => a.TypeCode).OrderBy(a => a.Sequence).OrderBy(a => a.Code);
                return list;
            }
            else
            {
                var list = this.GetQueryable().Where(a => a.IsDeleted == 0 && a.TypeCode == typecode && a.Status == 1).OrderBy(a => a.Sequence).OrderBy(a => a.Code);
                return list;
            }
        }

        public async Task<IEnumerable<BasicDictionary>> GetBasicDictionaryList(List<int> codes)
        {
            var list = await this.GetListAsync(a => codes.Contains(a.Code) && a.IsDeleted == 0 && a.Status == 1);
            return list;
        }


        public async Task<IEnumerable<BasicDictionary>> GetBasicDictionaryList(List<string> ids)
        {
            List<int> idList = new List<int>();
            foreach (var id in ids)
            {
                int _id;
                if (id != null && int.TryParse(id, out _id))
                {
                    idList.Add(_id);
                }

            }
            var data = from d in this.GetQueryable()
                       where idList.Contains(d.Id)
                       select d;
            return await data.ToListAsync();
        }


        public async Task<string> GetNameById(string id, IEnumerable<BasicDictionary> basicDictionaries = null)
        {
            int _id;
            if (int.TryParse(id, out _id))
            {
                if (basicDictionaries != null && basicDictionaries.Count() > 0)
                {
                    var data = basicDictionaries.FirstOrDefault(a => a.Id == _id);
                    if (data != null)
                    {
                        return data.Name;
                    }
                }
                else
                {
                    var data = await this.GetAsync(a => a.Id == _id);
                    if (data != null)
                    {
                        return data.Name;
                    }
                }

                return "";
            }
            return id;
        }

        public async Task<IEnumerable<BasicDictionary>> GetBasicDictionaryCodeList(List<string> codes)
        {
            List<int> idList = new List<int>();
            foreach (var code in codes)
            {
                int _code;
                if (code != null && int.TryParse(code, out _code))
                {
                    idList.Add(_code);
                }

            }
            var data = from d in this.GetQueryable()
                       where idList.Contains(d.Code)
                       select d;
            return await data.ToListAsync();
        }

        public async Task<string> GetNameByCode(string code, IEnumerable<BasicDictionary> basicDictionaries = null)
        {
            int _code;
            if (int.TryParse(code, out _code))
            {
                if (basicDictionaries != null && basicDictionaries.Count() > 0)
                {
                    var data = basicDictionaries.FirstOrDefault(a => a.Code == _code);
                    if (data != null)
                    {
                        return data.Name;
                    }
                }
                else
                {
                    var data = await this.GetAsync(a => a.Code == _code);
                    if (data != null)
                    {
                        return data.Name;
                    }
                }

                return "";
            }
            return code;
        }
        public async Task<BasicDictionaryDto> GetOneByCode(string code, IEnumerable<BasicDictionary> basicDictionaries = null)
        {
            int _code;
            if (int.TryParse(code, out _code))
            {
                if (basicDictionaries != null && basicDictionaries.Count() > 0)
                {
                    var data = basicDictionaries.FirstOrDefault(a => a.Code == _code);
                    if (data != null)
                    {
                        return mapper.Map<BasicDictionaryDto>(data);
                    }
                }
                else
                {
                    var data = await this.GetAsync(a => a.Code == _code);
                    if (data != null)
                    {
                        return mapper.Map<BasicDictionaryDto>(data);
                    }
                }

                return null;
            }
            return null;
        }

        /// <summary>
        /// 获取标签分页数据
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<IPagedList<BasicDictionaryHouseTagDto>> GetDictionaryHouseTagPageListAsync(HouseTagQueryModel body)
        {
            string sql = string.Format(@"
                            SELECT 
                            d.id,
                            d.`code`,
                            d.`name`,
                            d.typeCode,
                            d.sequence,
                            d.fontColor,
                            d.description,
                            d.remark,
                            d.iconFileId,
                            d.iconFileUrl,
                            d.updatedAt,
                            u.nickName as updatedBy 
                            FROM BasicDictionary d
                            LEFT JOIN BasicUser u on d.UpdatedBy = u.id
                            WHERE d.typeCode={0} and d.IsDeleted=0 ", body.TypeCode);
            string keyword = DVS.Common.Utils.FilterKeyword(body.Keyword);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += " and `name` like '%" + keyword + "%'";
            }

            var data = this.Context.Database.SqlQueryPagedList<BasicDictionaryHouseTagDto>(body.Page, body.Limit, sql, "", this.Context.Database.GetOrderBySql(body.Orders));
            foreach (var item in data)
            {
                item.UpdatedBy = BasicSO.Decrypt(item.UpdatedBy);
            }
            return data;
        }


        /// <summary>
        /// 保存门牌标签
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> SaveDictionaryHouseTagAsync(BasicDictionaryHouseTagBody body, int updatedBy = 0)
        {
            //var res = await this.GetQueryable().Where(a => a.Id == body.Id).UpdateFromQueryAsync(a => new BasicDictionary()
            //{

            //    Description = body.Description,
            //    Remark = body.Remark,
            //    IconFileId = body.IconFileId,
            //    IconFileUrl = body.IconFileUrl,
            //    UpdatedAt = DateTime.Now,
            //    UpdatedBy = updatedBy
            //});

            var res = await this.Context.Database.ExecuteSqlRawAsync(@"update BasicDictionary set
                                                                      Description={0},Remark={1},IconFileId={2},IconFileUrl={3},UpdatedAt={4},UpdatedBy={5}
                                                                      where id={6}", body.Description, body.Remark, body.IconFileId, body.IconFileUrl, DateTime.Now, updatedBy, body.Id);
            return res > 0;
        }
    }
}
