using Autofac.Extras.DynamicProxy;
using DaiMaWuDong.AgileFramework.PollyExtend;
using DaiMaWuDong.MSACommerce.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Interface
{
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效  类上标记
    public interface IGoodsService
    {
        [PollyPolicyConfig(FallBackMethod = "QuerySkusByIdsFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
                       /*CacheTTLMilliseconds = 1000 * 20*/)] //方法声明上标记
        List<TbSku> QuerySkusByIds(List<long> list);
    }
}
