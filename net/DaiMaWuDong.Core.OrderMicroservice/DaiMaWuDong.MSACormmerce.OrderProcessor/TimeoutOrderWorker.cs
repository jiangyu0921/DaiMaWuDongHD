using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Common;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model;
using Newtonsoft.Json;

namespace DaiMaWuDong.MSACormmerce.OrderProcessor
{
    /// <summary>
    /// 超时订单的处理方式
    /// </summary>
    public class TimeoutOrderWorker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TimeoutOrderWorker> _logger;
        private readonly RabbitMQInvoker _RabbitMQInvoker;
        private readonly IOrderService _IOrderService;
        private readonly RedisClusterHelper _cacheClientDB;

        public TimeoutOrderWorker(ILogger<TimeoutOrderWorker> logger,
            RabbitMQInvoker rabbitMQInvoker, IConfiguration configuration,
            IOrderService orderService, RedisClusterHelper cacheClientDB)
        {
            this._logger = logger;
            this._RabbitMQInvoker = rabbitMQInvoker;
            this._configuration = configuration;
            this._IOrderService = orderService;
            this._cacheClientDB = cacheClientDB;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RabbitMQConsumerModel rabbitMQConsumerModel = new RabbitMQConsumerModel()
            {
                ExchangeName = RabbitMQExchangeQueueName.OrderCreate_Delay_Exchange,
                QueueName = RabbitMQExchangeQueueName.OrderCreate_Delay_Queue_CancelOrder
            };
            this._RabbitMQInvoker.RegistReciveAction(rabbitMQConsumerModel, message =>
            {
                try
                {
                    OrderCreateQueueModel orderCreateQueueModel = JsonConvert.DeserializeObject<OrderCreateQueueModel>(message)!;
                    var orderStatus = this._IOrderService.GetOrderStatus(orderCreateQueueModel.OrderId);
                    // 处理超时位付款的订单
                    if (orderStatus != null && orderStatus.Value == (int)OrderStatusEnum.INIT)
                    {
                        // 关闭订单
                        bool bResult = this._IOrderService.CloseOrder(orderCreateQueueModel.OrderId);
                        //如果是秒杀订单，关闭订单需增加Redis库存
                        if (orderCreateQueueModel.OrderType == OrderCreateQueueModel.OrderTypeEnum.Seckill)
                        {
                            // 考虑下秒杀已结束的恢复  TODO
                            // this._cacheClientDB.IncrementValueInHash(SeckillService.KEY_PREFIX_STOCK, orderCreateQueueModel.SkuIdList[0].ToString(), 1);
                        }
                        return bResult;
                    }
                    else
                    {
                        this._logger.LogWarning($"{nameof(TimeoutOrderWorker)}.CancelOrder complate message={message}, 未修改状态，当时状态为{orderStatus}");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    LogModel logModel = new LogModel()
                    {
                        OriginalClassName = this.GetType().FullName,
                        OriginalMethodName = nameof(ExecuteAsync),
                        Remark = "定时作业错误日志"
                    };
                    this._logger.LogError(ex, $"{nameof(TimeoutOrderWorker)}.CancelOrder failed message={message}, Exception:{ex.Message}", JsonConvert.SerializeObject(logModel));
                    return false;
                }
            });
            await Task.CompletedTask;
        }
    }
}