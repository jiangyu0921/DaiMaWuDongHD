using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.HttpApiExtend;
using DaiMaWuDong.MSACommerce.Search.Interface;
using DaiMaWuDong.MSACommerce.Search.Model.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Search.Service
{
    public class CategoryService : ICategoryService
    {
        private IHttpAPIInvoker _invoker;
        private readonly AbstractConsulDispatcher _dispatcher;

        public CategoryService(IHttpAPIInvoker invoker,
                            AbstractConsulDispatcher dispatcher)
        {
            _invoker = invoker;
            _dispatcher = dispatcher;
        }


        public List<TbCategory> QueryCategoryByIds(List<long> cid3s)
        {
            string url = $"http://CategoryMicroservice/api/Category/list/ids";
            string realUrl = this._dispatcher.MapAddress(url);
            string content = this._invoker.InvokeApi(realUrl,
                                                     HttpMethod.Post,
                                                     JsonConvert.SerializeObject(cid3s));
            var brand = JsonConvert.DeserializeObject<List<TbCategory>>(content)!;

            return brand;
        }

        public List<TbCategory> QueryCategoryByIdsFallback(List<long> cid3s)
        {
            return new List<TbCategory>();
        }
    }
}
