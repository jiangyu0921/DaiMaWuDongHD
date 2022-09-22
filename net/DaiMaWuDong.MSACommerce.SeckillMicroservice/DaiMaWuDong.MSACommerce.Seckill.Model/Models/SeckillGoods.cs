using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Seckill.Model.Models
{
    public class SeckillGoods
    {
        public long Id { get; set; }
        public long SkuId { get; set; }
        public string Title { get; set; }
        public double SeckillPrice { get; set; }
        public string Image { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Enable { get; set; }

        public DateTime CurrentTime { get; set; }
        public int? Stock { get; set; }
        public double Price { get; set; }
        public int? SeckillTotal { get; set; }

    }
}
