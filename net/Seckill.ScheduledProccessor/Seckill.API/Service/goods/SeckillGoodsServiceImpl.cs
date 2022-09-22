using CSRedis;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Seckill.API.DTO;
using Seckill.Common.Helper;

namespace Seckill.API.Service.goods
{
    public class SeckillGoodsServiceImpl : ISeckillGoodsService
    {
        private readonly CSRedisClient _redisHelper;
        private readonly IMemoryCache _memoryCache;

        public SeckillGoodsServiceImpl(CSRedisHelper redisHelper, IMemoryCache memoryCache)
        {
            this._redisHelper = redisHelper.Client();
            this._memoryCache = memoryCache;
        }
        public List<TbSeckillGoodDTO> ListGoods(string key)
        {
            List<TbSeckillGoodDTO> seckillGoods = new();
            var goodsList = _redisHelper.HVals("SeckillGoods_" + key);
            foreach (var goodsString in goodsList)
            {
                TbSeckillGoodDTO seckillGood = JsonConvert.DeserializeObject<TbSeckillGoodDTO>(goodsString);
                seckillGoods.Add(seckillGood);
            }

            return seckillGoods;
        }

        public TbSeckillGoodDTO QuerySeckillGoodsDetail(string time, long goodsid)
        {
            // return GetDataFromMemoryCache(time, goodsid);
            return GetDataFromRedis(time, goodsid);
        }

        /// <summary>
        /// 保证Redis必须有预热数据的
        /// </summary>
        /// <param name="time"></param>
        /// <param name="goodsid"></param>
        /// <returns></returns>
        private TbSeckillGoodDTO GetDataFromRedis(string time, long goodsid)
        {
            string cacheKey = goodsid + "";
            TbSeckillGoodDTO dto = _redisHelper.HGet<TbSeckillGoodDTO>("SeckillGoods_" + time, cacheKey);

            return dto;
        }

        /// <summary>
        /// 保证Redis必须有预热数据的
        /// </summary>
        /// <param name="time"></param>
        /// <param name="goodsid"></param>
        /// <returns></returns>
        private TbSeckillGoodDTO GetDataFromMemoryCache(string time, long goodsid)
        {
            string cacheKey = goodsid + "";
            TbSeckillGoodDTO dto;
            // 首先从Cache中获取数据
            string productJson = _memoryCache.Get<string>(cacheKey);
            if (productJson == null)
            {
                Console.WriteLine("=======From Redis========");
                // 说明应用程序缓存中不存在，找Redis拿数据
                dto = _redisHelper.HGet<TbSeckillGoodDTO>("SeckillGoods_" + time, cacheKey);
                // 把数据保存到Cache中,设置120s有效期
                _memoryCache.Set(cacheKey, JsonConvert.SerializeObject(dto), TimeSpan.FromSeconds(120));
            }
            else
            {
                Console.WriteLine("=========From MemoryCache==========");
                // 不为空直接获取结果
                dto = JsonConvert.DeserializeObject<TbSeckillGoodDTO>(productJson);
            }

            return dto;
        }
    }
}
