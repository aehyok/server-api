using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Models.SunFile
{
    /// <summary>
    /// 文件清理任务
    /// </summary>
    public class FileCleanTask
    {
        // 文件的路径，判断文件是否存在，存在则删除
        public string FilePath { get; set; }
    }
}
