using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seckill.Common.Result
{
    public class Result
    {
        public bool Flag { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public Result(bool flag, int code, string message)
        {
            Flag = flag;
            Code = code;
            Message = message;
        }

        public Result(bool flag, int code, string message, object data) : this(flag, code, message)
        {
            Data = data;
        }
    }
}
