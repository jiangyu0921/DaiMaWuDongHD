using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.HttpApiExtend;
using DaiMaWuDong.MSACommerce.Search.Interface;
using DaiMaWuDong.MSACommerce.Search.Model;
using DaiMaWuDong.MSACommerce.Search.Model.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Search.Service
{
    public class GoodsService : IGoodsService
    {
        private RedisClusterHelper _cacheClientDB;
        private IHttpAPIInvoker _invoker;
        private readonly AbstractConsulDispatcher _dispatcher;

        public GoodsService(RedisClusterHelper cacheClientDB,
            IHttpAPIInvoker invoker,
            AbstractConsulDispatcher dispatcher)
        {
            _cacheClientDB = cacheClientDB;
            _invoker = invoker;
            _dispatcher = dispatcher;
        }

        public List<TbSku> QuerySkuBySpuId(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  查询分页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="value"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public PageResult<TbSpu> QuerySpuByPage(int page, int rows, string key, bool? saleable)
        {
            string url = $"http://GoodsMicroservice/api/item/Goods/sup/page";
            string realUrl = this._dispatcher.MapAddress(url);
            string content = this._invoker.InvokeApi(realUrl);
            var brand = JsonConvert.DeserializeObject<PageResult<TbSpu>>(content)!;

            return brand;
        }

        public PageResult<TbSpu> QueryBrandByBidFallback(int page, int rows, object value, bool v)
        {
            return new PageResult<TbSpu>(-1, new List<TbSpu>());
        }

        public TbSpu QuerySpuBySpuId(long spuId)
        {
            throw new NotImplementedException();
        }

        public TbSpuDetail QuerySpuDetailBySpuId(long id)
        {
            throw new NotImplementedException();
        }
    }
}
