using Seckill.API.DTO;

namespace Seckill.API.Service.goods
{
    public interface ISeckillGoodsService
    {
        List<TbSeckillGoodDTO> ListGoods(string key);

        /// <summary>
        /// 根据ID查询商品详情
        /// </summary>
        /// <param name="time"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        TbSeckillGoodDTO QuerySeckillGoodsDetail(string time, long goodsid);
    }
}
