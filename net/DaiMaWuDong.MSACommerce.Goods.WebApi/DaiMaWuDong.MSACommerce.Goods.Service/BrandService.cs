using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.MSACommerce.Goods.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Goods.Interface;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using IHttpClientFactory = System.Net.Http.IHttpClientFactory;

namespace DaiMaWuDong.MSACommerce.Goods.Service
{
    public class BrandService : IBrandService
    {
        private IHttpClientFactory _invoker;
        private readonly AbstractConsulDispatcher _dispatcher;

        public BrandService(IHttpClientFactory invoker,
                            AbstractConsulDispatcher dispatcher)
        {
            _invoker = invoker;
            _dispatcher = dispatcher;
        }

        public Dictionary<long, string> QueryBrandById(List<GoodsCategoryBrandDTO> dto)
        {
            string url = $"http://BrandMicroservice/api/item/Brand/GetBrandNameDic";
            string realUrl = _dispatcher.MapAddress(url);
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.Content = new StringContent(JsonConvert.SerializeObject(dto), System.Text.Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri(realUrl);
            HttpResponseMessage response = _invoker.CreateClient().SendAsync(message).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var cnames = JsonConvert.DeserializeObject<Dictionary<long, string>>(result)!;

            return cnames;
        }
    }
}