using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Seckill.EntityFrameworkCore.Converter;

namespace Seckill.API.Models
{
    public class SeckillStatus
    {
        //秒杀用户名
        public string Username { set; get; }
        //创建时间
        public DateTime CreateTime { set; get; }
        //秒杀状态  1:排队中，2:秒杀等待支付,3:支付超时，4:秒杀失败,5:支付完成
        public int Status { set; get; }
        //秒杀的商品ID
        [JsonConverter(typeof(MyNumberConverter), NumberConverterShip.Int64)]
        public long GoodsId { set; get; }

        //应付金额
        public float Money { set; get; }

        //订单号
        [JsonConverter(typeof(MyNumberConverter), NumberConverterShip.Int64)]
        public long OrderId { set; get; }
        //时间段
        public string Time { set; get; }

        public SeckillStatus()
        {

        }

        public SeckillStatus(string username, DateTime createTime, int status, long goodsId, string time)
        {
            this.Username = username;
            this.CreateTime = createTime;
            this.Status = status;
            this.GoodsId = goodsId;
            this.Time = time;
        }
    }
}
