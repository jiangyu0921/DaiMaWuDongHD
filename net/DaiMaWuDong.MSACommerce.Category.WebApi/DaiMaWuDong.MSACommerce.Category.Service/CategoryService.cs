using DaiMaWuDong.MSACommerce.Category.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Category.Interface;
using DaiMaWuDong.MSACommerce.Category.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Category.Service
{
    public class CategoryService : ICategoryService
    {
        private OrangeContext _orangeContext;
        public CategoryService(OrangeContext orangeContext)
        {
            _orangeContext = orangeContext;
        }
        public List<TbCategory> QueryCategoryByPid(long pid)
        {
            List<TbCategory> categoryList = _orangeContext.TbCategory.Where(m => m.ParentId == pid).ToList();
            if (categoryList.Count <= 0)
            {
                throw new Exception("查询分类不存在");
            }
            return categoryList;
        }

        public List<TbCategory> QueryCategoryByIds(List<long> ids)
        {
            return _orangeContext.TbCategory.Where(m => ids.Contains(m.Id)).ToList();
        }


        public List<TbCategory> QueryAllByCid3(long id)
        {
            TbCategory c3 = _orangeContext.TbCategory.Where(m => m.Id == id).FirstOrDefault();
            TbCategory c2 = _orangeContext.TbCategory.Where(m => m.Id == c3.ParentId).FirstOrDefault();
            TbCategory c1 = _orangeContext.TbCategory.Where(m => m.Id == c2.ParentId).FirstOrDefault();
            List<TbCategory> list = new List<TbCategory>() { c1, c2, c3 };

            return list;
        }

        /// <summary>
        /// 根据一级，二级，三级类别ID查询列表名称集合
        /// 
        /// </summary>
        /// <param name="cid1"></param>
        /// <param name="cid2"></param>
        /// <param name="cid3"></param>
        /// <returns></returns>
        public Dictionary<long, string> QueryCategoryByThreeLevel(List<GoodsCategoryBrandDTO> dto)
        {
            Dictionary<long, string> dic = new Dictionary<long, string>();

            foreach (GoodsCategoryBrandDTO category in dto)
            {
                //根据spu中的分类ids查询分类名
                string cname1 = _orangeContext.TbCategory.Where(c => c.Id.Equals(category.Cid1.Value)).FirstOrDefault().Name;
                string cname2 = _orangeContext.TbCategory.Where(c => c.Id.Equals(category.Cid2.Value)).FirstOrDefault().Name;
                string cname3 = _orangeContext.TbCategory.Where(c => c.Id.Equals(category.Cid3.Value)).FirstOrDefault().Name;

                dic.Add(category.SpuId.Value, string.Join("/", new List<string> { cname1, cname2, cname3 }));
            }


            return dic;
        }

        /// <summary>
        /// 根据3级类别ID查询品牌ID
        /// </summary>
        /// <param name="cid3"></param>
        /// <returns></returns>
        public List<long> QueryBrandIdByCid(long cid3)
        {
            return _orangeContext.TbCategoryBrand.Where(c => c.CategoryId == cid3).Select(b => b.BrandId).ToList();
        }
    }
}
