using Microsoft.EntityFrameworkCore.Diagnostics;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Collections.Generic;

namespace Lychee.EntityFramework.MySql
{
    public class LycheeMySqlConfiguration
    {
        /// <summary>
        /// 数据库版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 数据库迁移程序集
        /// </summary>
        public string MigrationsAssembly { get; set; }

        /// <summary>
        /// 数据库类型 MySQL/Mariadb
        /// </summary>
        public ServerType ServerType { get; set; } = ServerType.MySql;

        /// <summary>
        /// 是否使用延迟加载
        /// </summary>
        public bool UseLazyLoadingProxies { get; set; }

        /// <summary>
        /// EF 数据迁移时忽略外键关系
        /// </summary>
        public bool IgonreForeignKeys { get; set; }

        /// <summary>
        /// EF 拦截器
        /// </summary>
        public IEnumerable<IInterceptor> Interceptors = new List<IInterceptor>();
    }
}