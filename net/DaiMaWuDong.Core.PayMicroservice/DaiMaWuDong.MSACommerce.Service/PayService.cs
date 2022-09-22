using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.AgileFramework.WechatPayCore;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.DTOModel;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model;
using DaiMaWuDong.MSACommerce.Model.Models;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace DaiMaWuDong.Core.PayMicroservice.Domain
{
    public class PayService : IPayService
    {
        private readonly OrangePayContext _orangePayContext;
        private readonly PayHelper _payHelper;
        private readonly IOrderService _orderService;
        private readonly ILogger<PayService> _logger;
        private readonly RedisClusterHelper _cacheClientDB;
        private readonly Notify _Notify;
        private readonly ICapPublisher _iCapPublisher;

        public PayService(OrangePayContext orangeStockContext,
            PayHelper payHelper,
            IOrderService orderService,
            ILogger<PayService> logger, RedisClusterHelper cacheClientDB,
            RabbitMQInvoker rabbitMQInvoker, Notify notify,
            ICapPublisher capPublisher)
        {
            _orangePayContext = orangeStockContext;
            _payHelper = payHelper;
            _orderService = orderService;
            _logger = logger;
            _cacheClientDB = cacheClientDB;
            _Notify = notify;
            _iCapPublisher = capPublisher;
        }

        /// <summary>
        /// 获取WX支付链接的方法
        /// 然后发布定时同步状态任务
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="user">用户信息</param>
        /// <param name="httpContext">请求上下文</param>
        /// <returns>返回生成的支持链接</returns>
        public string GenerateUrl(long orderId, UserInfo user, HttpContext httpContext)
        {
            OrderInfoCore order = _orderService.GetOrderInfoCoreRemote(orderId);//服务的直接调用
            //判断订单状态
            if (order.orderStatus.Status != (int)OrderStatusEnum.INIT)
            {
                throw new Exception("订单状态异常");
            }

            string redisKey = $"{CreatePayUrl_RedisKeyPrefix}_{orderId}";
            string payUrl = null;
            if (_cacheClientDB.ContainsKey(redisKey))
            {
                payUrl = _cacheClientDB.Get(redisKey);
            }
            else
            {
                _logger.LogInformation("准备生成支付链接.....");
                payUrl = _payHelper.CreatePayUrl(orderId, "易淘电商微信支付链接", order.TotalPay, httpContext);
                _cacheClientDB.Set(redisKey, payUrl, TimeSpan.FromHours(2));//2h有效期
                _logger.LogInformation("生成支付链接为:{payUrl}", payUrl);
            }

            //优先去支付日志表中查询信息
            TbPayLog payLog = _orangePayContext.TbPayLog.Where(l => l.OrderId == orderId).FirstOrDefault()!;
            if (payLog == null)
            {
                // 生成支付日志信息
                TbPayLog newPayLog = new TbPayLog()
                {
                    OrderId = orderId,
                    TotalFee = order.ActualPay,
                    UserId = user.id,
                    PayType = 1,
                    Status = (int)PayStatusEnum.NOT_PAY
                };
                _orangePayContext.TbPayLog.Add(newPayLog);
                _orangePayContext.SaveChanges(); //写入支付日志
            }

            return payUrl;
        }

        private string CreatePayUrl_RedisKeyPrefix = "Create_Pay_Url";


        /// <summary>
        /// 处理微信支付回调
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public WxPayData HandleNotify(HttpContext httpContext)
        {
            WxPayData wxPayData = _Notify.ProcessNotify(httpContext);

            long orderId = long.Parse(wxPayData.GetValue("out_trade_no").ToString()!);  //订单编号
            if (!CheckIsNeedRefreshOrderPayStatusRedis(orderId))
            {
                _logger.LogWarning($"微信回调支付{orderId},已有更新记录");
                return wxPayData;
            }

            UpdatePayStatus(orderId, wxPayData);

            #region 后续动作---也可以考虑异步化
            SetIsNeedRefreshOrderPayStatusRedis(orderId);//1小时内回调不管了
            string redisKey = $"{CreatePayUrl_RedisKeyPrefix}_{orderId}";//删除Redis支付链接
            _cacheClientDB.Remove(redisKey);
            #endregion

            return wxPayData;
        }

        private string RefreshOrderPayStatus_RedisKeyPrefix = "Check_Order_Pay_Status";
        /// <summary>
        /// 基于Redis检查是否需要更新支付状态，
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool CheckIsNeedRefreshOrderPayStatusRedis(long orderId)
        {
            string key = $"{RefreshOrderPayStatus_RedisKeyPrefix}_{orderId}";

            if (_cacheClientDB.ContainsKey(key))//检查是否存在key，存在则不需要更新了
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 设置不需要刷新了
        /// </summary>
        /// <param name="orderId"></param>
        public void SetIsNeedRefreshOrderPayStatusRedis(long orderId)
        {
            string key = $"{RefreshOrderPayStatus_RedisKeyPrefix}_{orderId}";
            _cacheClientDB.Set(key, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), TimeSpan.FromHours(1));
        }

        /// <summary>
        /// 直接去微信拿状态，不管数据库，不管redis
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public WxPayData QueryOrderStateFromWechatByOrderId(long orderId)
        {
            return _payHelper.QueryOrderById(orderId);
        }

        /// <summary>
        /// 根据微信支付返回的数据更新数据库的Pay-Log，且发布到订单的任务
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="wxPayData"></param>
        /// <returns></returns>
        public bool UpdatePayStatus(long orderId, WxPayData wxPayData)
        {
            #region 先检测数据
            long totalFee = long.Parse(wxPayData.GetValue("total_fee").ToString()!);//订单金额
            string transactionId = wxPayData.GetValue("transaction_id").ToString()!;//商户订单号
            string bankType = wxPayData.GetValue("bank_type").ToString()!;//银行类型

            OrderInfoCore tbOrder = _orderService.GetOrderInfoCoreRemote(orderId);
            if (1 != totalFee)//tbOrder.ActualPay---当下只是做了个假数据比较
            {
                _logger.LogError("【微信支付回调】支付回调返回数据不正确");
                throw new WxPayException("支付参数不正常");
            }
            #endregion

            #region 修改订单状态--未分库
            //#region 修改支付日志状态
            //Console.WriteLine("修改支付日志状态");
            //TbPayLog payLog = _orangePayContext.TbPayLog.First(l => l.OrderId == orderId);
            ////未支付的订单才需要更改----重试机制
            //if (payLog.Status == (int)PayStatusEnum.NOT_PAY)
            //{
            //    payLog.OrderId = orderId;
            //    payLog.BankType = bankType;
            //    payLog.PayTime = DateTime.Now;
            //    payLog.TransactionId = transactionId;
            //    payLog.Status = (int)PayStatusEnum.SUCCESS;
            //}
            //#endregion

            //Console.WriteLine("修改订单状态");
            //TbOrderStatus updateOrderStatus = _orangeContext.TbOrderStatus.First(o => o.OrderId == orderId);

            //updateOrderStatus.Status = (int)OrderStatusEnum.PAY_UP;
            //updateOrderStatus.OrderId = orderId;
            //updateOrderStatus.PaymentTime = DateTime.Now;

            //// 提交所有更改
            //_orangePayContext.SaveChanges();
            //_orangeContext.SaveChanges();
            #endregion
            //return true;

            #region 修改订单状态--分库后异步化
            IDbContextTransaction trans = null;
            try
            {
                trans = _orangePayContext.Database.BeginTransaction(_iCapPublisher, autoCommit: false);

                #region 修改支付日志状态
                Console.WriteLine("修改支付日志状态");
                TbPayLog payLog = _orangePayContext.TbPayLog.First(l => l.OrderId == orderId);
                //未支付的订单才需要更改----重试机制
                if (payLog.Status == (int)PayStatusEnum.NOT_PAY)
                {
                    payLog.OrderId = orderId;
                    payLog.BankType = bankType;
                    payLog.PayTime = DateTime.Now;
                    payLog.TransactionId = transactionId;
                    payLog.Status = (int)PayStatusEnum.SUCCESS;

                    _iCapPublisher.Publish(name: RabbitMQExchangeQueueName.Pay_Order_UpdateStatus,
                    contentObj: new PayOrderStatusDto()
                    {
                        PayStatus = 2,//表示已支付
                        OrderId = orderId
                    }, headers: new Dictionary<string, string>());
                    _orangePayContext.SaveChanges();
                    Console.WriteLine("头一次回调，执行更新");
                    trans.Commit();
                }
                #endregion
                return true;
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
            #endregion
        }

        /// <summary>
        /// 只负责数据查询订单支付状态
        /// 根据订单ID查询订单支付状态，只负责查询，数据库没有回去调用API查询
        /// 以TbPayLog数据为准
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public int QueryOrderStateByOrderId(long orderId)
        {
            //优先去支付日志表中查询信息
            TbPayLog payLog = _orangePayContext.TbPayLog.FirstOrDefault(l => l.OrderId == orderId)!;

            if ((int)PayStatusEnum.NOT_PAY == payLog.Status)
            {
                return (int)PayStatusEnum.NOT_PAY;
            }
            if ((int)PayStatusEnum.SUCCESS == payLog.Status)
            {
                //支付成功，返回1
                return (int)PayStatusEnum.SUCCESS;
            }
            //如果是其他状态，返回失败
            return (int)PayStatusEnum.FAIL;
        }
    }
}
