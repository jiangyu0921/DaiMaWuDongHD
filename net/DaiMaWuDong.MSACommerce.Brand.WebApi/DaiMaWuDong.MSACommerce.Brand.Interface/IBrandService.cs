using DaiMaWuDong.MSACommerce.Brand.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Brand.Model;
using DaiMaWuDong.MSACommerce.Brand.Model.Models;

namespace DaiMaWuDong.MSACommerce.Brand.Interface
{
    public interface IBrandService
    {
        PageResult<TbBrand> QueryBrandByPageAndSort(int page, int rows, string sortBy, bool desc, string key);

        void SaveBrand(TbBrand brand, List<long> cids);

        void UpdateBrand(BrandBo brandbo);

        void DeleteBrand(long bid);

        List<TbBrand> QueryBrandByCid(long cid);

        TbBrand QueryBrandByBid(long id);

        List<TbBrand> QueryBrandByIds(List<long> ids);
        Dictionary<long, string> QueryBrandNameByIds(List<GoodsCategoryBrandDTO> dto);
    }
}