using DaiMaWuDong.MSACommerce.Category.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Category.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Category.Interface
{
    public interface ICategoryService
    {
        List<TbCategory> QueryCategoryByPid(long pid);

        List<TbCategory> QueryCategoryByIds(List<long> ids);

        List<TbCategory> QueryAllByCid3(long id);

        Dictionary<long, string> QueryCategoryByThreeLevel(List<GoodsCategoryBrandDTO> dto);

        /// <summary>
        /// 根据三级类别ID查询品牌ID集合
        /// </summary>
        /// <param name="cid3"></param>
        /// <returns></returns>
        List<long> QueryBrandIdByCid(long cid3);
    }
}
