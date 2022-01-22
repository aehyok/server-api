using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    /// <summary>
    /// 推送消息体
    /// </summary>
    public class PushMessageDto
    {
        /// <summary>
        /// 极光别名列表
        /// </summary>
        public List<string> AliasList { get; set; } = new List<string>();

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; } = "";


        /// <summary>
        /// 消息描述
        /// </summary>
        public string Content { get; set; } = "";


        /// <summary>
        /// 扩展字段
        /// </summary>
        public object Extras { get; set; }

        /// <summary>
        /// 离线时间，秒，0表示不保留离线消息，不传默认1天
        /// </summary>
        public int TimeToLive { get; set; } = 1;


        /// <summary>
        /// 华为别名列表
        /// </summary>
        public List<string> HuaweiPushIds { get; set; } = new List<string>();

        /// <summary>
        /// 别名列表 预留
        /// </summary>
        public List<string> DebugUserIds { get; set; } = new List<string>();
    }
}
