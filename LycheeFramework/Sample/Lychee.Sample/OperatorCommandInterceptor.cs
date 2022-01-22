using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Lychee.Sample
{
    public class OperatorCommandInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<DbCommand> CommandCreating(CommandCorrelatedEventData eventData, InterceptionResult<DbCommand> result)
        {
            return base.CommandCreating(eventData, result);
        }
    }
}