using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLQRCode.Model
{
  public class QRCodeContent
  {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("householderName")]
        public string HouseholderName { get; set; }
        [JsonProperty("houseName")]
        public string HouseName { get; set; }
        [JsonProperty("houseNumber")]
        public string HouseNumber { get; set; }
        [JsonProperty("areaCode")]
        public long AreaCode { get; set; }
        [JsonProperty("areaName")]
        public string AreaName { get; set; }
        [JsonProperty("tepmlateImage")]
        public bool TemplateImage { get; set; }

    }
}
