using DaiMaWuDong.MSACommerce.Detail.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Detail.Model;
using DaiMaWuDong.MSACommerce.Detail.Model.Models;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Detail.Interface
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

        void DecreaseStock(List<CartDto> cartDtos);
    }
}