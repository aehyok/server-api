using DVS.Common.Services;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
   public  interface IFFPMatrixService : IServiceBase<FFPMatrix>
    {
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<int> DeleteMatrix(int id, int userid);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="inspector"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IPagedList<FFPMatrixDto>> ListMatrix(string keyword, int inspector, int page, int limit, string ids = "");

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FFPMatrixDto> DetailMatrix(int id);


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        Task<int> SaveMatrix(FFPMatrix matrix);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="inspector"></param>
        /// <returns></returns>
        Task<byte[]> GetExcelData(string keyword, int inspector, string ids = "");
    }
}
