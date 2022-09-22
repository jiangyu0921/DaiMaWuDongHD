using Seckill.API.Models;

namespace Seckill.API.Service.order
{
    public interface ISeckillOrderService
    {
        /// <summary>
        /// 添加秒杀订单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        bool AddSeckillOrder(long id, string time, string username);

        /// <summary>
        /// 根据用户名查询订单状态
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        SeckillStatus QueryStatus(string username);
    }
}
