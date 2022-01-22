using System.Threading.Tasks;

namespace Lychee.Core.Events.Handlers
{
    public interface IEventHandler
    { }

    /// <summary>
    /// 事件处理器
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IEventHandler<in TEvent> : IEventHandler where TEvent : IEvent
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task HandleAsync(TEvent @event);
    }
}