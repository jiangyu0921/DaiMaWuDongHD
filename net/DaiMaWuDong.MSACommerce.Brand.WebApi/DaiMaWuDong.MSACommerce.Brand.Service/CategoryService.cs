using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.HttpApiExtend;
using DaiMaWuDong.MSACommerce.Brand.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Brand.Service
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

        public List<long> QueryBrandIdByCid(long cid3)
        {
            string url = $"http://CategoryMicroservice/api/category/{cid3}";
            string realUrl = this._dispatcher.MapAddress(url);
            string content = this._invoker.InvokeApi(realUrl);
            var brandIdList = JsonConvert.DeserializeObject<List<long>>(content)!;

            return brandIdList;
        }

        public List<long> QueryBrandIdByCidFallback(long cid3)
        {
            Console.WriteLine("CategoryService=>调用了降级方法");
            return new List<long>();
        }
    }
}
