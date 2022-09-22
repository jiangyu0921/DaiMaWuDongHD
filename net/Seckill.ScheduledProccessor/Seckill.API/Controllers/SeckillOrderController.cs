using Microsoft.AspNetCore.Mvc;
using Seckill.API.Models;
using Seckill.API.Service.order;
using Seckill.Common.Helper;
using Seckill.Common.Result;

namespace Seckill.API.Controllers
{
    [ApiController]
    public class SeckillOrderController : ControllerBase
    {
        private readonly ISeckillOrderService _seckillOrderService;

        public SeckillOrderController(ISeckillOrderService seckillOrderService)
        {
            this._seckillOrderService = seckillOrderService;
        }
        /// <summary>
        /// 抢购API
        /// </summary>
        /// <param name="time"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/seckill/order/add")]
        public Result AddSeckillOrder(string time, long id)
        {
            try
            {
                // 用户登录名
                //string username = "zhaoxigerry";
                // 随机生成用户名
                string username = StringHelper.GetRandomString(8);
                bool bo = _seckillOrderService.AddSeckillOrder(id, time, username);

                if (bo)
                {
                    //抢单成功
                    return new Result(true, SeckillStatusCode.OK, "排队成功！");
                }
            }
            catch (Exception)
            {
                return new Result(true, SeckillStatusCode.REPERROR, "不允许重复下单");
            }
            return new Result(true, SeckillStatusCode.ERROR, "服务器繁忙，请稍后再试");
        }

        [HttpGet("/api/seckill/order/query/{username}")]
        public Result QueryStatus(string username)
        {
            //获取用户名 【代表是登录用户】
            //string username = "zhaoxigerry";

            //根据用户名查询用户抢购状态
            SeckillStatus seckillStatus = _seckillOrderService.QueryStatus(username);

            if (seckillStatus != null)
            {
                return new Result(true, seckillStatus.Status, "查询状态信息", seckillStatus);
            }
            //NOTFOUNDERROR =20006,没有对应的抢购数据
            return new Result(false, SeckillStatusCode.NOTFOUNDERROR, "商品秒完了");
        }
    }
}
