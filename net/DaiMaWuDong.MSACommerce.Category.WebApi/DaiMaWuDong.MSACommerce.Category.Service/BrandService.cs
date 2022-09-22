using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.HttpApiExtend;
using DaiMaWuDong.MSACommerce.Category.Interface;
using DaiMaWuDong.MSACommerce.Category.Model.Models;
using Newtonsoft.Json;

namespace DaiMaWuDong.MSACommerce.Category.Service
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

        /// <summary>
        /// 远程调用品牌微服务
        /// </summary>
        /// <param name="bid"></param>
        /// <returns></returns>
        public TbBrand QueryBrandByBid(long bid)
        {
            string url = $"http://BrandMicroservice/api/item/Brand/{bid}";
            string realUrl = this._dispatcher.MapAddress(url);
            string content = this._invoker.InvokeApi(realUrl);
            var brand = JsonConvert.DeserializeObject<TbBrand>(content)!;

            return brand;
        }

        public TbBrand QueryBrandByBidFallback(long bid)
        {
            Console.WriteLine("调用了->QueryBrandByBidFallback");
            return new TbBrand
            {
                Id = bid
            };
        }

        public List<TbCategory> QueryCategoryByBid(long bid)
        {
            return null;
        }
    }
}