using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.MSACommerce.Stock.DTOModel;
using DaiMaWuDong.MSACommerce.Stock.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Stock.Interface;
using DaiMaWuDong.MSACommerce.Stock.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Stock.Service
{
    public class StockService : IStockService
    {
        private OrangeContext _orangeStockContext;
        private readonly CacheClientDB _cacheClientDB;

        public StockService(OrangeContext orangeStockContext, CacheClientDB cacheClientDB)
        {
            _orangeStockContext = orangeStockContext;
            _cacheClientDB = cacheClientDB;
        }

        /// <summary>
        /// 增加库存
        /// </summary>
        /// <param name="spu"></param>
        public void IncreaseStock()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 恢复库存，以订单为单位
        /// </summary>
        /// <param name="spu"></param>
        public void ResumeStock(List<CartDto> cartDtos, long orderId)
        {
            IDbContextTransaction trans = null;
            try
            {
                trans = _orangeStockContext.Database.BeginTransaction();
                if (!_orangeStockContext.Set<TbStockLog>().Any(l => l.OrderId == orderId && l.StockStatus == (int)TbStockLogEnum.StockStatus.Normal))
                {
                    Console.WriteLine("订单需要恢复的库存流水不存在");//可以细致处理
                    return;
                }

                foreach (CartDto cartDto in cartDtos)
                {
                    int count = _orangeStockContext.Database.ExecuteSqlRaw($"update tb_stock set stock = stock + {cartDto.num} where sku_id = {cartDto.skuId}");
                    if (count != 1)
                    {
                        throw new Exception("恢复库存失败");
                    }
                    int countLog = _orangeStockContext.Database.ExecuteSqlRaw($"update tb_stock_log set stock_status ={(int)TbStockLogEnum.StockStatus.Backoff} where sku_id = {cartDto.skuId} and order_id={orderId} and stock_status={(int)TbStockLogEnum.StockStatus.Normal}");
                    if (countLog != 1)
                    {
                        throw new Exception("恢复库存失败");
                    }
                }
                trans.Commit();

                foreach (CartDto cartDto in cartDtos)
                {
                    #region 增加Redis库存
                    string key = $"{CommonConfigConstant.StockRedisKeyPrefix}{cartDto.skuId}";
                    _cacheClientDB.IncrementValueBy(key, cartDto.num);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
                throw;
            }
            finally
            {
                trans.Dispose();
            }
        }

        /// <summary>
        /// 减少库存
        /// </summary>
        /// <param name="cartDtos"></param>
        public void DecreaseStock(IDbContextTransaction trans, List<CartDto> cartDtos, long orderId)
        {
            try
            {
                foreach (CartDto cartDto in cartDtos)
                {
                    int count = _orangeStockContext.Database.ExecuteSqlRaw($"update tb_stock set stock = stock - {cartDto.num} where sku_id = {cartDto.skuId} and stock >= {cartDto.num}");
                    if (count != 1)
                    {
                        throw new Exception("扣减库存失败");
                    }
                    TbStockLog tbStockLog = new TbStockLog()
                    {
                        SeckillNum = 0,
                        SkuId = cartDto.skuId,
                        OrderId = orderId,
                        OrderNum = cartDto.num,
                        StockStatus = (int)TbStockLogEnum.StockStatus.Normal,
                        StockType = (int)TbStockLogEnum.StockType.Decrease
                    };
                    _orangeStockContext.Set<TbStockLog>().Add(tbStockLog);
                }
                _orangeStockContext.SaveChanges();
            }
            catch (Exception)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
                throw;
            }
            finally
            {
                trans.Dispose();
            }
        }
    }
}
