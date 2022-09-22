using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Service
{
    public class OrderService : IOrderService
    {
        private IHttpClientFactory _invoker;
        private readonly AbstractConsulDispatcher _dispatcher;

        private readonly string BASE_URL = "http://OrderMicroservice/api/order";

        public OrderService(
            IHttpClientFactory invoker,
            AbstractConsulDispatcher dispatcher)
        {
            _invoker = invoker;
            _dispatcher = dispatcher;
        }

        public OrderInfoCore GetOrderInfoCoreRemote(long orderId)
        {
            string url = $"{BASE_URL}/{orderId}";
            string realUrl = _dispatcher.MapAddress(url);
            string content = _invoker.CreateClient().GetStringAsync(realUrl).Result;
            AjaxResult result = JsonConvert.DeserializeObject<AjaxResult>(content)!;
            if (result.Result)
            {
                return JsonConvert.DeserializeObject<OrderInfoCore>(result.Value.ToString()!)!;
            }
            else
            {
                throw new Exception("order信息获取失败");
            }
        }
    }
}
