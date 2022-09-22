using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.HttpApiExtend;
using DaiMaWuDong.MSACommerce.Search.Interface;
using DaiMaWuDong.MSACommerce.Search.Model.Models;
using Newtonsoft.Json;

namespace DaiMaWuDong.MSACommerce.Search.Service
{
    public class BrandService : IBrandService
    {
        private IHttpAPIInvoker _invoker;
        private readonly AbstractConsulDispatcher _dispatcher;

        public BrandService(IHttpAPIInvoker invoker,
                            AbstractConsulDispatcher dispatcher)
        {
            _invoker = invoker;
            _dispatcher = dispatcher;
        }

        public TbBrand QueryBrandByBid(long brandId)
        {
            string url = $"http://BrandMicroservice/api/item/Brand/{brandId}";
            string realUrl = this._dispatcher.MapAddress(url);
            string content = this._invoker.InvokeApi(realUrl);
            var brand = JsonConvert.DeserializeObject<TbBrand>(content)!;

            return brand;
        }

        public TbBrand QueryBrandByBidFallback(long brandId)
        {
            return new TbBrand();
        }

        public List<TbBrand> QueryBrandByIds(List<long> brandIds)
        {
            string url = $"http://BrandMicroservice/api/item/Brand/list";
            string realUrl = _dispatcher.MapAddress(url);
            string content = _invoker.InvokeApi(realUrl, HttpMethod.Post, JsonConvert.SerializeObject(brandIds));
            var brand = JsonConvert.DeserializeObject<List<TbBrand>>(content)!;

            return brand;
        }

        public List<TbBrand> QueryBrandByIdsFallback(List<long> brandIds)
        {
            return new List<TbBrand>();
        }
    }
}