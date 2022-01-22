using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    public class SunFileInfoDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OriginName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string ByteSize { get; set; }
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
        /// 缩略图相对路径
        /// </summary>
        public string ThumbnailRelativePath { get; set; }

        /// <summary>
        /// 文件是否被使用
        /// </summary>
        public bool Used { get; set; }
        /// <summary>
        /// 素材文件，素材文件不受Used影响而被删除
        /// </summary>
        public bool IsMaterial { get; set; }
        /// <summary>
        /// 文件的Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 缩略图的URL
        /// </summary>
        public string ThumbnailUrl { get; set; }
    }
}
