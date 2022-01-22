using System;
using System.Collections.Generic;
using System.Text;

namespace SLQRCode.Model.Common
{
  public class ResultInfo<T>
  {
    public string Message { get; set; }
    public int Code { get; set; }
    public T Data { get; set; }

    public ResultInfo<T> SetValue(T data, string message = "", int code = 0) {
      return new ResultInfo<T>() { Message=message,Code=code,Data=data };
    }
  }
}
