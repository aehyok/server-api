namespace Lychee.Extension.ChunkUploader
{
    public class ChunkResult
    {
        /// <summary>
        /// 分片序号
        /// </summary>
        public int ChunkNumber { get; set; }

        /// <summary>
        /// 分片大小
        /// </summary>
        public int ChunkSize { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int TotalSize { get; set; }

        /// <summary>
        /// 文件 MD5
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 分片总数
        /// </summary>
        public int TotalChunk { get; set; }
    }
}