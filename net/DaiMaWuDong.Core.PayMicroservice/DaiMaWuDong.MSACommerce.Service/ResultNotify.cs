using DaiMaWuDong.AgileFramework.WechatPayCore;
using Microsoft.AspNetCore.Http;

namespace DaiMaWuDong.Core.PayMicroservice.Domain
{
    public class ResultNotify : Notify
    {
        private readonly WxPayApi _WxPayApi = null;
        public ResultNotify(WxPayApi wxPayApi)
        {
            _WxPayApi = wxPayApi;
        }

        /// <summary>
        /// 处理支付回调
        /// </summary>
        /// <returns></returns>
        public override WxPayData ProcessNotify(HttpContext httpContext)
        {
            _httpContext = httpContext;
            WxPayData notifyData = GetNotifyData();
            //解析数据
            string totalFee = notifyData.GetValue("total_fee").ToString()!;  //订单金额
            string outTradeNo = notifyData.GetValue("out_trade_no").ToString()!;  //订单编号
            string transactionId = notifyData.GetValue("transaction_id").ToString()!;  //商户订单号
            string bankType = notifyData.GetValue("bank_type").ToString()!;  //银行类型

            Console.WriteLine($"=============>{totalFee}=={outTradeNo}=={transactionId}=={bankType}");
            if (totalFee.Equals("") || outTradeNo.Equals("") || transactionId.Equals("") || bankType.Equals(""))
            {
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付回调返回数据不正确");
                Console.WriteLine(GetType().ToString(), "支付错误结果 : " + res.ToXml());
                return res;
            }

            //检查支付结果中transaction_id是否存在--流水号
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                Console.WriteLine(GetType().ToString(), "支付错误结果 : " + res.ToXml());

                return res;
            }

            string transaction_id = notifyData.GetValue("transaction_id").ToString()!;

            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id, httpContext))
            {
                //若订单查询失败，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                Console.WriteLine(GetType().ToString(), "订单查询失败 : " + res.ToXml());

                return res;
            }
            //查询订单成功
            else
            {
                // 打印结果 （仅打印）
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
                Console.WriteLine(GetType().ToString(), "订单查成功: " + res.ToXml());

                // 设置返回响应结果
                notifyData.SetValue("return_code", "SUCCESS");
                notifyData.SetValue("return_msg", "OK");

                return notifyData;
            }
        }

        /// <summary>
        /// 根据流水号查询订单信息
        /// </summary>
        /// <param name="transaction_id">订单id</param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private bool QueryOrder(string transaction_id, HttpContext httpContext)
        {
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);
            WxPayData res = _WxPayApi.OrderQuery(req, httpContext);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
