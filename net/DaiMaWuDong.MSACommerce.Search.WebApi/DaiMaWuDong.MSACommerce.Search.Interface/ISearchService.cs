using DaiMaWuDong.MSACommerce.Search.Model.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Search.Interface
{
    public interface ISearchService
    {
        SearchResult<Goods> Search(SearchRequest searchRequest);
        public void ImpDataBySpu();
        public SearchResult<Goods> GetData(SearchRequest searchRequest);
        public Goods GetGoodsBySpuId(long spuId);
    }
}
