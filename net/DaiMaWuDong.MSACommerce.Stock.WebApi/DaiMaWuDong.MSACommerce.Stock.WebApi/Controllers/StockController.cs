using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Stock.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Stock.Interface;
using DaiMaWuDong.MSACommerce.Stock.Model.Models;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection.PortableExecutable;

namespace DaiMaWuDong.MSACommerce.Stock.WebApi.Controllers
{
    /// <summary>
    /// 分布式事务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        #region Identity
        private readonly IStockService _iStockService;
        private readonly IStockManagerService _IStockManagerService;
        private readonly IConfiguration _iConfiguration;
        private readonly OrangeContext _OrangeStockContext;
        private readonly ILogger<StockController> _Logger;
        private readonly ICapPublisher _iCapPublisher;
        public StockController(IConfiguration configuration, OrangeContext userServiceDbContext, ILogger<StockController> logger,
            IStockService stockService, IStockManagerService stockManagerService, ICapPublisher iCapPublisher)
        {
            _iConfiguration = configuration;
            _OrangeStockContext = userServiceDbContext;
            _Logger = logger;
            _iStockService = stockService;
            _IStockManagerService = stockManagerService;
            _iCapPublisher = iCapPublisher;
        }
        #endregion

        [Route("test")]
        [HttpGet]
        //public JsonResult Test(int index)
        //{
        //    OrderCartDto orderCartDto = new OrderCartDto()
        //    {
        //        Carts = new List<CartDto>()
        //        {
        //            new CartDto()
        //            {
        //                skuId=2600242,
        //                num=10
        //            },
        //            new CartDto()
        //            {
        //                skuId=2600248,
        //                num=10
        //            }
        //        },
        //        OrderId = 1234567777
        //    };

        //    if (index == 1)
        //    {
        //        _iCapPublisher.Publish(name: RabbitMQExchangeQueueName.Order_Stock_Decrease, contentObj: orderCartDto, headers: null);
        //    }
        //    else if (index == 2)
        //    {
        //        _iCapPublisher.Publish(name: RabbitMQExchangeQueueName.Order_Stock_Resume, contentObj: orderCartDto, headers: null);
        //    }

        //    //this._iStockService.DecreaseStock(orderCartDto.carts, orderCartDto.OrderId);

        //    //this._iStockService.ResumeStock(orderCartDto.carts, orderCartDto.OrderId);

        //    return new JsonResult(new
        //    {
        //        Result = true,
        //        Msg = "Succeed"
        //    });
        //}


        [HttpGet]
        [Route("init/{skuId}")]
        public JsonResult InitStock(long skuId)
        {
            _IStockManagerService.ForceInitRedisStockBySkuId(skuId);
            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "更新成功"
            });
        }




        #region 下单减库存
        [NonAction]
        [CapSubscribe(RabbitMQExchangeQueueName.Order_Stock_Decrease)]
        public void DecreaseStockByOrder(OrderCartDto orderCartDto, [FromCap] CapHeader header)
        {
            IDbContextTransaction trans = null!;
            try
            {
                Console.WriteLine($@"{DateTime.Now} DecreaseStockByOrder invoked, Info: {Newtonsoft.Json.JsonConvert.SerializeObject(orderCartDto)}");
                trans = _OrangeStockContext.Database.BeginTransaction(_iCapPublisher, autoCommit: false);
                _iStockService.DecreaseStock(trans, orderCartDto.Carts, orderCartDto.OrderId);
                _iCapPublisher.Publish(name: RabbitMQExchangeQueueName.Stock_Logistics, contentObj: orderCartDto, headers: new Dictionary<string, string>()!);
                _OrangeStockContext.SaveChanges();
                Console.WriteLine("减库存====>数据库业务数据已经插入,操作完成");
                trans.Commit();
                _Logger.LogWarning($"This is EFCoreTransaction Invoke");
            }
            catch (Exception ex)
            {
                Console.WriteLine("****************************************************");
                Console.WriteLine(ex.Message);
                if (trans != null)
                {
                    trans.Rollback();
                }
            }
        }
        #endregion

        #region 取消订单恢复库存
        [NonAction]
        [CapSubscribe(RabbitMQExchangeQueueName.Order_Stock_Resume)]
        public void ResumeStockByOrder(OrderCartDto orderCartDto, [FromCap] CapHeader header)
        {
            try
            {
                Console.WriteLine($@"{DateTime.Now} ResumeStockByOrder invoked, Info: {Newtonsoft.Json.JsonConvert.SerializeObject(orderCartDto)}");
                _iStockService.ResumeStock(orderCartDto.Carts, orderCartDto.OrderId);
                Console.WriteLine("恢复库存====>数据库业务数据已经插入,操作完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine("****************************************************");
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion


    }
}
