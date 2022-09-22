using Autofac.Extras.DynamicProxy;
using DaiMaWuDong.AgileFramework.PollyExtend;
using DaiMaWuDong.MSACommerce.Search.Model;
using DaiMaWuDong.MSACommerce.Search.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Search.Interface
{
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效  类上标记
    public interface IGoodsService
    {
        [PollyPolicyConfig(FallBackMethod = "QueryBrandByBidFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
                       /*CacheTTLMilliseconds = 1000 * 20*/)]
        List<TbSku> QuerySkuBySpuId(long id);

        [PollyPolicyConfig(FallBackMethod = "QueryBrandByBidFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
                       /*CacheTTLMilliseconds = 1000 * 20*/)]
        PageResult<TbSpu> QuerySpuByPage(int page, int rows, string key, bool? saleable);

        [PollyPolicyConfig(FallBackMethod = "QueryBrandByBidFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
                       /*CacheTTLMilliseconds = 1000 * 20*/)]
        TbSpu QuerySpuBySpuId(long spuId);

        [PollyPolicyConfig(FallBackMethod = "QueryBrandByBidFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
                       /*CacheTTLMilliseconds = 1000 * 20*/)]
        TbSpuDetail QuerySpuDetailBySpuId(long id);
    }
}
