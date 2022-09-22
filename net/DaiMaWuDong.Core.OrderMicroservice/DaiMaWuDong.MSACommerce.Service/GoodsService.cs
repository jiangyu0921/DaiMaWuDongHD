using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Common;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.MSACommerce.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model;
using DaiMaWuDong.MSACommerce.Model.Models;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Service
{
    public class GoodsService : IGoodsService
    {
        private IHttpClientFactory _invoker;
        private readonly AbstractConsulDispatcher _dispatcher;

        private readonly string BASE_URL = "http://GoodsMicroservice/api/item/Goods";

        public GoodsService(RedisClusterHelper cacheClientDB,
            IHttpClientFactory invoker,
            AbstractConsulDispatcher dispatcher)
        {
            _invoker = invoker;
            _dispatcher = dispatcher;
        }

        public List<TbSku> QuerySkusByIds(List<long> list)
        {
            string url = $"{BASE_URL}/sku/list/ids";
            string realUrl = _dispatcher.MapAddress(url);
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.Content = new StringContent(JsonConvert.SerializeObject(list), System.Text.Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri(realUrl);
            HttpResponseMessage response = _invoker.CreateClient().SendAsync(message).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var skuList = JsonConvert.DeserializeObject<List<TbSku>>(result)!;

            return skuList;
        }

        public List<TbSku> QuerySkusByIdsFallback(List<long> list)
        {
            return new List<TbSku>();
        }
    }
}
