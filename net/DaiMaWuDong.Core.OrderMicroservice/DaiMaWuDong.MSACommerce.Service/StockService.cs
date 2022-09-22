using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.MSACommerce.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Service
{
    public class StockService : IStockService
    {
        private IHttpClientFactory _invoker;
        private readonly AbstractConsulDispatcher _dispatcher;

        private readonly string BASE_URL = "http://StockMicroservice/api/item/Goods";

        public StockService(RedisClusterHelper cacheClientDB,
            IHttpClientFactory invoker,
            AbstractConsulDispatcher dispatcher)
        {
            _invoker = invoker;
            _dispatcher = dispatcher;
        }

        public bool DecreaseStock(List<CartDto> carts)
        {
            string url = $"{BASE_URL}/decrease";
            string realUrl = _dispatcher.MapAddress(url);
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.Content = new StringContent(JsonConvert.SerializeObject(carts), System.Text.Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri(realUrl);
            HttpResponseMessage response = _invoker.CreateClient().SendAsync(message).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var flag = JsonConvert.DeserializeObject<bool>(result)!;

            return flag;
        }

        public bool DecreaseStockFallback(List<CartDto> carts)
        {
            return true;
        }
    }
}
