using CSRedis;
using Newtonsoft.Json;
using Seckill.API.Models;
using Seckill.API.MultiThread;
using Seckill.API.Service.goods;
using Seckill.Common.Helper;
using Seckill.Common.Result;
using Seckill.EntityFrameworkCore.Modles;

namespace Seckill.API.Service.order
{
    public class SeckillOrderServiceImpl : ISeckillOrderService
    {
        private readonly YitaoSeckillDBContext _dbContext;
        private readonly CSRedisClient _redisHelper;
        private readonly ISeckillGoodsService _goodService;
        private readonly MultiThreadingCreateOrder _multiThreadCreate;

        public SeckillOrderServiceImpl(
            YitaoSeckillDBContext dbContext,
            CSRedisHelper redisHelper,
            ISeckillGoodsService seckillGoodsService,
            MultiThreadingCreateOrder threadingCreateOrder
            )
        {
            this._dbContext = dbContext;
            this._redisHelper = redisHelper.Client();
            this._goodService = seckillGoodsService;
            this._multiThreadCreate = threadingCreateOrder;
        }

        /// <summary>
        /// 添加秒杀订单信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool AddSeckillOrder(long id, string time, string username)
        {
            #region 单线程下单实现
            /*//获取Redis中的商品数据
            var goods = _goodService.QuerySeckillGoodsDetail(time, id);
            //如果没有库存，则直接抛出异常
            if (goods == null || goods.Num <= 0)
            {
                throw new Exception("已售罄!");
            }
            //如果有库存，则创建秒杀商品订单
            TbSeckillOrder seckillOrder = new()
            {
                Id = Snowflake.Next(), // 雪花算法
                SeckillId = id,
                Money = goods.Price,
                UserId = username,
                CreateTime = DateTime.Now,
                Status = "0"
            };
            //库存减少
            goods.Num = goods.Num - 1; // 库存原子问题
            //将秒杀订单存入到Redis中
            _redisHelper.HSet("SeckillOrder", username, JsonConvert.SerializeObject(seckillOrder));


            //判断当前商品是否还有库存
            if (goods.Num <= 0)
            {
                // 把DTO转换为对应实体对象
                var goodsDtoJson = JsonConvert.SerializeObject(goods);
                var goodsEntity = JsonConvert.DeserializeObject<TbSeckillGood>(goodsDtoJson);
                //并且将商品数据同步到MySQL中
                _dbContext.TbSeckillGoods.Update(goodsEntity);
                _dbContext.SaveChanges();
                //如果没有库存,则清空Redis缓存中该商品
                _redisHelper.HDel("SeckillGoods_" + time, goods.Id);
            }
            else
            {
                //如果有库存，则直数据重置到Reids中【更新商品库存】
                _redisHelper.HSet("SeckillGoods_" + time, goods.Id, JsonConvert.SerializeObject(goods));
            }*/
            #endregion
            #region 多线程下单+排队实现
            // //递增，判断是否排队
            long userQueueCount = _redisHelper.HIncrBy("UserQueueCount_" + id, username, 1);
            if (userQueueCount > 1)
            {
                //20005：表示有重复抢单
                throw new Exception(SeckillStatusCode.REPERROR.ToString());
            }
            //排队信息封装
            SeckillStatus seckillStatus = new(username, DateTime.Now, 1, id, time);
            ////将秒杀抢单信息存入到Redis中,这里采用List方式存储,List本身是一个队列
            _redisHelper.LPush("SeckillOrderQueue", JsonConvert.SerializeObject(seckillStatus));
            // //// 设置排队状态标识
            _redisHelper.HMSet("UserQueueStatus", username, JsonConvert.SerializeObject(seckillStatus));
            //Console.WriteLine("我执行中....");
            _multiThreadCreate.CreateOrder();
            Console.WriteLine("Main已经执行完了。。。。。");
            #endregion

            return true;
        }

        /// <summary>
        /// 根据用户名查询订单状态
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public SeckillStatus QueryStatus(string username)
        {
            return JsonConvert.DeserializeObject<SeckillStatus>(_redisHelper.HGet("UserQueueStatus", username));
        }
    }
}
