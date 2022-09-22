
using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.MSACommerce.Stock.DTOModel;
using DaiMaWuDong.MSACommerce.Stock.Interface;
using DaiMaWuDong.MSACommerce.Stock.Model.Models;

namespace DaiMaWuDong.MSACommerce.Stock.Service
{
    public class StockManagerService : IStockManagerService
    {
        private OrangeContext _orangeStockContext;
        private readonly RedisClusterHelper _cacheClientDB;

        public StockManagerService(OrangeContext orangeStockContext, RedisClusterHelper cacheClientDB)
        {
            _orangeStockContext = orangeStockContext;
            _cacheClientDB = cacheClientDB;
        }

        #region InitRedisStock
        private static readonly object InitRedisStock_Lock = new object();
        private static bool Is_InitRedisStock = false;

        /// <summary>
        /// 初始化全局的库存
        /// 锁定+单例----TODO 分布式锁
        /// </summary>
        public void InitRedisStock()
        {
            if (!Is_InitRedisStock)
            {
                lock (InitRedisStock_Lock)
                {
                    if (!Is_InitRedisStock)
                    {
                        InitRedisStockCore();
                    }
                }
            }
        }

        private void InitRedisStockCore()
        {
            int index = 1;
            int size = 100;
            int originalSize = size;
            while (size == originalSize)
            {
                List<TbStock> stockList = _orangeStockContext.TbStock.Where(s => s.SkuId > 0)
                                                                            .OrderBy(s => s.SkuId)
                                                                            .Skip((index - 1) * size)
                                                                            .Take(size)
                                                                            .ToList();
                foreach (var stock in stockList)
                {
                    string key = $"{CommonConfigConstant.StockRedisKeyPrefix}{stock.SkuId}";
                    if (!_cacheClientDB.ContainsKey(key))
                    {
                        _cacheClientDB.Set(key, stock.Stock.ToString());
                    }
                }
                index++;
                size = stockList.Count;
            }
        }
        #endregion

        public void ForceInitRedisStockBySkuId(long skuId)
        {
            var stock = _orangeStockContext.TbStock.First(s => s.SkuId == skuId);
            string key = $"{CommonConfigConstant.StockRedisKeyPrefix}{stock.SkuId}";
            _cacheClientDB.Set(key, stock.Stock.ToString());
        }

    }
}