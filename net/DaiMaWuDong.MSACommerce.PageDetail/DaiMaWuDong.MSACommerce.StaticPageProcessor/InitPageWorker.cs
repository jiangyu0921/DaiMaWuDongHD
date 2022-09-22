using DaiMaWuDong.AgileFramework.Common.QueueModel;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.Common.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.StaticPageProcessor
{
    /// <summary>
    /// workerItem
    /// </summary>
    public class InitPageWorker : BackgroundService
    {
        #region Identity
        private readonly IConfiguration _configuration;
        private readonly ILogger<InitPageWorker> _logger;
        private readonly RabbitMQInvoker _RabbitMQInvoker;
        private readonly AbstractConsulDispatcher _AbstractConsulDispatcher = null;

        public InitPageWorker(ILogger<InitPageWorker> logger, RabbitMQInvoker rabbitMQInvoker, IConfiguration configuration, AbstractConsulDispatcher abstractConsulDispatcher)
        {
            _logger = logger;
            _RabbitMQInvoker = rabbitMQInvoker;
            _configuration = configuration;
            _AbstractConsulDispatcher = abstractConsulDispatcher;

        }
        #endregion

        #region 具体动作
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RabbitMQConsumerModel rabbitMQConsumerModel = new RabbitMQConsumerModel()
            {
                ExchangeName = RabbitMQExchangeQueueName.SKUCQRS_Exchange,
                QueueName = RabbitMQExchangeQueueName.SKUCQRS_Queue_StaticPage
            };
            HttpClient _HttpClient = new HttpClient();

            _RabbitMQInvoker.RegistReciveAction(rabbitMQConsumerModel, message =>
            {
                SPUCQRSQueueModel skuCQRSQueueModel = JsonConvert.DeserializeObject<SPUCQRSQueueModel>(message);

                string detailUrl = _AbstractConsulDispatcher.MapAddress(_configuration["DetailPageUrl"]);
                string totalUrl = null;
                switch (skuCQRSQueueModel.CQRSType)
                {
                    case (int)SPUCQRSQueueModelType.Insert:
                        totalUrl = $"{detailUrl}{skuCQRSQueueModel.SpuId}.html";//新增，访问一下就自动生成一下
                        break;
                    case (int)SPUCQRSQueueModelType.Update:
                        totalUrl = $"{detailUrl}{skuCQRSQueueModel.SpuId}.html";
                        break;
                    case (int)SPUCQRSQueueModelType.Delete:
                        totalUrl = $"{detailUrl}{skuCQRSQueueModel.SpuId}.html?ActionHeader=Delete";//也可以用RequestHeader来传递
                        break;
                    default:
                        break;
                }

                try
                {
                    var result = _HttpClient.GetAsync(totalUrl).Result;
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        _logger.LogInformation($"{nameof(InitPageWorker)}.Init succeed {totalUrl}");
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"{nameof(InitPageWorker)}.Init succeed {totalUrl}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    var logModel = new LogModel()
                    {
                        OriginalClassName = GetType().FullName,
                        OriginalMethodName = nameof(ExecuteAsync),
                        Remark = "定时作业错误日志"
                    };
                    _logger.LogError(ex, $"{nameof(InitPageWorker)}.Init failed {totalUrl}, Exception:{ex.Message}", JsonConvert.SerializeObject(logModel));
                    return false;
                }
            });
            await Task.CompletedTask;
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);
            //}
        }
        #endregion
    }
}
