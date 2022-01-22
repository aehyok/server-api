using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Common.Infrastructures
{
    public class MessageResult<T>
    {
        public T Data;
        public bool Flag = false;
        public string Message = "";

        public MessageResult(string message, bool flag, T data)
        {
            this.Data = data;
            this.Flag = flag;
            this.Message = message;
        }

        public MessageResult(string message, bool flag = false)
        {

            this.Flag = flag;
            this.Message = message;
        }

        public MessageResult()
        {

        }
    }
}

