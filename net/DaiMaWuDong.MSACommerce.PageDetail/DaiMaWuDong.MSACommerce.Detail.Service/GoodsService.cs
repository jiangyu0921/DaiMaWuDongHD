using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.MSACommerce.Detail.Interface;
using DaiMaWuDong.MSACommerce.Detail.Model.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Detail.Service
{
    public class GoodsService : IGoodsService
    {
        private RedisClusterHelper _cacheClientDB;
        private IHttpClientFactory _invoker;
        private readonly AbstractConsulDispatcher _dispatcher;

        private readonly string BASE_URL = "http://GoodsMicroservice/api/item/Goods";

        public GoodsService(RedisClusterHelper cacheClientDB,
            IHttpClientFactory invoker,
            AbstractConsulDispatcher dispatcher)
        {
            _cacheClientDB = cacheClientDB;
            _invoker = invoker;
            _dispatcher = dispatcher;
        }

        public TbSpu QuerySpuBySpuId(long spuId)
        {
            string url = $"{BASE_URL}/spu/{spuId}";
            string realUrl = _dispatcher.MapAddress(url);
            string content = _invoker.CreateClient().GetStringAsync(realUrl).Result;
            var spu = JsonConvert.DeserializeObject<TbSpu>(content)!;

            return spu;
        }

        public TbSpu QuerySpuBySpuIdFallback(long spuId)
        {
            return new TbSpu();
        }

        public List<long> QuerySpuIdsPage(int pageIndex, int count)
        {
            throw new System.NotImplementedException();
        }
    }
}
