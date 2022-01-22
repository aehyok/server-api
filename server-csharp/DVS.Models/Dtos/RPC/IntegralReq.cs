using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.RPC
{
    public class IntegralReq
    {
        public int HouseholdId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 对于发积分才起作用
        /// </summary>
        public int Score { get; set; } = 0;

        public IntegralAction IntegralAction { get; set; }
    }

    public enum IntegralAction {
        /// <summary>
        /// 发积分
        /// </summary>
        Allot,
        /// <summary>
        /// 创建户码
        /// </summary>
        CreateHousehold,
        /// <summary>
        /// 认证用户
        /// </summary>
        ApplyUserAuth,
        /// <summary>
        /// 门牌标签
        /// </summary>
        FamilyTag,
        /// <summary>
        /// 随手拍
        /// </summary>
        PhotoAnyWhere,
        /// <summary>
        /// 发布农产品
        /// </summary>
        CreateProducesale,
        /// <summary>
        /// 查看农产品
        /// </summary>
        ReadProduceSale,
        /// <summary>
        /// 查看公开信息
        /// </summary>
        ReadInfoPublic,
        /// <summary>
        /// 查看便民服务
        /// </summary>
        ServiceChannel,
        /// <summary>
        /// 签到
        /// </summary>
        CheckIn
    }
}
