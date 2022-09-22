using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.MSACommerce.Detail.Interface;
using DaiMaWuDong.MSACommerce.Detail.Model.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace DaiMaWuDong.MSACommerce.Detail.Service
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

        public TbBrand QueryBrandByBid(long brandId)
        {
            string url = $"http://BrandMicroservice/api/item/Brand/{brandId}";
            string realUrl = _dispatcher.MapAddress(url);
            string content = _invoker.CreateClient().GetStringAsync(realUrl).Result;
            var brand = JsonConvert.DeserializeObject<TbBrand>(content)!;

            return brand;
        }

        public TbBrand QueryBrandByBidFallback(long brandId)
        {
            return new TbBrand();
        }
    }
}