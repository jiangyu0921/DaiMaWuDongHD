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
    public class SpecService : ISpecService
    {
        private readonly string BASE_URL = "http://SpecMicroservice/api/item/Spec";

        private IHttpClientFactory _invoker;
        private readonly AbstractConsulDispatcher _dispatcher;

        public SpecService(
            IHttpClientFactory invoker,
            AbstractConsulDispatcher dispatcher)
        {
            _invoker = invoker;
            _dispatcher = dispatcher;
        }

        public List<TbSpecGroup> QuerySpecsByCid(long cid3)
        {
            string url = $"{BASE_URL}/{cid3}";
            string realUrl = _dispatcher.MapAddress(url);
            string content = _invoker.CreateClient().GetStringAsync(realUrl).Result;
            var specGrupList = JsonConvert.DeserializeObject<List<TbSpecGroup>>(content)!;

            return specGrupList;
        }

        public List<TbSpecGroup> QuerySpecsByCidFallback(long cid3)
        {
            return new List<TbSpecGroup>();
        }
    }
}
