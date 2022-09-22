using DaiMaWuDong.MSACommerce.Goods.Model;
using DaiMaWuDong.MSACommerce.Goods.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Goods.Interface
{
    public interface IGoodsService
    {
        PageResult<TbSpu> QuerySpuByPage(int page, int rows, string key, bool? saleable);

        TbSpuDetail QuerySpuDetailBySpuId(long spuId);

        List<TbSku> QuerySkuBySpuId(long spuId);

        void DeleteGoodsBySpuId(long spuId);

        void AddGoods(TbSpu spu);

        void UpdateGoods(TbSpu spu);

        void HandleSaleable(TbSpu spu);

        TbSpu QuerySpuBySpuId(long spuId);

        List<long> QuerySpuIdsPage(int page, int pageSize);
        List<TbSku> QuerySkusByIds(List<long> ids);
    }
}
