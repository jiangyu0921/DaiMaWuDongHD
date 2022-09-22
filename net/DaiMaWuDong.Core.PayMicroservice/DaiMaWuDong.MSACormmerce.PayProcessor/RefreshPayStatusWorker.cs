using DaiMaWuDong.AgileFramework.Common;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.AgileFramework.WechatPayCore;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.Core.PayMicroservice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACormmerce.PayProcessor
{
    /// <summary>
    /// 根据订单号，去微信获取订单的支付状态，并且更新Pay-Log,发布CAP
    /// </summary>
    public class RefreshPayStatusWorker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RefreshPayStatusWorker> _logger;
        private readonly RabbitMQInvoker _RabbitMQInvoker;
        private readonly IPayService _IPayService = null;
        private readonly PayHelper _PayHelper = null;

        public RefreshPayStatusWorker(ILogger<RefreshPayStatusWorker> logger, RabbitMQInvoker rabbitMQInvoker, IConfiguration configuration, IPayService orderService, PayHelper payHelper)
        {
            _logger = logger;
            _RabbitMQInvoker = rabbitMQInvoker;
            _configuration = configuration;
            _IPayService = orderService;
            _PayHelper = payHelper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RabbitMQConsumerModel rabbitMQConsumerModel = new RabbitMQConsumerModel()
            {
                ExchangeName = RabbitMQExchangeQueueName.OrderPay_Exchange,
                QueueName = RabbitMQExchangeQueueName.OrderPay_Queue_RefreshPay
            };
            _RabbitMQInvoker.RegistReciveAction(rabbitMQConsumerModel, message =>
            {
                try
                {
                    OrderCreateQueueModel orderCreateQueueModel = JsonConvert.DeserializeObject<OrderCreateQueueModel>(message);
                    if (!_IPayService.CheckIsNeedRefreshOrderPayStatusRedis(orderCreateQueueModel.OrderId))
                    {
                        //已经更新过   则结束了
                        _logger.LogWarning($"微信回调支付{orderCreateQueueModel.OrderId},已有更新记录");
                        return true;
                    }
                    else
                    {
                        if (orderCreateQueueModel.TryTime > 0)
                            Thread.Sleep(5 * 1000 * orderCreateQueueModel.TryTime);

                        //根据订单id去获取支付状态
                        WxPayData wxPayData = _IPayService.QueryOrderStateFromWechatByOrderId(orderCreateQueueModel.OrderId);
                        var state = wxPayData.GetValue("trade_state");
                        if ("SUCCESS".Equals(state))
                        {
                            if (!_IPayService.CheckIsNeedRefreshOrderPayStatusRedis(orderCreateQueueModel.OrderId))//更新过的
                            {
                                return true;
                            }
                            else//没更新就更新数据库，然后关闭redis
                            {
                                _IPayService.UpdatePayStatus(orderCreateQueueModel.OrderId, wxPayData);
                                _IPayService.SetIsNeedRefreshOrderPayStatusRedis(orderCreateQueueModel.OrderId);
                                return true;
                            }
                        }
                        else
                        {
                            if (orderCreateQueueModel.TryTime >= 50)
                            {
                                _logger.LogWarning($"{nameof(RefreshPayStatusWorker)}.RefreshPayStatus 超过50次放弃 message={message}");
                                return true;
                            }
                            else
                            {
                                orderCreateQueueModel.TryTime += 1;
                                _RabbitMQInvoker.Send(RabbitMQExchangeQueueName.OrderPay_Exchange, JsonConvert.SerializeObject(orderCreateQueueModel));
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogModel logModel = new LogModel()
                    {
                        OriginalClassName = GetType().FullName,
                        OriginalMethodName = nameof(ExecuteAsync),
                        Remark = "定时作业错误日志"
                    };
                    _logger.LogError(ex, $"{nameof(RefreshPayStatusWorker)}.Refresh failed message={message}, Exception:{ex.Message}", JsonConvert.SerializeObject(logModel));
                    return false;
                }
            });
            await Task.CompletedTask;
        }
    }
}
