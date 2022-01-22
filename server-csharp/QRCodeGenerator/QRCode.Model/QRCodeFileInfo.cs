using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLQRCode.Model
{
  public class QRCodeFileInfo
  {
    [JsonProperty("id")]
    public int Id { get; set; }
  }
}
