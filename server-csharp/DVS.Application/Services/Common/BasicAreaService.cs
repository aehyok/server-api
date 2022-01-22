using AutoMapper;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.Extensions.Logging;

namespace DVS.Application.Services.Common
{
    public class BasicAreaService : ServiceBase<BasicArea>, IBasicAreaService
    {
        private readonly ILogger logger;
        public BasicAreaService(DbContext dbContext, IMapper mapper, ILogger<BasicAreaService> logger)
            : base(dbContext, mapper)
        {
            this.logger = logger;
        }

        public async Task<string> GetAreaName(int areaId)
        {
            if (areaId > 0)
            {
                var data = await this.GetAsync(a => a.Id == areaId && a.IsDeleted == 0);
                if (data != null)
                {
                    return data.Name;
                }
            }
            return "";
        }


        public async Task<List<BasicArea>> FindChildrenAreas(int areaId) {


            // 找子
            var list = await this.Context.Database.SqlQueryAsync<BasicArea>(string.Format(@"  SELECT  DATA.id,DATA.pid,DATA.areaCode,DATA.`name`,DATA.`level`,DATA.sequence
                            FROM BasicArea DATA,(
                                SELECT
                                    @id as id,
                                    @id:= (SELECT GROUP_CONCAT(id) FROM BasicArea WHERE FIND_IN_SET(pid, @id) ) as cid
                                FROM BasicArea ID,
                                    (SELECT @id:= {0}) x
                              WHERE @id IS NOT NULL
                            )ID WHERE FIND_IN_SET(DATA.id, ID.id)", areaId));

            return list;


        }

        public async Task<List<int>> FindChildrenAreaIds(int areaId, bool includeMe = true, bool cache = true)
        {
            string key = $"children_areaids_{areaId}";
            List<int> ids = new List<int>();

            try
            {

                if (cache)
                {
                    string str = await RedisHelper.GetAsync(key);
                    if (str != null)
                    {

                        string[] strArr = str.Split(',');

                        foreach (var item in strArr)
                        {
                            ids.Add(int.Parse(item));
                        }
                        return ids;
                    }
                }

                var list = await FindChildrenAreas(areaId);
                foreach (var item in list)
                {
                    if (!includeMe && item.Id == areaId) // 不包含自己
                    {
                        continue;
                    }
                    ids.Add(item.Id);
                }
                if (cache)
                {
                    await RedisHelper.SetAsync(key, string.Join(',', ids.ToArray()), 120);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);

            }

            return ids;
        }


        public List<BasicArea> FindParentAreas(int areaId)
        {
            // 找子
            // SELECT DATA.*
            //FROM hub_sys_menu DATA,(
            //    SELECT
            //        @id as id,
            //        @id:= (SELECT GROUP_CONCAT(menuid) FROM hub_sys_menu WHERE FIND_IN_SET(parentid, @id) ) as cid
            //    FROM hub_sys_menu ID,
            //        (SELECT @id:= '00ecdb96-e040-4bda-83f7-c70103b34876' ) x
            //  WHERE @id IS NOT NULL
            //)ID WHERE FIND_IN_SET(DATA.menuid, ID.id)

            // 找父
            var list = this.Context.Database.SqlQuery<BasicArea>(string.Format(@"SELECT t.id,t.pid,t.areaCode,t.`name`,t.`level`,t.sequence
                                        FROM BasicArea t,(
                                            SELECT
                                                @id as id,
                                                @id := (SELECT pid FROM BasicArea WHERE id = @id and isDeleted=0 ) as pid
                                            FROM BasicArea ID,
                                                ( SELECT @id := {0} ) x
                                            WHERE @id IS NOT NULL
                                        )ID WHERE ID.id = t.id and t.isDeleted=0 ORDER BY t.`level` ASC ", areaId));

            return list;
        }

        public string FindParentAreaString(int areaId, bool includeMe = true)
        {
            try
            {
                var list = FindParentAreas(areaId);
                string str = "";
                foreach (var item in list)
                {
                    if (!includeMe && item.Id == areaId) // 不包含自己
                    {
                        continue;
                    }
                    str += item.Name + " ";
                }
                return str.Trim();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return "";
            }
        }

        private void RecursionChildren(IList<BasicArea> basicAreas, List<BasicAreaTreeDto> children, List<int> Ids)
        {
            foreach (var parent in children)
            {
                parent.Children = mapper.Map<List<BasicAreaTreeDto>>(basicAreas.Where(a => a.Pid == parent.Id).OrderBy(a => a.Sequence).ToList());
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
                    this.RecursionChildren(basicAreas, parent.Children, Ids);
                }
            }
        }
        public async Task<BasicAreaTreeDto> GetBasicAreaTree(int areaId = 0)
        {
            var list = await this.GetListAsync(a => a.IsDeleted == 0);
            var tree = new BasicAreaTreeDto();

            var parent = list.FirstOrDefault(a => a.Id == areaId || a.Pid == 0);
            if (areaId > 0)
            {
                parent = list.FirstOrDefault(a => a.Id == areaId);
            }

            if (parent != null)
            {

                tree.Id = parent.Id;
                tree.Level = parent.Level;
                tree.Sequence = parent.Sequence;
                tree.Name = parent.Name;
                List<int> Ids = new List<int>();
                Ids.Add(parent.Id);
                tree.Children = mapper.Map<List<BasicAreaTreeDto>>(list.Where(a => a.Pid == tree.Id).OrderBy(a => a.Sequence).ToList());
                foreach (var item in tree.Children)
                {
                    Ids.Add(item.Id);
                    item.Ids.Add(item.Id);
                }
                this.RecursionChildren(list, tree.Children, Ids);
                tree.Ids = Ids;
            }
            return tree;
        }

        /// <summary>
        /// 查询本级和上级
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public string GetParentAreaString(int areaId)
        {
            var list = this.Context.Database.SqlQuery<BasicArea>(string.Format(@"SELECT * FROM (
                        SELECT id,pid,areaCode,name,level,sequence
                        FROM BasicArea
                        WHERE id = {0}
                        UNION ALL
                        SELECT id,pid,areaCode,name,level,sequence
                        FROM BasicArea
                        WHERE id = (SELECT pid FROM BasicArea WHERE id = {1})
                        ) a ORDER BY level ASC 
                        ", areaId, areaId));

            string str = "";
            foreach (var item in list)
            {
                str += item.Name + " ";
            }
            return str.Trim();

        }
    }
}
