using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Category.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Category.Interface;
using DaiMaWuDong.MSACommerce.Category.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DaiMaWuDong.MSACommerce.Category.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;

        }

        /**
        * 根据父类ID查询分类结果
        * @param pid
        * @return
        */
        [Route("list")]
        [HttpGet]
        public List<TbCategory> QueryCategoryByPid(long pid)
        {
            List<TbCategory> categoryList = _categoryService.QueryCategoryByPid(pid);
            return categoryList;
        }

        /**
		 * 根据商品分类Ids查询分类
		 * @param ids
		 * @return
		 */

        [Route("list/ids")]
        [HttpPost]
        public List<TbCategory> QueryCategoryByIds(List<long> ids)
        {
            return _categoryService.QueryCategoryByIds(ids);
        }

        /**
		 * 根据cid3查询三级分类
		 * @param id
		 * @return
		 */
        [Route("all/level/{id}")]
        [HttpGet]
        public JsonResult QueryAllByCid3(long id)
        {
            List<TbCategory> categoryList = this._categoryService.QueryAllByCid3(id);

            return new JsonResult(new AjaxResult()
            {
                Result = true,
                StatusCode = 200,
                Value = categoryList,
                Message = "Success"
            });
        }

        [HttpPost("GetNamesDic")]
        public Dictionary<long, string> QueryCateroyByThreeLevel(List<GoodsCategoryBrandDTO> dto)
        {
            return _categoryService.QueryCategoryByThreeLevel(dto);
        }

        [HttpGet("{cid3}")]
        public List<long> QueryBrandIdsByCid(long cid3)
        {
            return _categoryService.QueryBrandIdByCid(cid3);
        }
    }
}
