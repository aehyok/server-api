using System;

namespace DVS.Common.Models
{
    public class ValidException : Exception
    {
        public int ExceptionCode { get; set; } = 0;

        public ValidException(string message, int code = -1) : base(message)
        {
            this.ExceptionCode = code;
        }
    }
}