using CSRedis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Seckill.API.Models;
using Seckill.API.Service.goods;
using Seckill.Common.Helper;
using Seckill.EntityFrameworkCore.Modles;

namespace Seckill.API.MultiThread
{
    public class MultiThreadingCreateOrder
    {
        private readonly CSRedisClient _redisHelper;
        private readonly ISeckillGoodsService _goodService;

        public MultiThreadingCreateOrder(
            CSRedisHelper redisHelper,
            ISeckillGoodsService seckillGoodsService)
        {
            this._redisHelper = redisHelper.Client();
            this._goodService = seckillGoodsService;
        }
        /// <summary>
        /// 多线程下单操作[真正抢单过程处理]
        /// </summary>
        public void CreateOrder()
        {
            //开启线程 内部已经配置了线程池
            Task.Run(() =>
            {
                // 获取dbContext构建对象
                var optionsBuilder = new DbContextOptionsBuilder<YitaoSeckillDBContext>();
                var contextOptions = optionsBuilder.UseMySql("server=192.168.1.103;database=yitao_seckill;user=root;password=123456;treattinyasboolean=true",
                    ServerVersion.Parse("8.0.21-mysql")).Options;

                // 从排队List中获取排队的信息（左边存储，右边取）
                var obj = _redisHelper.RPop("SeckillOrderQueue");
                // 
                SeckillStatus seckillStatus = null;
                if (obj != null)
                {
                    // 从redis中取出obj并反序列化为SeckillStatus
                    seckillStatus = JsonConvert.DeserializeObject<SeckillStatus>(obj);
                }

                try
                {
                    if (seckillStatus != null)
                    {
                        //获取排队信息
                        string time = seckillStatus.Time;
                        long id = seckillStatus.GoodsId;//秒杀商品的ID
                        string username = seckillStatus.Username;

                        //判断 先从队列中获取商品 ,如果能获取到,说明 有库存,如果获取不到,说明 没库存 卖完了 return.
                        string o = _redisHelper.RPop("SeckillGoodsCountList_" + id);
                        if (o == null)
                        {
                            //卖完了
                            //清除 掉  防止重复排队的key
                            //_redisHelper.HDel("UserQueueCount", username);
                            //清除 掉  排队标识(存储用户的抢单信息)
                            //_redisHelper.HDel("UserQueueStatus", username);
                            throw new Exception("秒活动结束!");
                        }

                        //获取商品详情数据
                        // 10 200个线程同时拿到了这个10
                        // 加锁，分布式锁，在高并发系统中极端降低处理响应效率
                        // 采用是内存队列.,如何设计内存队列呢？（说白了 高并发秒杀系统其实商品库存竞争），队列天生线程安全(FIFO)
                        var goods = _goodService.QuerySeckillGoodsDetail(time, id);

                        //如果没有库存，则直接抛出异常
                        if (goods == null || goods.Num <= 0)
                        {
                            throw new Exception("已售罄!");
                        }

                        //如果有库存，则创建秒杀商品订单
                        TbSeckillOrder seckillOrder = new()
                        {
                            Id = Snowflake.Next(), // 通过雪花算法
                            SeckillId = id,
                            Money = goods.Price,
                            UserId = username,
                            CreateTime = DateTime.Now,
                            Status = "0"
                        };

                        //将秒杀订单存入到Redis中
                        _redisHelper.HSet("SeckillOrder", username, JsonConvert.SerializeObject(seckillOrder));
                        //库存减少
                        //goods.Num--;

                        //5.减库存
                        long increment = _redisHelper.HIncrBy("SeckillGoodsCount", id + "", -1L);
                        goods.Num = (int)increment;

                        //判断当前商品是否还有库存
                        if (goods.Num <= 0)
                        {
                            // 把Dto转化为TbSeckillGood
                            var goodsJson = JsonConvert.SerializeObject(goods);
                            var goodsEntity = JsonConvert.DeserializeObject<TbSeckillGood>(goodsJson);
                            //并且将商品数据同步到MySQL中
                            SyncDataToMySQL(goodsEntity, contextOptions);
                            // 同步秒杀订单到数据库
                        }
                        else
                        {
                            //Console.WriteLine($"秒杀库存剩余量为：{goods.Num}");
                            //如果有库存，则直数据重置到Reids中
                            _redisHelper.HSet("SeckillGoods_" + time, goods.Id, JsonConvert.SerializeObject(goods));
                        }

                        //7.创建订单成功()
                        /*try
                        {
                            Console.WriteLine("开始模拟下单操作=====start====" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Thread.CurrentThread.ManagedThreadId);
                            Thread.Sleep(30000);
                            Console.WriteLine("开始模拟下单操作=====end====" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Thread.CurrentThread.ManagedThreadId);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("下单出现异常");
                        }*/

                        // 修改抢购状态
                        //抢单成功，更新抢单状态,排队->等待支付
                        seckillStatus.Status = 2;
                        seckillStatus.OrderId = seckillOrder.Id;
                        seckillStatus.Money = float.Parse(seckillOrder.Money.ToString());
                        _redisHelper.HSet("UserQueueStatus", username, JsonConvert.SerializeObject(seckillStatus));
                        Console.WriteLine($"秒杀库存剩余量为：{goods.Num}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine("子线程已经执行完成");
            });

        }

        /// <summary>
        /// 同步数据到数据库中
        /// </summary>
        /// <param name="goodsEntity"></param>
        /// <param name="time"></param>
        /// <param name="goodsId"></param>
        public void SyncDataToMySQL(TbSeckillGood goodsEntity, DbContextOptions<YitaoSeckillDBContext> contextOptions)
        {
            using var context = new YitaoSeckillDBContext(contextOptions);
            //并且将商品数据同步到MySQL中
            context.TbSeckillGoods.Update(goodsEntity);
            context.SaveChanges();
            //如果没有库存,则清空Redis缓存中该商品
            // _redisHelper.HDel("SeckillGoods_" + time, goodsId);

        }
    }
}
