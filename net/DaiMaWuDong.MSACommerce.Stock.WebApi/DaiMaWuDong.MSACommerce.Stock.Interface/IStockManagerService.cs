namespace DaiMaWuDong.MSACommerce.Stock.Interface
{
    public interface IStockManagerService
    {
        /// <summary>
        /// 初始化全局的库存
        /// 锁定+单例----TODO 分布式锁
        /// </summary>
        void InitRedisStock();
        /// <summary>
        /// 以数据库现有数据为准，恢复单个Stock缓存
        /// </summary>
        void ForceInitRedisStockBySkuId(long skuId);
    }
}