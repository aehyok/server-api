using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.RPC
{
    public class RpcResultBase<T>
    {
        public long Id { get; set; }
        public T Result { get; set; }
    }
}
