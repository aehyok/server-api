using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLQRCode.Model
{
  public class NotifyCompletedResult
  {
    [JsonProperty("taskId")]
    public string TaskId { get; set; }
  }
}
