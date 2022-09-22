using Newtonsoft.Json;
using Seckill.EntityFrameworkCore.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seckill.EntityFrameworkCore.Modles
{
    public partial class TbSeckillGood
    {
        [JsonConverter(typeof(MyNumberConverter), NumberConverterShip.Int64)]
        public long Id { get; set; }
        public long? SpuId { get; set; }
        public long? SkuId { get; set; }
        public string Name { get; set; }
        public string SmallPic { get; set; }
        public decimal? Price { get; set; }
        public decimal? CostPrice { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? CheckTime { get; set; }
        public string Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Num { get; set; }
        public int? StockCount { get; set; }
        public string Introduction { get; set; }
    }
}
