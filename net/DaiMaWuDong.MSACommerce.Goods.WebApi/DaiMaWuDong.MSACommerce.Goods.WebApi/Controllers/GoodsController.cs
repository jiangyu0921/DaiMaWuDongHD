using DaiMaWuDong.MSACommerce.Goods.Interface;
using DaiMaWuDong.MSACommerce.Goods.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Goods.WebApi.Controllers
{
    [Route("api/item/[controller]")]
    [ApiController]
    public class GoodsController : ControllerBase
    {
        private IGoodsService _goodsService;
        public GoodsController(IGoodsService goodsService)
        {
            _goodsService = goodsService;
        }
        [Route("spu/page")]
        [HttpGet]
        public string QuerySpuByPage(int page, int rows, string key, bool saleable)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(_goodsService.QuerySpuByPage(page, rows, key, saleable));
        }

        /**
		 * 查询spu详情
		 * @param spuId
		 * @return
		 */

        [Route("spu/detail/{spuId}")]
        [HttpGet]
        public TbSpuDetail QuerySpuDetailBySpuId(long spuId)
        {
            return _goodsService.QuerySpuDetailBySpuId(spuId);
        }

        /**
		 * 根据spuId查询商品详情
		 * @param id
		 * @return
		 */
        [Route("sku/list")]
        [HttpGet]
        public List<TbSku> QuerySkuBySpuId(long id)
        {
            return _goodsService.QuerySkuBySpuId(id);

        }

        /**
		 * 根据sku ids查询sku
		 * @param ids
		 * @return
		 */
        [Route("sku/list/ids")]
        [HttpGet]
        public List<TbSku> QuerySkusByIds(List<long> ids)
        {
            return _goodsService.QuerySkusByIds(ids);
        }


        /**
		 * 删除商品
		 * @param spuId
		 * @return
		 */
        [Route("spu/spuId/{spuId}")]
        [HttpDelete]
        public void DeleteGoodsBySpuId(long spuId)
        {
            _goodsService.DeleteGoodsBySpuId(spuId);
        }


        /**
		 * 添加商品
		 * @param spu
		 * @return
		 */
        [Route("goods")]
        [HttpPost]

        public void addGoods([FromBody] TbSpu spu)
        {
            _goodsService.AddGoods(spu);
        }

        /**
		 * 更新商品
		 * @param spu
		 * @return
		 */
        [Route("goods")]
        [HttpPut]
        public void UpdateGoods([FromBody] TbSpu spu)
        {
            _goodsService.UpdateGoods(spu);
        }

        [Route("spu/saleable")]
        [HttpPut]
        public void handleSaleable([FromBody] TbSpu spu)
        {
            _goodsService.HandleSaleable(spu);


        }

        /**
		 * 根据spuId查询spu及skus
		 * @param spuId
		 * @return
		 */
        [Route("spu/{spuId}")]
        [HttpGet]
        public TbSpu QuerySpuBySpuId(long spuId)
        {
            return _goodsService.QuerySpuBySpuId(spuId);
        }
    }
}
