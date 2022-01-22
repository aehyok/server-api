using Cronos;
using System;

namespace DVS.Core
{
    public static class CronHelper
    {
        public static DateTime? GetNextOccurrence(string expression, CronFormat format = CronFormat.Standard)
        {
            return CronExpression.Parse(expression, format).GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local);
        }
    }
}