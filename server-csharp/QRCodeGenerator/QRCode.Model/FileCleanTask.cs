using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLQRCode.Model
{
  public class FileCleanTask
  {
    [JsonProperty("filePath")]
    public string FilePath { get; set; }

    [JsonProperty("id")]
    public string TaskId { get; set; }
  }
}
