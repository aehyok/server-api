using System.Reflection;
using System.Threading.Tasks;

namespace Lychee.Core.Schedulers
{
    /// <summary>
    /// 调度器
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        /// 暂停
        /// </summary>
        /// <returns></returns>
        Task PauseAsync();

        /// <summary>
        /// 恢复
        /// </summary>
        /// <returns></returns>
        Task ResumeAsync();

        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        Task StopAsnc();

        /// <summary>
        /// 添加作业
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <returns></returns>
        Task AddJobAsync<TJob>() where TJob : IJob, new();

        /// <summary>
        /// 扫描并添加作业
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        Task ScanJobsAsync(params Assembly[] assemblies);
    }
}