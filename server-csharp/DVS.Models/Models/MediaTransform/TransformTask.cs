using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Models.MediaTransform
{
    public class TransformTask
    {
        public int FileId { get; set; }
        public string DownloadUrl { get; set; }
        public string UploadUrl { get; set; }
        public string Md5 { get; set; }
        public string NotifyCompletedUrl { get; set; }
    }
}
