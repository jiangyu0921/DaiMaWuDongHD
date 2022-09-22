using Autofac.Extras.DynamicProxy;
using DaiMaWuDong.AgileFramework.PollyExtend;
using DaiMaWuDong.MSACommerce.Category.Model.Models;

namespace DaiMaWuDong.MSACommerce.Category.Interface
{
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效
    public interface IBrandService
    {
        // 通过品牌微服务获取信息
        List<TbCategory> QueryCategoryByBid(long bid);

        // 通过类别三级类库ID获取品牌信息
        [PollyPolicyConfig(FallBackMethod = "QueryBrandByBidFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
                       /*CacheTTLMilliseconds = 1000 * 20*/)]
        TbBrand QueryBrandByBid(long bid);
    }
}