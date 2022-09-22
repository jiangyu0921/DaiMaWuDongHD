using Autofac.Extras.DynamicProxy;
using DaiMaWuDong.AgileFramework.PollyExtend;
using DaiMaWuDong.MSACommerce.Search.Model.Models;

namespace DaiMaWuDong.MSACommerce.Search.Interface
{
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效
    public interface IBrandService
    {
        [PollyPolicyConfig(FallBackMethod = "QueryBrandByBidFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
                       /*CacheTTLMilliseconds = 1000 * 20*/)]
        TbBrand QueryBrandByBid(long brandId);

        [PollyPolicyConfig(FallBackMethod = "QueryBrandByIdsFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
                       /*CacheTTLMilliseconds = 1000 * 20*/)]
        List<TbBrand> QueryBrandByIds(List<long> brandIds);
    }
}