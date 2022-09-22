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
    public class CategoryService : ICategoryService
    {
        private IHttpClientFactory _invoker;
        private readonly AbstractConsulDispatcher _dispatcher;

        public CategoryService(IHttpClientFactory invoker,
                            AbstractConsulDispatcher dispatcher)
        {
            _invoker = invoker;
            _dispatcher = dispatcher;
        }


        public List<TbCategory> QueryCategoryByIds(List<long> cid3s)
        {
            string url = $"http://CategoryMicroservice/api/Category/list/ids";
            string realUrl = _dispatcher.MapAddress(url);
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.Content = new StringContent(JsonConvert.SerializeObject(cid3s), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri(realUrl);
            HttpResponseMessage response = _invoker.CreateClient().SendAsync(message).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var categoryList = JsonConvert.DeserializeObject<List<TbCategory>>(result)!;

            return categoryList;
        }

        public List<TbCategory> QueryCategoryByIdsFallback(List<long> cid3s)
        {
            return new List<TbCategory>();
        }
    }
}
