using System;

namespace Lychee.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 获取时间戳（秒）
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long GetUnixTimestamp(this DateTime date)
        {
            return date.ToDateTimeOffset().ToUnixTimeMilliseconds() / 1000;
        }

        /// <summary>
        /// 获取时间戳（毫秒）
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long GetUnixTimeMilliseconds(this DateTime date)
        {
            return date.ToDateTimeOffset().ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// DateTime 转 DateTimeOffset
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime() <= DateTimeOffset.MinValue.UtcDateTime ? DateTimeOffset.MinValue : new DateTimeOffset(dateTime);
        }
    }
}