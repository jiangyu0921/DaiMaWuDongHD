using CSRedis;
using Newtonsoft.Json;
using Seckill.Common.Helper;
using Seckill.EntityFrameworkCore.Modles;

namespace Seckill.PreheatProccessor
{
    public class WarmupWorker : BackgroundService
    {
        private readonly ILogger<WarmupWorker> _logger;
        private readonly YitaoSeckillDBContext _dbContext;
        private readonly CSRedisClient _redisHelper;

        public WarmupWorker(ILogger<WarmupWorker> logger, YitaoSeckillDBContext dbContext, CSRedisHelper redisHelper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _redisHelper = redisHelper.Client();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // ��ȡʱ��˵�
                var dateMenus = DateHelper.GetDateMenus();

                foreach (var startTime in dateMenus)
                {
                    // ��ȡ��ʽΪ:2022012421
                    string extName = DateHelper.Data2str(startTime);
                    // ��ȡ������������Ʒ
                    // 1������ʱ������ݲ�ѯ��Ӧ����ɱ��Ʒ����
                    // 1)��Ʒ�������ͨ��  status=1
                    // 2)���>0
                    // 3)��ʼʱ��>=���ʼʱ��
                    // 4)�����ʱ��<��ʼʱ��+2Сʱ
                    // 5)�ų�֮ǰ�Ѿ����ص�Redis�����е���Ʒ����

                    var query = _dbContext.TbSeckillGoods.Where(
                        goods => goods.Status == "1" &&
                        goods.Num > 0 &&
                        goods.StartTime >= startTime && goods.EndTime < startTime.AddHours(2)
                        );
                    // ��ȡredis��ָ��hashid�ĵ�keys
                    var keys = _redisHelper.HKeys("SeckillGoods_" + extName);

                    if (keys != null && keys.Length > 0)
                    {
                        query = query.Where(goods => !keys.Contains(goods.Id.ToString()));

                    }

                    // ��ѯ����
                    List<TbSeckillGood> seckillGoodsList = query.ToList();

                    //����ɱ��Ʒ���ݴ��뵽Redis����
                    seckillGoodsList.ForEach(goods =>
                    {
                        var goodsJson = JsonConvert.SerializeObject(goods);
                        _redisHelper.HSet("SeckillGoods_" + extName, goods.Id.ToString(), goodsJson);
                        _redisHelper.ExpireAt("SeckillGoods_" + extName, startTime.AddHours(2));

                        ///////////////////////// ����������� /////////////////////////////
                        //��Ʒ����ѹ�������
                        PushGoods(goods);
                        //���һ�������� (key:��Ʒ��ID  value : �����)
                        _redisHelper.HIncrBy("SeckillGoodsCount", goods.Id.ToString(), goods.Num!.Value);
                    });
                }

                await Task.Delay(5000, stoppingToken);
            }
        }

        public void PushGoods(TbSeckillGood seckillGoods)
        {
            //����redis�Ķ���(ÿһ����Ʒ����һ������,���е�Ԫ�صĸ�������Ʒ�Ŀ��һ��) ѹ�����
            for (int i = 0; i < seckillGoods.Num; i++)
            {   //5
                // List SeckillGoodsCountList__1001 {1001,1001,1001,1001,1001} // �ڴ����
                _redisHelper.LPush("SeckillGoodsCountList_" + seckillGoods.Id, seckillGoods.Id);
            }
        }

    }
}