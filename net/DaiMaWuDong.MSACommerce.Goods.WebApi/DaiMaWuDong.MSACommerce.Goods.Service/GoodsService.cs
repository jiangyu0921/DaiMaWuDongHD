using DaiMaWuDong.MSACommerce.Goods.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Goods.Interface;
using DaiMaWuDong.MSACommerce.Goods.Model;
using DaiMaWuDong.MSACommerce.Goods.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Goods.Service
{
    public class GoodsService : IGoodsService
    {
        private OrangeContext _orangeContext;
        private IBrandService _brandService;
        private ICategoryService _categoryService;

        public GoodsService(OrangeContext orangeContext,
            IBrandService brandService, ICategoryService categoryService)
        {
            _orangeContext = orangeContext;
            _brandService = brandService;
            _categoryService = categoryService;
        }

        public void AddGoods(TbSpu spu)
        {
            throw new NotImplementedException();
        }

        public void DeleteGoodsBySpuId(long spuId)
        {
            throw new NotImplementedException();
        }

        public void HandleSaleable(TbSpu spu)
        {
            throw new NotImplementedException();
        }



        public List<TbSku> QuerySkuBySpuId(long spuId)
        {
            List<TbSku> skuList = _orangeContext.TbSku.Where(m => m.SpuId == spuId).ToList();
            if (skuList.Count <= 0)
            {
                throw new Exception("查询的商品的SKU失败");
            }

            return skuList;
        }
        public List<long> QuerySpuIdsPage(int page, int pageSize)
        {
            return _orangeContext.TbSpu.Where(spu => spu.Id > 0)
                  .OrderBy(spu => spu.Id)
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize)
                  .Select(spu => spu.Id)
                    .ToList();
        }

        public List<TbSku> QuerySkusByIds(List<long> ids)
        {
            List<TbSku> skus = _orangeContext.TbSku.Where(m => ids.Contains(m.Id)).ToList();
            if (skus.Count <= 0)
            {
                throw new Exception("查询");
            }
            //填充库存
            FillStock(ids, skus);
            return skus;
        }

        private void FillStock(List<long> ids, List<TbSku> skus)
        {
            ////批量查询库存
            //List<TbStock> stocks = _orangeContext.TbStock.Where(m => ids.Contains(m.SkuId)).ToList();
            //if (stocks.Count <= 0)
            //{
            //    throw new Exception("保存库存失败");
            //}
            //Dictionary<long, int> map = stocks.ToDictionary(s => s.SkuId, s => s.Stock);
            ////首先将库存转换为map，key为sku的ID
            ////遍历skus，并填充库存
            //foreach (var sku in skus)
            //{
            //    sku.Stock = map[sku.Id];
            //}
        }


        public PageResult<TbSpu> QuerySpuByPage(int page, int rows, string key, bool? saleable)
        {
            var list = _orangeContext.TbSpu.AsQueryable();
            if (!string.IsNullOrEmpty(key))
            {
                list = list.Where(m => m.Title.Contains(key));
            }
            if (saleable != null)
            {
                list = list.Where(m => m.Saleable == saleable);
            }
            //默认以上一次更新时间排序
            list = list.OrderByDescending(m => m.LastUpdateTime);

            //只查询未删除的商品 
            list = list.Where(m => m.Valid == true);

            //查询
            List<TbSpu> spuList = list.ToList();

            if (spuList.Count <= 0)
            {
                throw new Exception("查询的商品不存在");
            }
            //对查询结果中的分类名和品牌名进行处理
            HandleCategoryAndBrand(spuList);
            return new PageResult<TbSpu>(spuList.Count, spuList);
        }

        /**
		 * 处理商品分类名和品牌名
		 *
		 * @param spuList
		 */
        private void HandleCategoryAndBrand(List<TbSpu> spuList)
        {
            List<GoodsCategoryBrandDTO> list = spuList.Select(g => new GoodsCategoryBrandDTO
            {
                SpuId = g.Id,
                Cid1 = g.Cid1,
                Cid2 = g.Cid2,
                Cid3 = g.Cid3,
                BrandId = g.BrandId
            }).ToList();
            ////根据spu中的分类ids查询分类名
            Dictionary<long, string> nameDic = _categoryService.QueryCategoryByThreeLevel(list);
            ////查询品牌
            Dictionary<long, string> brandDic = _brandService.QueryBrandById(list);
            ////对分类名进行处理
            foreach (var spu in spuList)
            {
                // 可以更新名称
                spu.Cname = nameDic[spu.Id];
                spu.Bname = brandDic[spu.Id];
            }

        }
        public TbSpu QuerySpuBySpuId(long spuId)
        {
            //根据spuId查询spu
            TbSpu spu = _orangeContext.TbSpu.Where(m => m.Id == spuId).FirstOrDefault();
            //查询spuDetail
            TbSpuDetail detail = QuerySpuDetailBySpuId(spuId);
            //查询skus
            List<TbSku> skus = QuerySkuBySpuId(spuId);
            spu.SpuDetail = detail;
            spu.Skus = skus;

            return spu;
        }

        public TbSpuDetail QuerySpuDetailBySpuId(long spuId)
        {
            TbSpuDetail spuDetail = _orangeContext.TbSpuDetail.Where(m => m.SpuId == spuId).FirstOrDefault();
            if (spuDetail == null)
            {
                throw new Exception("查询的商品不存在");
            }
            return spuDetail;
        }

        public void UpdateGoods(TbSpu spu)
        {
            throw new NotImplementedException();
        }
    }
}
