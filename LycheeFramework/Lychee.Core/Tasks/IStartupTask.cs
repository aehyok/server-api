using System.Threading.Tasks;

namespace Lychee.Core.Tasks
{
    /// <summary>
    /// 启动任务
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// 任务排序
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        Task ExecuteAsync();
    }
}