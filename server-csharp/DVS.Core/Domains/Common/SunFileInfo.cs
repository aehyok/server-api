using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Common
{
    public class SunFileInfo: DvsEntityBase
    {
        /// <summary>
        /// 保存到CDN的id
        /// </summary>
        public string RefId { get; set; }
        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OriginName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long ByteSize { get; set; }
        /// <summary>
        /// 文件访问的相对路径
        /// </summary>
        public string RelativePath { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string ExtensionName { get; set; }
        /// <summary>
        /// 文件类型，1图片，2视频，3压缩文件(zip,tar.gz……)，4文本文件（txt,pdf)
        /// </summary>
        public int FileType { get; set; }
        /// <summary>
        /// 文件的Md5
        /// </summary>
        public string Md5 { get; set; }
        /// <summary>
        /// 缩略图对应的ID
        /// </summary>
        public int ThumbnailId { get; set; }

        /// <summary>
        /// 缩略图路径
        /// </summary>
        public string ThumbnailRelativePath { get; set; }

        /// <summary>
        /// 文件是否被使用
        /// </summary>
        public bool Used { get; set; } = false;
        /// <summary>
        /// 素材文件，素材文件不受Used影响而被删除
        /// </summary>
        public bool IsMaterial { get; set; } = false;
        /// <summary>
        /// 是否已经做H264转码
        /// </summary>
        public bool IsTransformed { get; set; } = false;
    }
}
