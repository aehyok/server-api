using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    /// <summary>
    /// 村民认证申请
    /// </summary>
    public class ApplyUserAuthDto
    {

        /// <summary>
        /// 提交类型 1 第一次提交mobile必填    2 第二次提交 mobile 、realName、idCard 必填
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }
        
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; }
        
        /// <summary>
        /// 图片Id
        /// </summary>
        public string ImageId { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrls { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>

        public string OpenId { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>

        public int AreaId { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>

        public string MobileCode { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; } = 0;

    }
}
