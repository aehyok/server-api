using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lychee.Extensions
{
    public static class ThreadExtensions
    {
        /// <summary>
        /// 执行多个操作，等待所有操作完成
        /// </summary>
        /// <param name="actions"></param>
        public static void WaitAll(params Action[] actions)
        {
            if (actions == null)
            {
                return;
            }

            var tasks = new List<Task>();

            foreach (var action in actions)
            {
                tasks.Add(Task.Factory.StartNew(action, TaskCreationOptions.None));
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// 并发执行多个操作
        /// </summary>
        /// <param name="actions"></param>
        public static void ParallelExecute(params Action[] actions)
        {
            Parallel.Invoke(actions);
        }

        /// <summary>
        /// 重复执行并发操作
        /// </summary>
        /// <param name="action"></param>
        /// <param name="count"></param>
        /// <param name="options"></param>
        public static void ParallelExecute(Action action, int count = 1, ParallelOptions options = null)
        {
            if (options == null)
            {
                Parallel.For(0, count, i => action());
                return;
            }

            Parallel.For(0, count, options, i => action());
        }
    }
}