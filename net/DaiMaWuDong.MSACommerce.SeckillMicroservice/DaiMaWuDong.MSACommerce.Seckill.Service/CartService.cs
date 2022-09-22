using DaiMaWuDong.AgileFramework.Consul;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace DaiMaWuDong.MSACommerce.Seckill.Service
{
    public class CartService : ICartService
    {
        private readonly string BASE_URL = "http://CartMicroservice/api/cart/delete/batch";
        private IHttpClientFactory _invoker;
        private AbstractConsulDispatcher _dispatcher;

        public CartService(
            IHttpClientFactory invoker,
            AbstractConsulDispatcher dispatcher)
        {
            _invoker = invoker;
            _dispatcher = dispatcher;
        }

        public void DeleteCarts(List<long> ids, long userId)
        {
            string url = $"{BASE_URL}/{userId}";
            string realUrl = _dispatcher.MapAddress(url);
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.Content = new StringContent(JsonConvert.SerializeObject(ids), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri(realUrl);
            HttpResponseMessage response = _invoker.CreateClient().SendAsync(message).Result;
            var result = response.Content.ReadAsStringAsync().Result;
        }

        public void DeleteCartsFallback(List<long> ids)
        {

        }
    }
}