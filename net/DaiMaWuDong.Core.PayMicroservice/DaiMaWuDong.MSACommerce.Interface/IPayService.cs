using DaiMaWuDong.AgileFramework.WechatPayCore;
using DaiMaWuDong.Common.Model;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace DaiMaWuDong.Core.PayMicroservice
{
    public interface IPayService
    {
        /// <summary>
        /// 获取WX支付链接的方法
        /// 然后发布定时同步状态任务
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="user">用户信息</param>
        /// <param name="httpContext">请求上下文</param>
        /// <returns>返回生成的支持链接</returns>
        string GenerateUrl(long orderId, UserInfo user, HttpContext httpContext);
        /// <summary>
        /// 处理微信支付回调
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        WxPayData HandleNotify(HttpContext httpContext);

        /// <summary>
        /// 只负责数据查询订单支付状态
        /// 根据订单ID查询订单支付状态，只负责查询，数据库没有回去调用API查询
        /// 以TbPayLog数据为准
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        int QueryOrderStateByOrderId(long orderId);

        /// <summary>
        /// 基于Redis检查是否需要更新支付状态，
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool CheckIsNeedRefreshOrderPayStatusRedis(long orderId);
        /// <summary>
        /// 设置不需要刷新了
        /// </summary>
        /// <param name="orderId"></param>
        void SetIsNeedRefreshOrderPayStatusRedis(long orderId);
        /// <summary>
        /// 直接去微信拿状态，不管数据库，不管redis
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        WxPayData QueryOrderStateFromWechatByOrderId(long orderId);
        /// <summary>
        /// 根据微信支付返回的数据更新数据库的Pay-Log，且发布到订单的任务
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="wxPayData"></param>
        /// <returns></returns>
        bool UpdatePayStatus(long orderId, WxPayData wxPayData);
    }
}
