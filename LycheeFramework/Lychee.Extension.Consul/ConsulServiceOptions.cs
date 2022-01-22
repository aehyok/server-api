namespace Lychee.Extension.Consul
{
    public class ConsulServiceOptions
    {
        /// <summary>
        /// 服务注册地址
        /// </summary>
        public string ConsulAddress { get; set; }

        /// <summary>
        /// 服务 ID
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 健康检查地址，完整并且 Consul 服务可直接访问地址
        /// </summary>
        public string HealthCheck { get; set; }

        /// <summary>
        /// 本服务运行地址
        /// </summary>
        public string ServiceAddress { get; set; }

        /// <summary>
        /// 本服务运行端口
        /// </summary>
        public int ServicePort { get; set; }

        /// <summary>
        /// 注册超时事件（单位秒），默认 5
        /// </summary>
        public int Timeout { get; set; } = 5;

        /// <summary>
        /// 服务停止多长时间后注销服务（单位秒），默认 5
        /// </summary>
        public int DeregisterCriticalServiceAfter { get; set; } = 5;

        /// <summary>
        /// 健康检测时间间隔（单位秒），默认值 10
        /// </summary>
        public int Interval { get; set; } = 10;
    }
}