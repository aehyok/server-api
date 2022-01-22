using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Services;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using DVS.Core.Domains.Warning;
using DVS.Models.Dtos.Warning;
using System.Threading.Tasks;
using System.Linq.Expressions;
using LinqKit;
using System.Linq;
using DVS.Common.Infrastructures;
using DVS.Common.SO;

namespace DVS.Application.Services.Warning
{
    public class WarningMessageService : ServiceBase<WarningMessage>, IWarningMessageService
    {
        private readonly IServiceBase<WarningOperationLog> operationLogSevice;
        public WarningMessageService(DbContext context, IMapper mapper,
            IServiceBase<WarningOperationLog> operationLogSevice
              ) : base(context, mapper)
        {

            this.operationLogSevice = operationLogSevice;
        }



        public async Task<WarningOperationLog> AddOperationLog(WarningOperationLog log)
        {
            log.CreatedAt = DateTime.Now;
            log.UpdatedAt = DateTime.Now;
            if (log.Operator == null) {
                log.Operator = "";
            }
            var res = await this.operationLogSevice.InsertAsync(log);
            return res;
        }

        public async Task<WarningMessage> AddWarningMessage(WarningMessage data, string userName = "")
        {
            data.Mobile = BasicSO.Encrypt(data.Mobile);
            var res = await this.InsertAsync(data);

            if (res != null)
            {

                await AddOperationLog(new WarningOperationLog()
                {
                    WarningMessageId = res.Id,
                    CreatedBy = data.CreatedBy,
                    Operator = userName,
                    Description = "发布告警信息",
                    Operation = "发布",
                });
            }
            return res;
        }



        public async Task<IPagedList<WarningMessage>> GetWarningMessagePageList(WarnigMessageQueryBody body)
        {
            Expression<Func<WarningMessage, bool>> expr = PredicateBuilder.New<WarningMessage>(true);
            expr = expr.And(x => x.IsDeleted == 0);

            if (body.IsFinish >= 0)
            {
                expr = expr.And(x => x.IsFinish == body.IsFinish);
            }
            if (!string.IsNullOrWhiteSpace(body.Keyword))
            {
                expr = expr.And(a => a.Title.Contains(body.Keyword) || a.Descrition.Contains(body.Keyword));
            }
            var data = await this.GetPagedListAsync(expr, a => a.CreatedAt, body.Page, body.Limit, false);
            return data;
        }



        public async Task<MessageResult<bool>> FinishWarningMessage(int id, int userId, string userName = "")
        {
            var result = new MessageResult<bool>("失败了", false, false);
            var res = await this.GetQueryable().Where(a => a.Id == id)
                     .UpdateFromQueryAsync(a => new WarningMessage()
                     {
                         IsFinish = 1,
                         UpdatedBy = userId,
                         UpdatedAt = DateTime.Now,
                     });
            if (res > 0)
            {
                result.Message = "成功";
                result.Flag = true;
                result.Data = true;
                await AddOperationLog(new WarningOperationLog()
                {
                    WarningMessageId = id,
                    CreatedBy = userId,
                    Operator = userName,
                    Description = "解除告警",
                    Operation = "解除",
                });
            }
            return result;
        }


        public async Task<List<WarningOperationLog>> GetWarningOperationLogList(int warningMessageId)
        {
            var data = from w in this.operationLogSevice.GetQueryable()
                       where w.WarningMessageId == warningMessageId && w.IsDeleted == 0
                       orderby w.CreatedAt descending
                       select w;

            return await data.ToListAsync();
        }

    }
}
