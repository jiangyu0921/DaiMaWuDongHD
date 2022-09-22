using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Seckill.API.DTO;
using Seckill.API.Service.goods;
using Seckill.Common.Helper;

namespace Seckill.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeckillGoodsController : ControllerBase
    {
        private readonly ISeckillGoodsService _seckillGoodsService;
        public SeckillGoodsController(ISeckillGoodsService seckillGoodsService)
        {
            this._seckillGoodsService = seckillGoodsService;
        }

        /// <summary>
        /// 查询时间菜单
        /// </summary>
        /// <returns></returns>
        [Route("/api/seckill/goods/menus")]
        [HttpGet]
        public List<DateTime> DateMenus()
        {
            return DateHelper.GetDateMenus();
        }

        /// <summary>
        /// 查询频道秒杀列表
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        [Route("/api/seckill/goods/list")]
        [HttpGet]
        public List<TbSeckillGoodDTO> ListGoods(string time)
        {
            //调用Service查询数据
            return _seckillGoodsService.ListGoods(time);
        }

        /// <summary>
        /// 查询某个秒杀时段的具体商品详情信息
        /// </summary>
        /// <param name="time"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        [Route("/api/seckill/goods/detail")]
        [HttpGet]
        public TbSeckillGoodDTO QuerySeckillGoodsDetail(string time, long goodsId)
        {
            //调用Service查询数据
            return _seckillGoodsService.QuerySeckillGoodsDetail(time, goodsId);
        }
    }
}
