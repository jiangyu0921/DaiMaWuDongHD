using DaiMaWuDong.AgileFramework.Common;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Detail.Interface;
using Newtonsoft.Json;
using System.Net;

namespace DaiMaWuDong.MSACommerce.StaticPageProcessor
{
    public class WarmupPageWorker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WarmupPageWorker> _logger;
        private readonly RabbitMQInvoker _RabbitMQInvoker;
        private readonly IGoodsService _IGoodsService = null;

        public WarmupPageWorker(ILogger<WarmupPageWorker> logger, RabbitMQInvoker rabbitMQInvoker, IConfiguration configuration, IGoodsService goodsService)
        {
            _logger = logger;
            _RabbitMQInvoker = rabbitMQInvoker;
            _configuration = configuration;
            _IGoodsService = goodsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RabbitMQConsumerModel rabbitMQConsumerModel = new RabbitMQConsumerModel()
            {
                ExchangeName = RabbitMQExchangeQueueName.SKUWarmup_Exchange,
                QueueName = RabbitMQExchangeQueueName.SKUWarmup_Queue_StaticPage
            };
            HttpClient _HttpClient = new HttpClient();
            _RabbitMQInvoker.RegistReciveAction(rabbitMQConsumerModel, message =>
            {
                SKUWarmupQueueModel skuWarmupQueueModel = JsonConvert.DeserializeObject<SKUWarmupQueueModel>(message);
                #region 先ClearAll
                {
                    string totalUrl = $"{_configuration["DetailPageUrl"]}{12580}.html?ActionHeader=ClearAll";
                    try
                    {
                        var result = _HttpClient.GetAsync(totalUrl).Result;
                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            _logger.LogInformation($"{nameof(WarmupPageWorker)}.ClearAll succeed {totalUrl}");
                            //return true;
                        }
                        else
                        {
                            _logger.LogWarning($"{nameof(InitPageWorker)}.ClearAll failed {totalUrl}");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{nameof(InitPageWorker)}.ClearAll failed {totalUrl}, Exception:{ex.Message}");
                        return false;
                    }
                }
                #endregion

                #region 然后全部创建  Warmup
                {
                    //限流？ 白名单----分批---记录当下等

                    int count = 100;//单次查询
                    int pageIndex = 1;//分页的页码七点
                    while (count == 100)
                    {
                        List<long> ids = _IGoodsService.QuerySpuIdsPage(pageIndex, count);
                        foreach (var id in ids)
                        {
                            string totalUrl = $"{_configuration["DetailPageUrl"]}{id}.html";
                            try
                            {
                                var result = _HttpClient.GetAsync(totalUrl).Result;
                                if (result.StatusCode == HttpStatusCode.OK)
                                {
                                    _logger.LogInformation($"{nameof(InitPageWorker)}.Warmup succeed {totalUrl}");
                                    //return true;
                                }
                                else
                                {
                                    _logger.LogWarning($"{nameof(InitPageWorker)}.Warmup failed {totalUrl}");
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
                                _logger.LogError(ex, $"{nameof(InitPageWorker)}.Warmup failed {totalUrl}, Exception:{ex.Message}", JsonConvert.SerializeObject(logModel));
                                return false;
                            }
                        }
                        pageIndex++;
                        count = ids.Count;
                    }
                }
                #endregion
                return true;
            });
            await Task.CompletedTask;
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);
            //}
        }
    }
}
