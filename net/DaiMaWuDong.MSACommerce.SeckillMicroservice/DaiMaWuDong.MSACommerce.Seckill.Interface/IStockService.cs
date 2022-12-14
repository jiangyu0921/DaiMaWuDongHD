using Autofac.Extras.DynamicProxy;
using DaiMaWuDong.AgileFramework.PollyExtend;
using DaiMaWuDong.MSACommerce.Seckill.DTOModel.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Seckill.Interface
{
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效  类上标记
    public interface IStockService
    {
        [PollyPolicyConfig(FallBackMethod = "DecreaseStockFallback",
                       IsEnableCircuitBreaker = true,
                       ExceptionsAllowedBeforeBreaking = 3,
                       MillisecondsOfBreak = 1000 * 5
                       /*CacheTTLMilliseconds = 1000 * 20*/)] //方法声明上标记
        bool DecreaseStock(List<CartDto> carts);
    }
}
