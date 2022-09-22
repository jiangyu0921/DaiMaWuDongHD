using Autofac.Extras.DynamicProxy;
using DaiMaWuDong.AgileFramework.PollyExtend;
using DaiMaWuDong.MSACommerce.Goods.DTOModel.DTO;

namespace DaiMaWuDong.MSACommerce.Goods.Interface
{
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效  类上标记
    public interface IBrandService
    {
        [PollyPolicyConfig(FallBackMethod = "QueryBrandByIdFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
                       /*CacheTTLMilliseconds = 1000 * 20*/)] //方法声明上标记
        Dictionary<long, string> QueryBrandById(List<GoodsCategoryBrandDTO> dto);
    }
}