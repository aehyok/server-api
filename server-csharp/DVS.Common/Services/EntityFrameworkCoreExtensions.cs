using DVS.Common.Models;
using DVS.Models.Dtos.Common;
using Lychee.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Common.Services
{


    public static class EntityFrameworkCoreExtensions
    {
        private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection connection, params object[] parameters)
        {
            var conn = facade.GetDbConnection();
            connection = conn;
            conn.Open();
            var cmd = conn.CreateCommand();
            // if (facade.IsMySql())
            // {
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(parameters);
            // }
            return cmd;
        }


        public static List<T> SqlQuery<T>(this DatabaseFacade facade, AutoMapper.Mapper mapper, string sql, params object[] parameters) where T : class, new()
        {

            var propertyInfos = typeof(T).GetProperties();
            var list = new List<T>();
            var command = CreateCommand(facade, sql, out DbConnection conn, parameters);
            var reader = command.ExecuteReader();
            list = mapper.Map<List<T>>(reader);
            return list;
        }

        public static List<T> SqlQuery<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        {
            var propertyInfos = typeof(T).GetProperties();
            var list = new List<T>();
            var command = CreateCommand(facade, sql, out DbConnection conn, parameters);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    try
                    {
                        if (reader[p.Name] != null && reader[p.Name] != DBNull.Value)
                            p.SetValue(t, reader[p.Name], null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                list.Add(t);
            }
            reader.Close();
            conn.Close();
            return list;
        }

        public static DataTable SqlQuery(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            var command = CreateCommand(facade, sql, out DbConnection conn, parameters);
            var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            conn.Close();
            return dt;
        }

        public static async Task<DataTable> SqlQueryAsync(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            var command = CreateCommand(facade, sql, out DbConnection conn, parameters);
            var reader = await command.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader);
            await reader.CloseAsync();
            await conn.CloseAsync();
            return dt;
        }


        //public static List<T> SqlQuery<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        //{
        //    var dt = SqlQuery(facade, sql, parameters);
        //    return dt.ToList<T>();
        //}

        public static async Task<List<T>> SqlQueryAsync<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        {
            var dt = await SqlQueryAsync(facade, sql, parameters);
            return dt.ToList<T>();
        }


        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            var propertyInfos = typeof(T).GetProperties();
            var list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                var t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        p.SetValue(t, row[p.Name], null);
                }
                list.Add(t);
            }
            return list;
        }

        public static IPagedList<T> SqlQueryPagedList<T>(this DatabaseFacade facade, int page, int limit, string sql, string totalItemSql, string orderSql = "", params object[] parameters) where T : class, new()
        {
            limit = limit <= 0 ? 10 : limit;
            int startLimit = (page - 1 < 0 ? 0 : page - 1) * limit;
            long totalItemCount = 0;
            string totalItemSqlNew = "";
            string sqlNew = string.Format(" select tbx.* from ({0})tbx {1} limit {2},{3} ", sql, orderSql, startLimit, limit);

            //var dt = SqlQuery(facade, sqlNew, parameters);

            //var list = dt.ToList<T>();

            var list = SqlQuery<T>(facade, sqlNew, parameters);


            // 查询总条数
            if (!string.IsNullOrWhiteSpace(totalItemSql))
            {
                totalItemSqlNew = string.Format(" select count(*) total from ({0})tbx ", totalItemSql);
            }
            else
            {
                totalItemSqlNew = string.Format(" select count(*) total from ({0})tbx ", sql);
            }

            var dt2 = SqlQuery(facade, totalItemSqlNew, parameters);
            if (dt2.Rows.Count > 0)
            {
                totalItemCount = ((long)dt2.Rows[0]["total"]);
            }

            StaticPagedList<T> pageList = new StaticPagedList<T>(list, page, limit, (int)totalItemCount);
            return pageList;
        }

        public static IPagedList<T> SqlQueryPagedList<T>(this DatabaseFacade facade, int page, int limit, string sql, string totalItemSql, params object[] parameters) where T : class, new()
        {
            limit = limit <= 0 ? 10 : limit;
            int startLimit = (page - 1 < 0 ? 0 : page - 1) * limit;
            long totalItemCount = 0;
            string totalItemSqlNew = "";
            string sqlNew = sql + " limit "+ startLimit + "," + limit;

            var list = SqlQuery<T>(facade, sqlNew, parameters);

            // 查询总条数
            if (!string.IsNullOrWhiteSpace(totalItemSql))
            {
                totalItemSqlNew = string.Format(" select count(*) total from ({0})tbx ", totalItemSql);
            }
            else
            {
                totalItemSqlNew = string.Format(" select count(*) total from ({0})tbx ", sql);
            }

            var dt2 = SqlQuery(facade, totalItemSqlNew, parameters);
            if (dt2.Rows.Count > 0)
            {
                totalItemCount = ((long)dt2.Rows[0]["total"]);
            }

            StaticPagedList<T> pageList = new StaticPagedList<T>(list, page, limit, (int)totalItemCount);
            return pageList;
        }

        public static List<OrderBy> GetValidOrderBy(this DatabaseFacade facade, List<OrderBy> orders, bool defaultById = false)
        {
            if (orders == null && defaultById)
            {
                orders = new List<OrderBy>() { new OrderBy() { FieldName = "id", Sort = "desc" } };
            }

            return orders.FindAll(o => !o.FieldName.IsNullOrWhiteSpace() && !o.Sort.IsNullOrWhiteSpace());
        }

        public static string GetOrderBySql(this DatabaseFacade facade, List<OrderBy> orders)
        {
            if (orders == null)
            {
                return string.Empty;
            }
            List<OrderBy> orderBy = GetValidOrderBy(facade, orders);
            string sql = " order by ";
            int i = 0;
            foreach (var item in orderBy)
            {
                sql += $"{item.FieldName} {item.Sort},";
                i += 1;
            }
            return i > 0 ? sql.TrimEnd(new char[] { ',' }) : "";
        }



        public static async Task<IPagedList<TEntity>> GetPagedListAsync<TEntity, TOrder>(this IServiceBase<TEntity> service, Expression<Func<TEntity, bool>> predicate, List<Expression<Func<TEntity, TOrder>>> orderBys, int page, int limit = 10, bool asc = true) where TEntity : EntityBase, new()
        {
            var query = service.GetQueryable().Where(predicate);
            if (orderBys.Count > 0)
            {
                IOrderedQueryable<TEntity> orderQuery = null;
                if (asc)
                {
                    orderQuery= query.OrderBy(orderBys[0]);
                }
                else {
                    orderQuery = query.OrderByDescending(orderBys[0]);
                }
                if (orderBys.Count > 1)
                {
                    for (int i = 1; i < orderBys.Count; i++)
                    {
                        if (asc) {
                            orderQuery= orderQuery.ThenBy(orderBys[i]);
                        }
                        else
                        {
                            orderQuery= orderQuery.ThenByDescending(orderBys[i]);
                        }
                    }
                }
                return await orderQuery.ToPagedListAsync(page, limit);
            }
            else {
                return await query.ToPagedListAsync(page, limit);
            }

        }
    }
}
