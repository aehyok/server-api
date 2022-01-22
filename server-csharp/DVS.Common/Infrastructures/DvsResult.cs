using Microsoft.AspNetCore.Mvc;

namespace DVS.Common.Infrastructures
{
    public class DvsResult : JsonResult
    {
        public DvsResult(object value)
            : base(value)
        { }
    }
}