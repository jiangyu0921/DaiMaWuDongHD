using Autofac.Extras.DynamicProxy;
using DaiMaWuDong.AgileFramework.PollyExtend;
using DaiMaWuDong.MSACommerce.Seckill.Model.Models;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Seckill.Interface
{
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效  类上标记
    public interface IGoodsService
    {
        [PollyPolicyConfig(FallBackMethod = "QuerySkuByIdFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
        /*CacheTTLMilliseconds = 1000 * 20*/)] //方法声明上标记
        TbSku QuerySkuById(long id);
    }
}