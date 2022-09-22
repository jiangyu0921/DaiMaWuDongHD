using DaiMaWuDong.MSACommerce.Brand.DTOModel.DTO;
using DaiMaWuDong.MSACommerce.Brand.Interface;
using DaiMaWuDong.MSACommerce.Brand.Model;
using DaiMaWuDong.MSACommerce.Brand.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Brand.WebApi.Controllers
{
    //api/item/brand
    [Route("api/item/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private IBrandService _brandService;
        private readonly ILogger<BrandController> _logger;
        public BrandController(IBrandService brandService, ILogger<BrandController> logger)
        {
            _logger = logger;
            _brandService = brandService;
        }
        [Route("page")]
        [HttpGet]
        public PageResult<TbBrand> QueryBrandByPage(string sortBy, bool desc, string key, int page = 1, int rows = 10)
        {
            _logger.LogWarning("这是一个警告");
            var data = _brandService.QueryBrandByPageAndSort(page, rows, sortBy, desc, key);
            return data;
            //var setting = new JsonSerializerSettings
            //{
            //	ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            //};

            //var str = Newtonsoft.Json.JsonConvert.SerializeObject(data, Formatting.None, setting);
            //return str;
        }

        [Route("cid/{cid}")]
        [HttpGet]
        public List<TbBrand> QueryBrandByCid(long cid)
        {
            var data = _brandService.QueryBrandByCid(cid);
            return data;
            //return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }


        [Route("{id}")]
        [HttpGet]
        public TbBrand QueryById(long id)
        {
            var data = _brandService.QueryBrandByBid(id);
            return data;
        }

        [HttpPost("list")]
        public List<TbBrand> queryBrandsByIds(List<long> ids)
        {
            var data = _brandService.QueryBrandByIds(ids);
            return data;
        }
        /**
     * 新增品牌
     *
     * @param brand
     * @param cids  品牌所在的分类ID（多个分类）
     * @return
     */
        [HttpPost]
        public void AddBrand([FromQuery] TbBrand brand, List<long> cids)
        {
            _brandService.SaveBrand(brand, cids);
        }


        /**
		 * 更新品牌
		 *
		 * @param brandBo
		 * @return
		 */
        [HttpPut]
        public void UpdateBrand(BrandBo brandBo)
        {
            _brandService.UpdateBrand(brandBo);
        }
        /**
		 * 删除品牌
		 *
		 * @param bid
		 * @return
		 */
        [Route("bid/{bid}")]
        [HttpDelete]
        public void DeleteBrand(long bid)
        {
            _brandService.DeleteBrand(bid);
        }


        [HttpPost("GetBrandNameDic")]
        public Dictionary<long, string> GetBrandNameDic(List<GoodsCategoryBrandDTO> dto)
        {
            return _brandService.QueryBrandNameByIds(dto);
        }
    }
}
