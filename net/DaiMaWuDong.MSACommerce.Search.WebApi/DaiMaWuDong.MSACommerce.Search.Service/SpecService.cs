using Autofac.Extras.DynamicProxy;
using DaiMaWuDong.AgileFramework.PollyExtend;
using DaiMaWuDong.MSACommerce.Search.Interface;
using DaiMaWuDong.MSACommerce.Search.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Search.Service
{
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效  类上标记
    public class SpecService : ISpecService
    {
        [PollyPolicyConfig(FallBackMethod = "QuerySpecParamsFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
                       /*CacheTTLMilliseconds = 1000 * 20*/)] //方法声明上标记
        public List<TbSpecParam> QuerySpecParams(object value1, long id, bool v, object value2)
        {
            throw new NotImplementedException();
        }
    }
}
