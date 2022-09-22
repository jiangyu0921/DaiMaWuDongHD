using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Seckill.Model.Models
{
    public partial class TbStock
    {
        public long SkuId { get; set; }
        public int? SeckillStock { get; set; }
        public int? SeckillTotal { get; set; }
        public int Stock { get; set; }
    }
}
