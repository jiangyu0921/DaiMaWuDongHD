using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seckill.Common.Result
{
    public class SeckillStatusCode
    {
        public static readonly int OK = 20000;//成功
        public static readonly int ERROR = 20001;//失败
        public static readonly int LOGINERROR = 20002;//用户名或密码错误
        public static readonly int ACCESSERROR = 20003;//权限不足
        public static readonly int REMOTEERROR = 20004;//远程调用失败
        public static readonly int REPERROR = 20005;//重复操作
        public static readonly int NOTFOUNDERROR = 20006;//没有对应的抢购数据
    }
}
