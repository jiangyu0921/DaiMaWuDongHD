
using DaiMaWuDong.AgileFramework.Common;
using DaiMaWuDong.AgileFramework.WechatPayCore;
using DaiMaWuDong.Common.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DaiMaWuDong.Core.PayMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {

        private IPayService _IPayService;
        public PayController(IPayService payService)
        {
            this._IPayService = payService;
        }

        /// <summary>
        /// 生成微信支付链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/api/pay/url/{id}")]
        [HttpGet]
        public JsonResult GenerateUrl(long id)
        {
            UserInfo user = base.HttpContext.GetCurrentUserInfo();
            string url = this._IPayService.GenerateUrl(id, user, base.HttpContext);

            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "生成url成功",
                Value = url
            });
        }

        /// <summary>
        /// 支付回调API---不走网关
        /// </summary>
        /// <returns></returns>
        [Route("/api/pay/wxpay/notify")]
        [HttpPost]
        public async Task PayNotify()
        {
            WxPayData wxPayData = this._IPayService.HandleNotify(this.HttpContext); // 处理回调结果
            WxPayData res = new WxPayData();
            res.SetValue("return_code", wxPayData.GetValue("return_code"));
            res.SetValue("return_msg", wxPayData.GetValue("return_msg"));
            HttpContext.Response.ContentType = "application/xml";
            string xmlResult = res.ToXml();
            Console.WriteLine($"微信支付回调结果：{xmlResult}");
            await HttpContext.Response.WriteAsync(xmlResult);
        }

        /// <summary>
        /// 查询订单支付状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("state/{id}")]
        [HttpGet]
        public JsonResult QueryOrderStateByOrderId(long id)
        {
            var state = this._IPayService.QueryOrderStateByOrderId(id);

            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "查询成功",
                Value = state
            });
        }
    }
}
