using CSRedis;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newtonsoft.Json;
using Quartz;
using Seckill.Common.Helper;
using Seckill.EntityFrameworkCore.Modles;

namespace Seckill.ScheduledProccessor.Job
{
    [DisallowConcurrentExecution] // 防止运行多个相同的实例
    public class SeckillGoodsJob : IJob
    {
        private readonly YitaoSeckillDBContext _dbContext;
        private readonly CSRedisClient _redisHelper;

        public SeckillGoodsJob(YitaoSeckillDBContext dbContext, CSRedisHelper redisHelper)
        {
            _dbContext = dbContext;
            _redisHelper = redisHelper.Client();
        }
        public Task Execute(IJobExecutionContext context)
        {
            // 获取时间菜单
            var dateMenus = DateHelper.GetDateMenus();

            foreach (var startTime in dateMenus)
            {
                // 获取格式为:2022012421
                string extName = DateHelper.Data2str(startTime);
                // 获取符合条件的商品
                // 1、根据时间段数据查询对应的秒杀商品数据
                // 1)商品必须审核通过  status=1
                // 2)库存>0
                // 3)开始时间>=活动开始时间
                // 4)活动结束时间<开始时间+2小时
                // 5)排除之前已经加载到Redis缓存中的商品数据

                var query = _dbContext.TbSeckillGoods.Where(
                    goods => goods.Status == "1" &&
                    goods.Num > 0 &&
                    goods.StartTime >= startTime && goods.EndTime < startTime.AddHours(2)
                    );
                // 获取redis中指定hashid的的keys
                var keys = _redisHelper.HKeys("SeckillGoods_" + extName);

                if (keys != null && keys.Length > 0)
                {
                    query = query.Where(goods => !keys.Contains(goods.Id.ToString()));

                }

                // 查询数据
                List<TbSeckillGood> seckillGoodsList = query.ToList();

                //将秒杀商品数据存入到Redis缓存
                seckillGoodsList.ForEach(goods =>
                {
                    var goodsJson = JsonConvert.SerializeObject(goods);
                    _redisHelper.HSet("SeckillGoods_" + extName, goods.Id.ToString(), goodsJson);
                    _redisHelper.ExpireAt("SeckillGoods_" + extName, startTime.AddHours(2));

                    ///////////////////////// 解决超卖问题 /////////////////////////////
                    //商品数据压入队列中
                    //PushGoods(goods);
                    //添加一个计数器 (key:商品的ID  value : 库存数)
                    //_redisHelper.HIncrBy("SeckillGoodsCount", goods.Id.ToString(), goods.Num.Value);
                });
            }

            return Task.CompletedTask;
        }

        public void PushGoods(TbSeckillGood seckillGoods)
        {
            //创建redis的队列(每一种商品就是一个队列,队列的元素的个数和商品的库存一致) 压入队列
            for (int i = 0; i < seckillGoods.Num; i++)
            {   //5
                // List SeckillGoodsCountList__1001 {1001,1001,1001,1001,1001} // 内存队列
                _redisHelper.LPush("SeckillGoodsCountList_" + seckillGoods.Id, seckillGoods.Id);
            }
        }
    }
}
