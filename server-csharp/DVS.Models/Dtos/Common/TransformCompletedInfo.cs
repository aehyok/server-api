using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    public class TransformCompletedInfo
    {
        public bool Completed { get; set; }
        public int FileId { get; set; }
        public int ThumbnailId { get; set; }
        public int TransformVideoId { get; set; }
    }
}
