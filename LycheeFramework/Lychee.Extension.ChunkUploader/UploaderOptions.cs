namespace Lychee.Extension.ChunkUploader
{
    public class UploaderOptions
    {
        /// <summary>
        /// 上传文件保存路径，默认为应用程序根目录下的 Uploads 文件夹
        /// </summary>
        public string SaveFolder { get; set; }

        /// <summary>
        /// 上传碎片文件临时目录，默认为上传文件保存路径下的 Temp 文件夹
        /// </summary>
        public string TemporayFolder { get; set; }
    }
}