using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.SunFSAgent
{
    [Serializable]
    public class FileUploadSunDFSResult
    {
        
        public int code { get; set; }
        public string message { get; set; }
        public SunDFSResultData data { get; set; }
    }
    [Serializable]
    public class SunDFSResultData
    {
        public string Description { get; set; }
        public string FailReason { get; set; }
        public string FileId { set; get; }
        public string SavePath { get; set; }
        public int Status { get; set; }
    }
}
