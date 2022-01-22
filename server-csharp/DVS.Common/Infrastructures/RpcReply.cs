using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Common.Infrastructures
{
    public class RpcReply<T>
    {
        public T Body { get; set; }
        public int Code { get; set; }

        public string Message { get; set; }
    }
}
