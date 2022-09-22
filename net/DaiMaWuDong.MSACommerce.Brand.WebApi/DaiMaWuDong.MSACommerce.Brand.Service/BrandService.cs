using DaiMaWuDong.MSACommerce.Brand.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Brand.Interface;
using DaiMaWuDong.MSACommerce.Brand.Model.Models;
using DaiMaWuDong.MSACommerce.Brand.Model;

namespace DaiMaWuDong.MSACommerce.Brand.Service
{
    public class BrandService : IBrandService
    {
        private OrangeContext _orangeContext;
        private ICategoryService _categoryService;

        public BrandService(OrangeContext orangeContext, ICategoryService categroyService)

        {
            _orangeContext = orangeContext;
            _categoryService = categroyService;
        }

        public void DeleteBrand(long bid)
        {
            throw new NotImplementedException();
        }

        public TbBrand QueryBrandByBid(long id)
        {
            TbBrand b1 = _orangeContext.TbBrand.Where(m => m.Id == id).FirstOrDefault();
            if (b1 == null)
            {
                throw new Exception("查询品牌不存在");
            }
            return b1;
        }

        public List<TbBrand> QueryBrandByCid(long cid)
        {
            // 根据三级类别ID获取品牌ID集合
            var brandIdList = _categoryService.QueryBrandIdByCid(cid);
            var brandList = _orangeContext.TbBrand.Where(b => brandIdList.Contains(b.Id)).ToList();
            if (brandList.Count <= 0)
            {
                throw new Exception("没有找到分类下的品牌");
            }
            return brandList;
        }

        public List<TbBrand> QueryBrandByIds(List<long> ids)
        {
            List<TbBrand> brands = _orangeContext.TbBrand.Where(m => ids.Contains(m.Id)).ToList();
            if (brands.Count <= 0)
            {
                throw new Exception("查询品牌不存在");
            }
            return brands;
        }

        public PageResult<TbBrand> QueryBrandByPageAndSort(int page, int rows, string sortBy, bool desc, string key)
        {
            var list = _orangeContext.TbBrand.AsQueryable();

            if (!string.IsNullOrEmpty(key))
            {
                list = list.Where(m => m.Name.Contains(key) || m.Letter == key);
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                String sortByClause = sortBy + (desc ? " DESC" : " ASC");
                if (desc)
                {
                    list.OrderByDescending(m => m.Letter);
                }
            }

            var total = list.Count();
            var tbBrands = list.Take(10).ToList();
            if (tbBrands.Count() <= 0)
            {
                throw new Exception("查询的品牌列表为空");
            }
            var data = new PageResult<TbBrand>(total, tbBrands);
            return data;
        }

        public Dictionary<long, string> QueryBrandNameByIds(List<GoodsCategoryBrandDTO> dto)
        {
            Dictionary<long, string> dic = new Dictionary<long, string>();
            foreach (GoodsCategoryBrandDTO brand in dto)
            {
                var name = _orangeContext.TbBrand.Where(b => b.Id == brand.BrandId).FirstOrDefault().Name;
                dic.TryAdd(brand.SpuId.Value, name);
            }

            return dic;
        }

        public void SaveBrand(TbBrand brand, List<long> cids)
        {
            throw new NotImplementedException();
        }

        public void UpdateBrand(BrandBo brandbo)
        {
            throw new NotImplementedException();
        }
    }
}